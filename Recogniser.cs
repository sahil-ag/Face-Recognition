using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV;
using System.IO;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;

namespace Hack_in_the_north_hand_mouse
{
    public partial class Recogniser : Form
    {

        private Image<Bgr, Byte> ImageFrame;
        private bool captureInProgress;
        private Capture capture;
        private int faceNo;
        private List<Bitmap> ExtFaces;
        private CascadeClassifier haarCascade;
        private double scaleFactor = 1.1;
        private int minNeighbors = 10;
        SQLiteConnection _sqLiteConnection;

        private bool live;

        private FaceRecognizer _faceRecognizer;
        private bool isTrained;

        public Recogniser()
        {
            _faceRecognizer = new EigenFaceRecognizer(80);
            isTrained = false;
            captureInProgress = false;
            haarCascade = new CascadeClassifier(@"haarcascade_frontalface_default.xml"); 
            _sqLiteConnection = new SQLiteConnection(String.Format("Data Source=face.sqlite;Version=3;"));
            live = false;
            InitializeComponent();
        }

        private void ReleaseData() /* Release data when task is over */
        {
            if (capture != null)
            {
                capture.Dispose();
            }
        }

        public bool TrainRecognizer()
        {
            var allFaces = new List<criminals_pic>();
            try
            {
                _sqLiteConnection.Open();
                var query = "SELECT * FROM c_people";
                var cmd = new SQLiteCommand(query, _sqLiteConnection);
                var result = cmd.ExecuteReader(); 
                while (result.Read())
                {
                    var criminal_pic = new criminals_pic
                    {
                        sample = (byte[])result["sample"],
                        userID = Convert.ToInt32(result["userID"]),
                    };
                    allFaces.Add(criminal_pic);
                }
                allFaces = allFaces.OrderBy(f => f.userID).ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            
            if (allFaces != null)
            {
                //MessageBox.Show("allFaces.Count.ToString() " + allFaces.Count.ToString());
                var faceImages = new Image<Gray, Byte>[allFaces.Count];
                var faceLabels = new int[allFaces.Count];
                for (int i = 0; i < allFaces.Count; i++)
                {
                    Stream stream = new MemoryStream();
                    stream.Write(allFaces[i].sample, 0, allFaces[i].sample.Length);
                    var faceImage = new Image<Gray, Byte>(new Bitmap(stream));
                    faceImages[i] = faceImage.Resize(100, 100, Inter.Cubic);
                    faceLabels[i] = allFaces[i].userID;
                }
                _faceRecognizer.Train(faceImages, faceLabels);
            }
            return true;
        }






        public FaceRecognizer.PredictionResult RecognizeUser(Image<Gray, Byte> userImage)
        {
            var result = _faceRecognizer.Predict(userImage.Resize(100, 100, Inter.Cubic));
            return result;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
           if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Image InputImg = Image.FromFile(openFileDialog.FileName);
                ImageFrame = new Image<Bgr, Byte>(new Bitmap(InputImg));
                CamImageBox.Image = ImageFrame;
                if (ExtFaces == null)
                    ExtFaces = new List<Bitmap>();
                else
                    ExtFaces.Clear();
                DetectFace2();
            }
            btnNext.Show();
            btnPrev.Show();
        }


        private void btnPrev_Click(object sender, EventArgs e)
        {
            
            if (faceNo > 0)
            {
                faceNo--;
               // MessageBox.Show("faceno prev : " + faceNo);
                pbExtractedFaces.Image = new Image<Gray, Byte>(ExtFaces.ElementAt(faceNo));
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("faceno prev : " + faceNo);
            if (faceNo < ExtFaces.Count - 1)
            {
                faceNo++;
               // MessageBox.Show("facenext : " + faceNo);
                pbExtractedFaces.Image = new Image<Gray, Byte>(ExtFaces.ElementAt(faceNo));
            }
        }

        private void DetectFace2()
        {
            Image<Gray, byte> grayframe = ImageFrame.Convert<Gray, byte>();
            if (haarCascade == null)
            {
                haarCascade = new CascadeClassifier(@"haarcascade_frontalface_default.xml");
            }

            var faces = haarCascade.DetectMultiScale(grayframe, scaleFactor, minNeighbors);
            if (faces.Length > 0)
            {
                Bitmap BmpInput = grayframe.ToBitmap();
                Bitmap ExtractedFace;   //empty
                Graphics FaceCanvas;
                if (ExtFaces == null)
                    ExtFaces = new List<Bitmap>();
                else
                    ExtFaces.Clear();
                faceNo = 0;

                //draw a green rectangle on each detected face in image
                foreach (var face in faces)
                {
                    ImageFrame.Draw(face, new Bgr(Color.Green), 3);

                    //set the size of the empty box(ExtractedFace) which will later contain the detected face
                    ExtractedFace = new Bitmap(face.Width, face.Height);

                    //set empty image as FaceCanvas, for painting
                    FaceCanvas = Graphics.FromImage(ExtractedFace);

                    FaceCanvas.DrawImage(BmpInput, 0, 0, face, GraphicsUnit.Pixel);

                    ExtFaces.Add(ExtractedFace);
                    faceNo++;
                }
                //MessageBox.Show("faceno : " + faceNo);

                pbExtractedFaces.Image = new Image<Gray, Byte>(ExtFaces.ElementAt(0));

                //Display the detected faces in imagebox
                CamImageBox.Image = ImageFrame;
            }
        }



        private void btnClick_Click(object sender, EventArgs e)
        {
            live = true;
            btnPrev.Hide();
            btnNext.Hide();
            if (isTrained == false)
            {
                isTrained = TrainRecognizer();
            }
            
            if (capture == null)
            {
                try
                {
                    capture = new Capture();
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
            if (captureInProgress)
            {
                btnClick.Text = "Begin Live Detection!";
                Application.Idle -= ProcessFrame;
            }
            else
            {
                btnClick.Text = "Stop Live Detection";
                Application.Idle += ProcessFrame;               
            }
            captureInProgress = !captureInProgress;

        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            Mat temp = capture.QueryFrame();

            ImageFrame = temp.ToImage<Bgr, Byte>();
            CamImageBox.Image = ImageFrame;
            DetectFace();
        }

        void DetectFace()
        {
            Image<Gray, Byte> grayframe = ImageFrame.Convert<Gray, Byte>();
            if (haarCascade == null)
            {
                haarCascade = new CascadeClassifier(@"haarcascade_frontalface_default.xml");
            }
            //detect faces from the gray-scale image and store into an array of type 'var',i.e 'MCvAvgComp[]'
            var faces = haarCascade.DetectMultiScale(grayframe, scaleFactor, minNeighbors);

            Bitmap BmpInput = grayframe.ToBitmap();
            Bitmap ExtractedFace;   //empty
            Graphics FaceCanvas;
            List<string> namesDetected = new List<string>();
            if (ExtFaces == null)
                ExtFaces = new List<Bitmap>();
           else
               ExtFaces.Clear();
            foreach (var face in faces)
            {
                ExtractedFace = new Bitmap(face.Width, face.Height);

                FaceCanvas = Graphics.FromImage(ExtractedFace);
                FaceCanvas.DrawImage(BmpInput, 0, 0, face, GraphicsUnit.Pixel);
                FaceRecognizer.PredictionResult ans = RecognizeUser(new Image<Gray, Byte>(ExtractedFace));
                if (ans.Distance < 3000.0)
                {
                    //MessageBox.Show("OK! " + ans.Distance.ToString());
                    namesDetected.Add(GetUsername(ans.Label));
                    ImageFrame.Draw(face, new Bgr(Color.Red), 3);
                    ExtFaces.Add(ExtractedFace);
                    faceNo++;
                    pbExtractedFaces.Image = new Image<Gray, Byte>(ExtFaces.ElementAt(0));
                    CamImageBox.Image = ImageFrame;
                    showNames.Text = String.Join(Environment.NewLine, namesDetected);

                    btnNext.Enabled = true;
                    btnPrev.Enabled = true;
                } else
                {
                    ImageFrame.Draw(face, new Bgr(Color.Green), 3);
                }
            }

            //CamImageBox.Image = ImageFrame;
            showNames.Text = String.Join(Environment.NewLine, namesDetected);
        }



        public string GetUsername(int userId)
        {
            var username = "";
            try
            {
                _sqLiteConnection.Open();
                var selectQuery = "SELECT name FROM c_people_data WHERE userID=@userId LIMIT 1";
                var cmd = new SQLiteCommand(selectQuery, _sqLiteConnection);
                cmd.Parameters.AddWithValue("userId", userId);
                var result = cmd.ExecuteReader();
                if (!result.HasRows) return username;
                while (result.Read())
                {
                    username = (String)result["name"];

                }
            }
            catch
            {
                return username;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            return username;
        }



        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _faceRecognizer.Load(openFileDialog.FileName);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _faceRecognizer.Save(saveFileDialog.FileName);
            }
        }

       
    }
}

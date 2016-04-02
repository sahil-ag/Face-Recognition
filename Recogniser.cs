using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;

using Emgu.CV.Face;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using Emgu.CV.CvEnum;

namespace Hack_in_the_north_hand_mouse
{
    public partial class Recogniser : Form
    {
        private FaceRecognizer _faceRecognizer;
        private DataStoreface _dataStore;
        private bool isTrained;

        private bool captureInProgress;
        Capture capture;
        Image<Bgr, Byte> ImageFrame;
        CascadeClassifier haarCascade;
        public Recogniser()
        {
            _dataStore = new DataStoreface();
            _faceRecognizer = new EigenFaceRecognizer(80);
            isTrained = false;
            captureInProgress = false;
            haarCascade = new CascadeClassifier(@"haarcascade_frontalface_default.xml");
            InitializeComponent();
        }

        public bool TrainRecognizer()
        {
            var allFaces = _dataStore.CallFaces("ALL_USERS");
            if (allFaces != null)
            {
                var faceImages = new Image<Gray, byte>[allFaces.Count];
                var faceLabels = new int[allFaces.Count];
                for (int i = 0; i < allFaces.Count; i++)
                {
                    Stream stream = new MemoryStream();
                    stream.Write(allFaces[i].Image, 0, allFaces[i].Image.Length);
                    var faceImage = new Image<Gray, byte>(new Bitmap(stream));
                    faceImages[i] = faceImage.Resize(100, 100, Inter.Cubic);
                    faceLabels[i] = allFaces[i].UserId;
                }
                _faceRecognizer.Train(faceImages, faceLabels);
            }
            return true;

        }

        public FaceRecognizer.PredictionResult RecognizeUser(Image<Gray, byte> userImage)
        {
            var result = _faceRecognizer.Predict(userImage.Resize(100, 100, Inter.Cubic));
            return result;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (isTrained == false)
            {
                TrainRecognizer();
            }
            else
            {
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
                    btnStart.Text = "Start!";
                    Application.Idle -= ProcessFrame;
                    capture.Dispose();
                }
                else
                {
                    btnStart.Text = "Stop";
                    Application.Idle += ProcessFrame;
                }
                captureInProgress = !captureInProgress;

            }
        }

        private void ReleaseData()
        {
            if (capture != null)
            {
                capture.Dispose();
            }
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
            Image<Gray, byte> grayframe = ImageFrame.Convert<Gray, byte>();
            if (haarCascade == null)
            {
                haarCascade = new CascadeClassifier(@"haarcascade_frontalface_default.xml");
            }
            double scaleFactor = 1.1;
            int minNeighbors = 10;
            //detect faces from the gray-scale image and store into an array of type 'var',i.e 'MCvAvgComp[]'
            var faces = haarCascade.DetectMultiScale(grayframe, scaleFactor, minNeighbors);

            Bitmap BmpInput = grayframe.ToBitmap();
            Bitmap ExtractedFace;   //empty
            Graphics FaceCanvas;
            List<string> namesDetected = new List<string>();
            
            foreach (var face in faces)
            {
                ExtractedFace = new Bitmap(face.Width, face.Height);

                FaceCanvas = Graphics.FromImage(ExtractedFace);
                FaceCanvas.DrawImage(BmpInput, 0, 0, face, GraphicsUnit.Pixel);
                FaceRecognizer.PredictionResult ans = RecognizeUser(new Image<Gray, Byte>(ExtractedFace));
                if (ans.Distance < 3000.0)
                {
                        namesDetected.Add(_dataStore.GetUsername(ans.Label));
                }
            }
            showNames.Text = String.Join(Environment.NewLine, namesDetected);
        }

        private void btnBrowseImg_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image InputImg = Image.FromFile(openFileDialog1.FileName);
                ImageFrame = new Image<Bgr, byte>(new Bitmap(InputImg));
                CamImageBox.Image = ImageFrame;

                DetectFace();
            }
        }
    }
}

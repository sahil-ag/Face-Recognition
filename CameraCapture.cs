using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using System.IO;

namespace Hack_in_the_north_hand_mouse
{
    public partial class CameraCapture : Form
    {
        //declaring global variable
        private Capture capture;
        private bool captureInProgress;
        Image<Bgr, Byte> ImageFrame;
        CascadeClassifier haarCascade;
        Bitmap [] ExtFaces;
        int faceNo;
        public CameraCapture()
        {
            InitializeComponent(); /* Initialise all the components */
        }

        private void ReleaseData() /* Release data when task is over */
        {
            if( capture != null)
            {
                capture.Dispose();
            }
        }

        /* start button click event */
        private void btnStart_Click(object sender, EventArgs e) 
        {
            #region if capture is not created, create it now
            if( capture == null) 
            {
                try
                {
                    capture = new Capture();
                } catch(NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
            #endregion

            if( capture != null) /* If capture is created */
            {
                if( captureInProgress) /* If true */
                {
                    btnStart.Text = "Start!";
                    Application.Idle -= ProcessFrame;
                    DetectFace();
                }
                else
                {
                    btnStart.Text = "Stop";
                    Application.Idle += ProcessFrame;
                }
                captureInProgress = !captureInProgress;
            }

        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            Mat temp = capture.QueryFrame();
            
            ImageFrame = temp.ToImage<Bgr, Byte>();
            CamImageBox.Image = ImageFrame;
            
            Image<Gray, byte> grayframe = ImageFrame.Convert<Gray, byte>(); /* Converting ImageFrame to grayframe */
            if (haarCascade == null) 
            {
                haarCascade = new CascadeClassifier(@"haarcascade_frontalface_default.xml"); /* Using front facial recogniser */
            }
            double scaleFactor = 1.1; /* 10% scaleFactor */
            int minNeighbors = 10;

            //detect faces from the gray-scale image and store into an array of type 'var',i.e 'MCvAvgComp[]'
            var faces = haarCascade.DetectMultiScale(grayframe, scaleFactor, minNeighbors);

            /* If some faces are detected */
            if (faces.Length > 0)
            {
                //MessageBox.Show("Total Faces Detected: " + faces.Length.ToString());
                Bitmap BmpInput = grayframe.ToBitmap(); /* grayframe to Bitmap, easy pixel implementation */
                Bitmap ExtractedFace;   //empty
                Graphics FaceCanvas;

                //draw a green rectangle on each detected face in image
                foreach (var face in faces)
                {
                    ImageFrame.Draw(face, new Bgr(Color.Green), 3);

                    ExtractedFace = new Bitmap(face.Width, face.Height);

                    FaceCanvas = Graphics.FromImage(ExtractedFace);
                    FaceCanvas.DrawImage(BmpInput, 0, 0, face, GraphicsUnit.Pixel);
                }
            }
            CamImageBox.Image = ImageFrame;
        }

        private void DetectFace()
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
            if (faces.Length > 0)
            {
                //MessageBox.Show("Total Faces Detected: " + faces.Length.ToString());
                Bitmap BmpInput = grayframe.ToBitmap();
                Bitmap ExtractedFace;   //empty
                Graphics FaceCanvas;
                ExtFaces = new Bitmap[faces.Length];
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

                    ExtFaces[faceNo] = ExtractedFace;
                    faceNo++;
                }

                pbExtractedFaces.Image = new Image<Gray, Byte>(ExtFaces[0]);

                //Display the detected faces in imagebox
                CamImageBox.Image = ImageFrame;

                btnNext.Enabled = true;
                btnPrev.Enabled = true;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (faceNo < ExtFaces.Length-1)
            {
                faceNo++;
                pbExtractedFaces.Image = new Image<Gray, Byte>(ExtFaces[faceNo]);
                Console.Out.Write(faceNo + " ");
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (faceNo > 0)
            {
                faceNo--;
                pbExtractedFaces.Image = new Image<Gray, Byte>(ExtFaces[faceNo]);
            }
        }
            

        private DataStoreface _dataStore = new DataStoreface();

        private void btnSave_Click(object sender, EventArgs e)
        {
            string username = txtUserName.Text;
            
            Bitmap temp = pbExtractedFaces.Image.Bitmap;
            MemoryStream _memory = new MemoryStream();
            temp.Save(_memory, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] sample = _memory.ToArray();

            _dataStore.SaveFace(username, sample);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Recogniser fcrec = new Recogniser();
            fcrec.Show();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
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

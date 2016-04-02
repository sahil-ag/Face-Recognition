using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sqlite;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;


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
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ReleaseData()
        {
            if( capture != null)
            {
                capture.Dispose();
            }
        }
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

            if( capture != null)
            {
                if( captureInProgress)
                {
                    btnStart.Text = "Start!";
                    Application.Idle -= ProcessFrame;
                    DetectFace();
                } else
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
            //CamImageBox.Image = ImageFrame;
            
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
    }

}

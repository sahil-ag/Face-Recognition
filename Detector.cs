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

namespace Hack_in_the_north_hand_mouse
{
    public partial class Detector : Form
    {
        private Image<Bgr, byte> ImageFrame;
        private bool captureInProgress;
        private Capture capture;
        private int faceNo;
        private List<Bitmap> ExtFaces;
        private CascadeClassifier haarCascade;
        private double scaleFactor = 1.1;
        private int minNeighbors = 10;

        public Detector()
        {
            _sqLiteConnection = new SQLiteConnection(String.Format("Data Source=face.sqlite;Version=3;"));
            InitializeComponent();
        }
        private void ReleaseData() /* Release data when task is over */
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
            MarkFace();
        }


        private void MarkFace()
        {
            Image<Gray, byte> grayframe = ImageFrame.Convert<Gray, byte>();
            if (haarCascade == null)
            {
                haarCascade = new CascadeClassifier(@"haarcascade_frontalface_default.xml");
            }
            

            var faces = haarCascade.DetectMultiScale(grayframe, scaleFactor, minNeighbors);
            if (faces.Length > 0)
            {
                foreach (var face in faces)
                    ImageFrame.Draw(face, new Bgr(Color.Green), 3);
            }
        }




        private void DetectFace()
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

                pbExtractedFaces.Image = new Image<Gray, Byte>(ExtFaces.ElementAt(0));

                //Display the detected faces in imagebox
                CamImageBox.Image = ImageFrame;

                btnNext.Enabled = true;
                btnPrev.Enabled = true;
                btnSave.Enabled = true;
                btnReset.Enabled = true;

                txtAddress.Enabled = true;
                txtName.Enabled = true;
                txtAge.Enabled = true;
                txtCity.Enabled = true;
                txtState.Enabled = true;

                radioNo.Enabled = true;
                radioYes.Enabled = true;
            }
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
                ImageFrame = new Image<Bgr, byte>(new Bitmap(InputImg));
                CamImageBox.Image = ImageFrame;
                if (ExtFaces == null)
                    ExtFaces = new List<Bitmap>();
                else
                    ExtFaces.Clear();
                DetectFace();
            }
        }

        private void btnClick_Click(object sender, EventArgs e)
        {
            #region if capture is not created, create it now
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
            #endregion

            if (capture != null) /* If capture is created */
            {
                if (captureInProgress) /* If true */
                {
                    btnClick.Text = "Click Picture Start!";
                    Application.Idle -= ProcessFrame;
                    DetectFace();
                }
                else
                {
                    btnClick.Text = "Click Picture Stop";
                    Application.Idle += ProcessFrame;
                }
                captureInProgress = !captureInProgress;
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (faceNo > 0)
            {
                faceNo--;
                pbExtractedFaces.Image = new Image<Gray, Byte>(ExtFaces.ElementAt(faceNo));
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (faceNo < ExtFaces.Count - 1)
            {
                faceNo++;
                pbExtractedFaces.Image = new Image<Gray, Byte>(ExtFaces.ElementAt(faceNo));
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtState.Text = "";
            txtName.Text = "";
            txtCity.Text = "";
            txtAge.Text = "";
            txtAddress.Text = "";
            radioNo.Checked = false;
            radioNo.Checked = false;
        }

        private SQLiteConnection _sqLiteConnection;
        private void btnSave_Click(object sender, EventArgs e)
        {
            string Name = txtName.Text;
            string Age = txtAge.Text;
            string Address = txtAddress.Text;
            string city = txtCity.Text;
            string state = txtState.Text;
            bool criminal = (radioYes.Checked==true) ? true : false;
            MessageBox.Show(Name + " " + Age + " " + " criminal " + criminal);
            try
            {
                var exisitingUserId = GetUserId(Name, Age, Address, city, state, criminal);
               
                if (exisitingUserId == 0)
                    exisitingUserId = GenerateUserId(Name, Age, Address, city, state, criminal);

                Console.WriteLine("part1 - " + exisitingUserId.ToString());

                _sqLiteConnection.Open();
                Bitmap temp = pbExtractedFaces.Image.Bitmap;
                MemoryStream _memory = new MemoryStream();
                temp.Save(_memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] faceBlob = _memory.ToArray();

                Console.WriteLine("part8 - " + exisitingUserId.ToString());

                if (!criminal)
                {
                    Console.WriteLine("part2 - " + exisitingUserId.ToString() + "!criminal " + criminal);

                    var insertQuery = "INSERT INTO i_people(sample, userID) VALUES(@faceSample,@userId)";
                    var cmd = new SQLiteCommand(insertQuery, _sqLiteConnection);
                    cmd.Parameters.AddWithValue("userId", exisitingUserId);
                    cmd.Parameters.Add("faceSample", DbType.Binary, faceBlob.Length).Value = faceBlob;

                    Console.WriteLine("part3 - " + exisitingUserId.ToString());

                    var result = cmd.ExecuteNonQuery(); //problem
                    MessageBox.Show(String.Format("{0} face(s) saved successfully", result));
                }
                else
                {
                    Console.WriteLine("part4 - " + exisitingUserId.ToString());

                    var insertQuery = "INSERT INTO c_people (userID, sample) VALUES(@userId,@faceSample)";
                    var cmd = new SQLiteCommand(insertQuery, _sqLiteConnection);
                    cmd.Parameters.AddWithValue("userId", exisitingUserId);
                    cmd.Parameters.Add("faceSample", DbType.Binary, faceBlob.Length).Value = faceBlob;

                    Console.WriteLine("part5 - " + exisitingUserId.ToString());

                    var result = cmd.ExecuteNonQuery(); //problem
                    Console.WriteLine("part10 - " + exisitingUserId.ToString());
                    MessageBox.Show(String.Format("{0} face(s) saved successfully", result));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally 
            {
                MessageBox.Show("CLosing ");
                _sqLiteConnection.Close();
            }
        }

        int GetUserId(string name, string age, string address, string city, string state, bool criminal)
        {
            var userId = 0;
            try
            {
                _sqLiteConnection.Open();
                if (!criminal)
                {
                    var selectQuery = "SELECT userID FROM i_people_data WHERE name=@name AND age=@age AND address=@address AND city=@city AND state=@state LIMIT 1";
                    var cmd = new SQLiteCommand(selectQuery, _sqLiteConnection);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("age", age);
                    cmd.Parameters.AddWithValue("address", address);
                    cmd.Parameters.AddWithValue("city", city);
                    cmd.Parameters.AddWithValue("state", state);
                    var result = cmd.ExecuteReader();
                    if (!result.HasRows)
                        return 0;
                    result.Read();
                    userId = Convert.ToInt32(result["userID"]);
                    Console.WriteLine("if userid - " + userId.ToString());
                }
                else
                {
                    var selectQuery = "SELECT userID FROM c_people_data WHERE name=@name AND age=@age AND address=@address AND city=@city AND state=@state LIMIT 1";
                    var cmd = new SQLiteCommand(selectQuery, _sqLiteConnection);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("age", age);
                    cmd.Parameters.AddWithValue("address", address);
                    cmd.Parameters.AddWithValue("city", city);
                    cmd.Parameters.AddWithValue("state", state);
                    var result = cmd.ExecuteReader();
                    if (!result.HasRows)
                        return 0;
                    result.Read();
                    userId = Convert.ToInt32(result["userID"]);
                    Console.WriteLine("else userid - " + userId.ToString());
                }
                _sqLiteConnection.Close();
            }
            catch
            {
                Console.WriteLine("catch userid - " + userId.ToString());
                return userId;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            return userId;
        }

        int GenerateUserId (string name, string age, string address, string city, string state, bool criminal)
        {
            var date = DateTime.Now.ToString("MMddHHmmss");
            var exisitingUserId = Convert.ToInt32(date);
            try
            {
                _sqLiteConnection.Open();
                if (!criminal)
                {
                    var insertQuery = "INSERT INTO i_people_data(userID, name, age, address, city, state) VALUES(@userID, @name, @age, @address, @city, @state)";
                    var cmd = new SQLiteCommand(insertQuery, _sqLiteConnection);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("age", age);
                    cmd.Parameters.AddWithValue("address", address);
                    cmd.Parameters.AddWithValue("city", city);
                    cmd.Parameters.AddWithValue("state", state);
                    cmd.Parameters.AddWithValue("userID", exisitingUserId);
                    var result = cmd.ExecuteNonQuery();
                    //MessageBox.Show(String.Format("{0} face(s) saved successfully", result));
                }
                else
                {
                    var insertQuery = "INSERT INTO c_people_data(userID, name, age, address, city, state) VALUES(@userID, @name, @age, @address, @city, @state)";
                    var cmd = new SQLiteCommand(insertQuery, _sqLiteConnection);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("age", age);
                    cmd.Parameters.AddWithValue("address", address);
                    cmd.Parameters.AddWithValue("city", city);
                    cmd.Parameters.AddWithValue("state", state);
                    cmd.Parameters.AddWithValue("userID", exisitingUserId);
                    var result = cmd.ExecuteNonQuery();
                    //MessageBox.Show(String.Format("{0} face(s) saved successfully", result));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            return Convert.ToInt32(date);
        }
    }
}

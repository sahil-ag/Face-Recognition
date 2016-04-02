namespace Hack_in_the_north_hand_mouse
{
    partial class Recogniser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.CamImageBox = new Emgu.CV.UI.ImageBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnBrowseImg = new System.Windows.Forms.Button();
            this.btnBrowseVid = new System.Windows.Forms.Button();
            this.showNames = new System.Windows.Forms.Label();
            this.btnTrain = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.CamImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // CamImageBox
            // 
            this.CamImageBox.Location = new System.Drawing.Point(12, 12);
            this.CamImageBox.Name = "CamImageBox";
            this.CamImageBox.Size = new System.Drawing.Size(655, 426);
            this.CamImageBox.TabIndex = 2;
            this.CamImageBox.TabStop = false;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(688, 34);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(130, 39);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnBrowseImg
            // 
            this.btnBrowseImg.Location = new System.Drawing.Point(12, 456);
            this.btnBrowseImg.Name = "btnBrowseImg";
            this.btnBrowseImg.Size = new System.Drawing.Size(157, 23);
            this.btnBrowseImg.TabIndex = 5;
            this.btnBrowseImg.Text = "Browse exsisting Image";
            this.btnBrowseImg.UseVisualStyleBackColor = true;
            // 
            // btnBrowseVid
            // 
            this.btnBrowseVid.Location = new System.Drawing.Point(209, 456);
            this.btnBrowseVid.Name = "btnBrowseVid";
            this.btnBrowseVid.Size = new System.Drawing.Size(157, 23);
            this.btnBrowseVid.TabIndex = 6;
            this.btnBrowseVid.Text = "Browse exsisting Video";
            this.btnBrowseVid.UseVisualStyleBackColor = true;
            // 
            // showNames
            // 
            this.showNames.AutoSize = true;
            this.showNames.Location = new System.Drawing.Point(685, 151);
            this.showNames.Name = "showNames";
            this.showNames.Size = new System.Drawing.Size(0, 13);
            this.showNames.TabIndex = 7;
            // 
            // btnTrain
            // 
            this.btnTrain.Location = new System.Drawing.Point(689, 84);
            this.btnTrain.Name = "btnTrain";
            this.btnTrain.Size = new System.Drawing.Size(128, 45);
            this.btnTrain.TabIndex = 8;
            this.btnTrain.Text = "Train";
            this.btnTrain.UseVisualStyleBackColor = true;
            this.btnTrain.Click += new System.EventHandler(this.btnTrain_Click);
            // 
            // Recogniser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 491);
            this.Controls.Add(this.btnTrain);
            this.Controls.Add(this.showNames);
            this.Controls.Add(this.btnBrowseVid);
            this.Controls.Add(this.btnBrowseImg);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.CamImageBox);
            this.Name = "Recogniser";
            this.Text = "Recogniser";
            ((System.ComponentModel.ISupportInitialize)(this.CamImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox CamImageBox;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnBrowseImg;
        private System.Windows.Forms.Button btnBrowseVid;
        private System.Windows.Forms.Label showNames;
        private System.Windows.Forms.Button btnTrain;
    }
}
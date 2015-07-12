namespace HueMusicViz
{
    partial class MainForm
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
            this._labelSong = new System.Windows.Forms.Label();
            this.songLabel = new System.Windows.Forms.Label();
            this._labelTime = new System.Windows.Forms.Label();
            this.trackTimeLabel = new System.Windows.Forms.Label();
            this.visualizer = new System.Windows.Forms.PictureBox();
            this.beatLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.visualizer)).BeginInit();
            this.SuspendLayout();
            // 
            // _labelSong
            // 
            this._labelSong.AutoSize = true;
            this._labelSong.Location = new System.Drawing.Point(12, 9);
            this._labelSong.Name = "_labelSong";
            this._labelSong.Size = new System.Drawing.Size(72, 13);
            this._labelSong.TabIndex = 0;
            this._labelSong.Text = "Current Song:";
            // 
            // songLabel
            // 
            this.songLabel.AutoSize = true;
            this.songLabel.Location = new System.Drawing.Point(90, 9);
            this.songLabel.Name = "songLabel";
            this.songLabel.Size = new System.Drawing.Size(0, 13);
            this.songLabel.TabIndex = 1;
            // 
            // _labelTime
            // 
            this._labelTime.AutoSize = true;
            this._labelTime.Location = new System.Drawing.Point(12, 32);
            this._labelTime.Name = "_labelTime";
            this._labelTime.Size = new System.Drawing.Size(70, 13);
            this._labelTime.TabIndex = 2;
            this._labelTime.Text = "Current Time:";
            // 
            // trackTimeLabel
            // 
            this.trackTimeLabel.AutoSize = true;
            this.trackTimeLabel.Location = new System.Drawing.Point(93, 32);
            this.trackTimeLabel.Name = "trackTimeLabel";
            this.trackTimeLabel.Size = new System.Drawing.Size(10, 13);
            this.trackTimeLabel.TabIndex = 3;
            this.trackTimeLabel.Text = " ";
            // 
            // visualizer
            // 
            this.visualizer.Location = new System.Drawing.Point(12, 64);
            this.visualizer.Name = "visualizer";
            this.visualizer.Size = new System.Drawing.Size(440, 314);
            this.visualizer.TabIndex = 4;
            this.visualizer.TabStop = false;
            // 
            // beatLabel
            // 
            this.beatLabel.AutoSize = true;
            this.beatLabel.Location = new System.Drawing.Point(423, 32);
            this.beatLabel.Name = "beatLabel";
            this.beatLabel.Size = new System.Drawing.Size(10, 13);
            this.beatLabel.TabIndex = 5;
            this.beatLabel.Text = " ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 390);
            this.Controls.Add(this.beatLabel);
            this.Controls.Add(this.visualizer);
            this.Controls.Add(this.trackTimeLabel);
            this.Controls.Add(this._labelTime);
            this.Controls.Add(this.songLabel);
            this.Controls.Add(this._labelSong);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.visualizer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _labelSong;
        private System.Windows.Forms.Label songLabel;
        private System.Windows.Forms.Label _labelTime;
        private System.Windows.Forms.Label trackTimeLabel;
        private System.Windows.Forms.PictureBox visualizer;
        private System.Windows.Forms.Label beatLabel;

    }
}


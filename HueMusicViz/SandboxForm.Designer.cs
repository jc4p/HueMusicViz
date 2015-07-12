namespace HueMusicViz
{
    partial class SandboxForm
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
            this.trackBarR = new System.Windows.Forms.TrackBar();
            this.trackBarG = new System.Windows.Forms.TrackBar();
            this.trackBarB = new System.Windows.Forms.TrackBar();
            this.labelG = new System.Windows.Forms.Label();
            this.labelR = new System.Windows.Forms.Label();
            this.labelB = new System.Windows.Forms.Label();
            this.trackBarSaturation = new System.Windows.Forms.TrackBar();
            this.labelBrightness = new System.Windows.Forms.Label();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSaturation)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBarR
            // 
            this.trackBarR.Location = new System.Drawing.Point(48, 9);
            this.trackBarR.Maximum = 255;
            this.trackBarR.Name = "trackBarR";
            this.trackBarR.Size = new System.Drawing.Size(224, 45);
            this.trackBarR.TabIndex = 0;
            this.trackBarR.TickFrequency = 50;
            this.trackBarR.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // trackBarG
            // 
            this.trackBarG.Location = new System.Drawing.Point(48, 60);
            this.trackBarG.Maximum = 255;
            this.trackBarG.Name = "trackBarG";
            this.trackBarG.Size = new System.Drawing.Size(224, 45);
            this.trackBarG.TabIndex = 2;
            this.trackBarG.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // trackBarB
            // 
            this.trackBarB.Location = new System.Drawing.Point(48, 111);
            this.trackBarB.Maximum = 255;
            this.trackBarB.Name = "trackBarB";
            this.trackBarB.Size = new System.Drawing.Size(224, 45);
            this.trackBarB.TabIndex = 3;
            this.trackBarB.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // labelG
            // 
            this.labelG.AutoSize = true;
            this.labelG.ForeColor = System.Drawing.Color.Green;
            this.labelG.Location = new System.Drawing.Point(17, 60);
            this.labelG.Name = "labelG";
            this.labelG.Size = new System.Drawing.Size(13, 13);
            this.labelG.TabIndex = 4;
            this.labelG.Text = "0";
            // 
            // labelR
            // 
            this.labelR.AutoSize = true;
            this.labelR.ForeColor = System.Drawing.Color.Red;
            this.labelR.Location = new System.Drawing.Point(17, 9);
            this.labelR.Name = "labelR";
            this.labelR.Size = new System.Drawing.Size(13, 13);
            this.labelR.TabIndex = 1;
            this.labelR.Text = "0";
            // 
            // labelB
            // 
            this.labelB.AutoSize = true;
            this.labelB.ForeColor = System.Drawing.Color.Blue;
            this.labelB.Location = new System.Drawing.Point(17, 111);
            this.labelB.Name = "labelB";
            this.labelB.Size = new System.Drawing.Size(13, 13);
            this.labelB.TabIndex = 5;
            this.labelB.Text = "0";
            // 
            // trackBarSaturation
            // 
            this.trackBarSaturation.Location = new System.Drawing.Point(48, 163);
            this.trackBarSaturation.Maximum = 254;
            this.trackBarSaturation.Name = "trackBarSaturation";
            this.trackBarSaturation.Size = new System.Drawing.Size(224, 45);
            this.trackBarSaturation.TabIndex = 6;
            this.trackBarSaturation.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // labelBrightness
            // 
            this.labelBrightness.AutoSize = true;
            this.labelBrightness.Location = new System.Drawing.Point(17, 163);
            this.labelBrightness.Name = "labelBrightness";
            this.labelBrightness.Size = new System.Drawing.Size(13, 13);
            this.labelBrightness.TabIndex = 7;
            this.labelBrightness.Text = "0";
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.Location = new System.Drawing.Point(197, 207);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(75, 23);
            this.buttonUpdate.TabIndex = 8;
            this.buttonUpdate.Text = "Update";
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.updateLights);
            // SandboxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 242);
            this.Controls.Add(this.buttonUpdate);
            this.Controls.Add(this.labelBrightness);
            this.Controls.Add(this.trackBarSaturation);
            this.Controls.Add(this.labelB);
            this.Controls.Add(this.labelG);
            this.Controls.Add(this.trackBarB);
            this.Controls.Add(this.trackBarG);
            this.Controls.Add(this.labelR);
            this.Controls.Add(this.trackBarR);
            this.Name = "SandboxForm";
            this.Text = "SandboxForm";
            this.Load += new System.EventHandler(this.SandboxForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSaturation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBarR;
        private System.Windows.Forms.TrackBar trackBarG;
        private System.Windows.Forms.TrackBar trackBarB;
        private System.Windows.Forms.Label labelG;
        private System.Windows.Forms.Label labelR;
        private System.Windows.Forms.Label labelB;
        private System.Windows.Forms.TrackBar trackBarSaturation;
        private System.Windows.Forms.Label labelBrightness;
        private System.Windows.Forms.Button buttonUpdate;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
    }
}
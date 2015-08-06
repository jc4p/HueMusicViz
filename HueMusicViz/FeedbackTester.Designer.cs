namespace HueMusicViz
{
    partial class FeedbackTester
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
            this.buttonToggle = new System.Windows.Forms.Button();
            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonToggle
            // 
            this.buttonToggle.Location = new System.Drawing.Point(13, 13);
            this.buttonToggle.Name = "buttonToggle";
            this.buttonToggle.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.buttonToggle.Size = new System.Drawing.Size(259, 46);
            this.buttonToggle.TabIndex = 0;
            this.buttonToggle.TabStop = false;
            this.buttonToggle.Text = "Toggle Lights";
            this.buttonToggle.UseVisualStyleBackColor = true;
            this.buttonToggle.Click += new System.EventHandler(this.buttonToggle_Click);
            // 
            // textBox
            // 
            this.textBox.Enabled = false;
            this.textBox.Location = new System.Drawing.Point(13, 66);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(259, 183);
            this.textBox.TabIndex = 1;
            // 
            // FeedbackTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.buttonToggle);
            this.KeyPreview = true;
            this.Name = "FeedbackTester";
            this.Text = "FeedbackTester";
            this.Load += new System.EventHandler(this.FeedbackTester_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FeedbackTester_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonToggle;
        private System.Windows.Forms.TextBox textBox;
    }
}
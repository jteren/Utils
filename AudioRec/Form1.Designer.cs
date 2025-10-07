namespace AudioRec
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            btnStop = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(36, 36);
            button1.Name = "button1";
            button1.Size = new Size(228, 105);
            button1.TabIndex = 0;
            button1.Text = "start";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(270, 36);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(228, 105);
            btnStop.TabIndex = 1;
            btnStop.Text = "stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(535, 178);
            Controls.Add(btnStop);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Recorder";
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button btnStop;
    }
}

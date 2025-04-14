
namespace ImageResolutionTool
{
    partial class Form1
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
            this.buttonSelectDirectory = new System.Windows.Forms.Button();
            this.labelSelectDirectory = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.buttonRun = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSelectDirectory
            // 
            this.buttonSelectDirectory.Location = new System.Drawing.Point(642, 75);
            this.buttonSelectDirectory.Name = "buttonSelectDirectory";
            this.buttonSelectDirectory.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectDirectory.TabIndex = 0;
            this.buttonSelectDirectory.Text = "...";
            this.buttonSelectDirectory.UseVisualStyleBackColor = true;
            this.buttonSelectDirectory.Click += new System.EventHandler(this.buttonSelectDirectory_Click);
            // 
            // labelSelectDirectory
            // 
            this.labelSelectDirectory.AutoSize = true;
            this.labelSelectDirectory.Location = new System.Drawing.Point(60, 76);
            this.labelSelectDirectory.Name = "labelSelectDirectory";
            this.labelSelectDirectory.Size = new System.Drawing.Size(116, 17);
            this.labelSelectDirectory.TabIndex = 1;
            this.labelSelectDirectory.Text = "Select Directory: ";
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(183, 76);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(453, 22);
            this.textBoxPath.TabIndex = 2;
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(642, 128);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 3;
            this.buttonRun.Text = "Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.labelSelectDirectory);
            this.Controls.Add(this.buttonSelectDirectory);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSelectDirectory;
        private System.Windows.Forms.Label labelSelectDirectory;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Button buttonRun;
    }
}


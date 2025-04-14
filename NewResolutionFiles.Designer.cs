
namespace ImageResolutionTool
{
    partial class NewResolutionFiles
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
            this.listBoxNewResFiles = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBoxNewResFiles
            // 
            this.listBoxNewResFiles.FormattingEnabled = true;
            this.listBoxNewResFiles.ItemHeight = 16;
            this.listBoxNewResFiles.Location = new System.Drawing.Point(12, 27);
            this.listBoxNewResFiles.Name = "listBoxNewResFiles";
            this.listBoxNewResFiles.Size = new System.Drawing.Size(776, 388);
            this.listBoxNewResFiles.TabIndex = 0;
            // 
            // NewResolutionFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listBoxNewResFiles);
            this.Name = "NewResolutionFiles";
            this.Text = "New Resolution Files";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListBox listBoxNewResFiles;
    }
}
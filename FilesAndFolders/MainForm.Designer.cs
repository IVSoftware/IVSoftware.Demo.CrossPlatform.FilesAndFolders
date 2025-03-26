namespace FilesAndFolders
{
    partial class MainForm
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
            FileCollectionView = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // FileCollectionView
            // 
            FileCollectionView.AutoScroll = true;
            FileCollectionView.BackColor = Color.AliceBlue;
            FileCollectionView.Dock = DockStyle.Fill;
            FileCollectionView.Location = new Point(2, 2);
            FileCollectionView.Name = "FileCollectionView";
            FileCollectionView.Size = new Size(474, 740);
            FileCollectionView.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.RoyalBlue;
            ClientSize = new Size(478, 744);
            Controls.Add(FileCollectionView);
            Name = "MainForm";
            Padding = new Padding(2);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Main Form";
            ResumeLayout(false);
        }

        #endregion
        private FlowLayoutPanel FileCollectionView;
    }
}

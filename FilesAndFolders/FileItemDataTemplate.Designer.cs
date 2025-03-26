namespace FilesAndFolders
{
    partial class FileItemDataTemplate
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Grid = new TableLayoutPanel();
            Spacer = new Label();
            PlusMinus = new Label();
            TextLabel = new Label();
            Grid.SuspendLayout();
            SuspendLayout();
            // 
            // Grid
            // 
            Grid.ColumnCount = 3;
            Grid.ColumnStyles.Add(new ColumnStyle());
            Grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            Grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Grid.Controls.Add(Spacer, 0, 0);
            Grid.Controls.Add(PlusMinus, 1, 0);
            Grid.Controls.Add(TextLabel, 2, 0);
            Grid.Dock = DockStyle.Fill;
            Grid.Location = new Point(0, 0);
            Grid.Margin = new Padding(0);
            Grid.Name = "Grid";
            Grid.RowCount = 1;
            Grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Grid.Size = new Size(443, 54);
            Grid.TabIndex = 0;
            // 
            // Spacer
            // 
            Spacer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Spacer.Location = new Point(0, 0);
            Spacer.Margin = new Padding(0);
            Spacer.Name = "Spacer";
            Spacer.Size = new Size(60, 54);
            Spacer.TabIndex = 0;
            // 
            // PlusMinus
            // 
            PlusMinus.Dock = DockStyle.Fill;
            PlusMinus.Font = new Font("Segoe UI", 14F);
            PlusMinus.ForeColor = Color.Black;
            PlusMinus.Location = new Point(60, 0);
            PlusMinus.Margin = new Padding(0);
            PlusMinus.Name = "PlusMinus";
            PlusMinus.Size = new Size(60, 54);
            PlusMinus.TabIndex = 0;
            PlusMinus.Text = "+";
            PlusMinus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // TextLabel
            // 
            TextLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TextLabel.Location = new Point(120, 0);
            TextLabel.Margin = new Padding(0);
            TextLabel.Name = "TextLabel";
            TextLabel.Size = new Size(323, 54);
            TextLabel.TabIndex = 0;
            TextLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FileItemDataTemplate
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(Grid);
            Margin = new Padding(0);
            Name = "FileItemDataTemplate";
            Size = new Size(443, 54);
            Grid.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel Grid;
        private Label Spacer;
        private Label PlusMinus;
        private Label Text;
        private Label TextLabel;
    }
}

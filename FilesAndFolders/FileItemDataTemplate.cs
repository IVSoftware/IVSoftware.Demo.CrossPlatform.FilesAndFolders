using FilesAndFolders.Portable;
using FilesAndFolders.WinForms;
using System.Xml.Serialization;

namespace FilesAndFolders
{
    public partial class FileItemDataTemplate : UserControl
    {
        public FileItemDataTemplate()
        {
            InitializeComponent();
            Height = 50;
            PlusMinus.Click += (sender, e)=>
            {
                Control? current = this.Parent; 
                while (current is not null)
                {
                    if(current.DataContext is MainPageViewModel vm)
                    {
                        vm.PlusMinusToggleCommand?.Execute(DataContext);
                        break;
                    }
                    current = current.Parent;
                }
            };
        }
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent?.TopLevelControl is Control form)
            {
                form.SizeChanged -= localOnSizeChanged;
                form.SizeChanged += localOnSizeChanged;
            }
            void localOnSizeChanged(object? sender, EventArgs e)
            {
                var MARGIN = new Padding(2);
                Margin = MARGIN;
                Width =
                    Parent.Width - Parent.Padding.Horizontal
                        - SystemInformation.VerticalScrollBarWidth - MARGIN.Horizontal;
            }
        }
        public FontFamily FileAndFolderFontFamily
        {
            get
            {
                if (_fileAndFolderFontFamily is null)
                {
                    _fileAndFolderFontFamily = "file-and-folder-icons".LoadFamilyFromEmbeddedFont();
                }
                return _fileAndFolderFontFamily;
            }
        }
        FontFamily? _fileAndFolderFontFamily = null;
        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is not null)
            {
                Spacer.DataBindings.Clear();
                PlusMinus.DataBindings.Clear();
                TextLabel.DataBindings.Clear();

                Spacer.DataBindings.Add(nameof(Spacer.Width), DataContext, nameof(DataContext.Space), false, DataSourceUpdateMode.OnPropertyChanged);
                PlusMinus.Font = new Font(FileAndFolderFontFamily, 16F);
                PlusMinus.UseCompatibleTextRendering = true;
                PlusMinus.DataBindings.Add(nameof(PlusMinus.Text), DataContext, nameof(DataContext.PlusMinusGlyph), false, DataSourceUpdateMode.OnPropertyChanged);
                TextLabel.DataBindings.Add(nameof(TextLabel.Text), DataContext, nameof(DataContext.Text), false, DataSourceUpdateMode.OnPropertyChanged);
            }
        }
        public new FileItem? DataContext
        {
            get => (FileItem?)base.DataContext;
            set => base.DataContext = value;
        }
    }
}

using IVSoftware.Portable.Xml.Linq.XBoundObject;
using IVSoftware.Portable.Xml.Linq.XBoundObject.Placement;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml.Linq;

namespace BasicPlacement.Maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            string path =
                @"C:\Github\IVSoftware\Demo\IVSoftware.Demo.CrossPlatform.FilesAndFolders\BasicPlacement.Maui\BasicPlacement.Maui.csproj"
                .Replace('\\', Path.DirectorySeparatorChar);
            if (PlacerResult.Created == BindingContext.XRoot.Place(path, out XElement newXel))
            {
                foreach (var xel in BindingContext.XRoot.Descendants())
                {
                    BindingContext.FileItems.Add(new FileItem(xel)
                    {
                        Text = xel.Attribute("text")?.Value ?? "Error",
                        PlusMinus = 
                            ReferenceEquals(xel, newXel)
                            ? PlusMinus.Leaf
                            : PlusMinus.Expanded,
                        Space = 10 * xel.Ancestors().Skip(1).Count(),
                    });
                }
            }
        }
        new MainPageViewModel BindingContext => (MainPageViewModel)base.BindingContext;
    }
    class MainPageViewModel
    {
        public XElement XRoot { get; } = new("root");
        public ObservableCollection<FileItem> FileItems { get; } = new();
    }
    class FileItem :  XBoundViewObjectImplementer
    {
        public FileItem(XElement xel) : base (xel) { }

        public string Text
        {
            get => _text;
            set
            {
                if (!Equals(_text, value))
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }
        string _text = string.Empty;

        public string PlusMinusGlyph
        {
            get
            {
                switch (PlusMinus)
                {
                    case PlusMinus.Collapsed:
                        return "\uE803";
                    case PlusMinus.Partial:
                    case PlusMinus.Expanded:
                        return "\uE804";
                    default:
                        return "\uE805";
                }
            }
        }

        public int Space
        {
            get => _space;
            set
            {
                if (!Equals(_space, value))
                {
                    _space = value;
                    OnPropertyChanged();
                }
            }
        }

        int _space = default;

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            switch (propertyName)
            {
                case nameof(PlusMinus):
                    OnPropertyChanged(nameof(PlusMinusGlyph));
                    break;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

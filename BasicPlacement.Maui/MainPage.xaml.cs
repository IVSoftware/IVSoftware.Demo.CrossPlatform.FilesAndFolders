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
                            ? string.Empty
                            : "-",
                        Space = 10 * xel.Ancestors().Skip(1).Count(),
                    });
                }
            }
        }
        new MainPageViewModel BindingContext => (MainPageViewModel)base.BindingContext;
    }
    class MainPageViewModel : INotifyPropertyChanged
    {
        public XElement XRoot { get; } = new ("root");
        public ObservableCollection<FileItem> FileItems { get; } = new();
        public FileItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }
        FileItem? _selectedItem = null;
        public ICommand PlusMinusToggleCommand
        {
            get
            {
                if (_plusMinusToggleCommand is null)
                {
                    _plusMinusToggleCommand = new Command<FileItem>((fileItem) =>
                    {
                        switch (fileItem.PlusMinus)
                        {
                            case "+":
                                try
                                {
                                    IsBusy = true;
                                    var index = 0;
                                    fileItem.PlusMinus = "-";
                                    foreach (var xel in XRoot.VisibleElements())
                                    {
                                        var currentFileItem = xel.To<FileItem>();
                                        var currentIndex = FileItems.IndexOf(currentFileItem);
                                        if (index < FileItems.Count)
                                        {
                                            var current = FileItems[index];
                                            if (ReferenceEquals(current, currentFileItem))
                                            {   /* G T K */
                                                // Item exists at the correct index.
                                            }
                                            else
                                            {
                                                if (currentIndex == -1)
                                                {
                                                    FileItems.Insert(index, currentFileItem);
                                                }
                                                else
                                                {
                                                    Debug.Fail("First Time! Make sure this works.");
                                                    FileItems.Move(currentIndex, index);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            FileItems.Insert(index, currentFileItem);
                                        }
                                        index++;
                                    }
                                }
                                finally
                                {
                                    IsBusy = false;
                                }
                                break;
                            case "-":
                                foreach (var desc in fileItem.XEL.Descendants())
                                {
                                    if (desc.To<FileItem>() is { } remove)
                                    {
                                        FileItems.Remove(remove);
                                    }
                                }
                                fileItem.PlusMinus = "+";
                                break;
                            default:
                                Debug.Fail("Unexpected");
                                break;
                        }
                    });
                }
                return _plusMinusToggleCommand;
            }
        }
        ICommand? _plusMinusToggleCommand = null;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (!Equals(_isBusy, value))
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }
        bool _isBusy = default;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler? PropertyChanged;
    }
    class FileItem : INotifyPropertyChanged
    {
        public FileItem(XElement xel)
        {
            XEL = xel;
            xel.SetBoundAttributeValue(this);
        }
        public XElement XEL { get; }

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

        public string PlusMinus
        {
            get => _plusMinus;
            set
            {
                if (!Equals(_plusMinus, value))
                {
                    _plusMinus = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PlusMinusGlyph));
                }
            }
        }
        string _plusMinus = "+";

        public string PlusMinusGlyph
        {
            get
            {
                switch (PlusMinus)
                {
                    case "+":
                        return "\uE803";
                    case "-":
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

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler? PropertyChanged;
    }
    internal static class Extensions
    {
        public static IEnumerable<XElement> VisibleElements(this XElement @this)
        {
            Debug.Assert(@this.Name.LocalName == "root");
            foreach (var element in localAddChildItems(@this.Elements()))
            {
                yield return element;
            }
            IEnumerable<XElement> localAddChildItems(IEnumerable<XElement> elements)
            {
                foreach (var element in elements)
                {
                    if (element.To<FileItem>() is { } fileItem)
                    {
                        yield return element;
                        if (fileItem.PlusMinus == "-")
                        {
                            foreach (var childElement in localAddChildItems(element.Elements()))
                            {
                                yield return childElement;
                            }
                        }
                    }
                }
            }
        }
    }
}

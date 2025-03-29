using IVSoftware.Portable.Xml.Linq.XBoundObject;
using IVSoftware.Portable.Xml.Linq.XBoundObject.Placement;
using static IVSoftware.Portable.Threading.Extensions;
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

            BindingContext.XRoot.Show(path);
        }
        new MainPageViewModel BindingContext => (MainPageViewModel)base.BindingContext;
    }
    class MainPageViewModel
    {
        public MainPageViewModel() 
        {
            XRoot = new XElement("root").WithXBoundView(FileItems);
        }
        public XElement XRoot { get; }
        public ObservableCollection<FileItem> FileItems { get; } = new();

        public void SyncListToXML() => XRoot.To<ViewContext>().SyncList();
    }
    class FileItem :  XBoundViewObjectImplementer
    {
        public override string PlusMinusGlyph
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
    }
}

using FilesAndFolders.Portable;
using IVSoftware.Portable.Xml.Linq;
using IVSoftware.Portable.Xml.Linq.XBoundObject;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml.Linq;

namespace FilesAndFolders.Maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext.InitDrives();
        }
        new MainPageViewModel BindingContext => (MainPageViewModel)base.BindingContext!;
    }
}

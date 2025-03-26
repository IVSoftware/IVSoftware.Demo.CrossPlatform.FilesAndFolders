using FilesAndFolders.Portable;
using FilesAndFolders.WinForms;
using IVSoftware.Portable;
using IVSoftware.Portable.Xml.Linq.XBoundObject;
using System.Collections.Specialized;
using System.Diagnostics;

namespace FilesAndFolders
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            base.DataContext = new MainPageViewModel();
            Load += (sender, e) => DataContext.InitDrives();

            DataContext.FileItems.CollectionChanged += (sender, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        localAdd();
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        localRemove();
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        localAdd();
                        localRemove();
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        FileCollectionView.Controls.Clear();
                        break;
                    case NotifyCollectionChangedAction.Move:
                    default: throw new NotImplementedException("Unhandled collection change action.");
                }
                wdtCollectionChangeSettled.StartOrRestart();
                #region L o c a l F x       
                void localAdd()
                {
                    foreach (
                        var item in
                        e.NewItems
                        ?.OfType<FileItem>() 
                        ?? Enumerable.Empty<FileItem>())
                    {
                        FileCollectionView.Add(item);
                    }
                }

                void localRemove()
                {
                    foreach (
                    var item in e.OldItems
                        ?.OfType<FileItem>()
                        ?? Enumerable.Empty<FileItem>())
                    {
                        FileCollectionView.Hide(item);
                    }
                }		
                #endregion L o c a l F x
            };
        }
        new MainPageViewModel DataContext => (MainPageViewModel)base.DataContext!;

        SemaphoreSlim _reentrancy = new SemaphoreSlim(1, 1);
        public WatchdogTimer wdtCollectionChangeSettled
        {
            get
            {
                if (_wdtCollectionChangeSettled is null)
                {
                    _wdtCollectionChangeSettled = new WatchdogTimer { Interval = TimeSpan.FromMilliseconds(100) };
                    _wdtCollectionChangeSettled.RanToCompletion += async(sender, e) =>
                    {
                        await _reentrancy.WaitAsync();
                        BeginInvoke(() =>
                        {
                            try
                            {
                                int index = 0;
                                foreach (var xel in DataContext.XRoot.VisibleElements())
                                {
                                    if (
                                        xel.To<FileItem>() is { } fileItem &&
                                        fileItem.DataTemplate is Control control &&
                                        control.Parent is FlowLayoutPanel parent)
                                    {
                                        parent.Controls.SetChildIndex(control, index++);
                                        control.Show();
                                    }
                                }
                            }
                            finally
                            {
                                _reentrancy?.Release();
                            }
                        });
                    };
                }
                return _wdtCollectionChangeSettled;
            }
        }
        WatchdogTimer? _wdtCollectionChangeSettled = null;
    }
    static partial class Extensions
    {
        public static void Add(this FlowLayoutPanel @this, FileItem fileItem)
        {
            if (fileItem.DataTemplate is Control control)
            {
                if (control.Parent is FlowLayoutPanel)
                {
                    control.Show();
                    return;
                }
                else fileItem.DataTemplate = null;
            }
            var MARGIN = new Padding(2);
            fileItem.DataTemplate = new FileItemDataTemplate
            {
                AutoSize = false,
                Margin = MARGIN,
                Width =
                    @this.Width - @this.Padding.Horizontal
                    - SystemInformation.VerticalScrollBarWidth - MARGIN.Horizontal,
                DataContext = fileItem,
                Visible = false,
            };
            @this.Controls.Add((Control)fileItem.DataTemplate);
        }
        public static void Hide(this FlowLayoutPanel @this, FileItem fileItem)
            => (fileItem.DataTemplate as Control)?.Hide();

        public static void Remove(this FlowLayoutPanel @this, FileItem fileItem)
        {
            Debug.Fail("Expecting this method to remain unused. Has this changed?");
            if (fileItem.DataTemplate is Control control)
            {
                fileItem.DataTemplate = null;
                @this.Controls.Remove(control);
            }
        }
    }
}

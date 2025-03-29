using IVSoftware.Portable;
using IVSoftware.Portable.Xml.Linq.XBoundObject;
using IVSoftware.Portable.Xml.Linq.XBoundObject.Placement;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Windows.Input;
using System.Xml.Linq;
using static System.Environment;

namespace FilesAndFolders.Portable
{
    // <PackageReference Include = "IVSoftware.Portable.Xml.Linq.XBoundObject" Version="1.4.1-rc" />
    class MainPageViewModel : INotifyPropertyChanged
    {
#if DEBUG
        public MainPageViewModel()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Directory.GetDirectories(appData, "*", SearchOption.TopDirectoryOnly);
        }
#endif
        const int SPACE_FACTOR = 10;
        public XElement XRoot { get; } = new("root");
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

        /// <summary>
        /// Gets the command that toggles the expanded or collapsed state of a FileItem node,
        /// loading or unloading its child elements as needed.
        /// </summary>
        /// <remarks>
        /// When the item's PlusMinus is "+", this command expands the node by loading its children via LoadOnDemand.
        /// When it is "-", the command collapses the node and removes its descendants from FileItems.
        /// If the folder is empty or access is denied, the command performs no operation.
        /// </remarks>
        public ICommand PlusMinusToggleCommand
        {
            get
            {
                if (_plusMinusToggleCommand is null)
                {
                    _plusMinusToggleCommand = new Command<FileItem>(async(fileItem) =>
                    {
                        switch (fileItem?.PlusMinus)
                        {
                            case "+":
                                WDTBusy.StartOrRestart();
                                await Task.Delay(TimeSpan.FromMilliseconds(10));
                                fileItem.PlusMinus = "-";
                                if (!fileItem.XEL.IsAccessDenied())
                                {
                                    LoadOnDemand(fileItem.XEL.GetPath(NodeSortOrder.text));
                                }
                                Refresh();
                                break;
                            case "-":
                                WDTBusy.StartOrRestart();
                                await Task.Delay(TimeSpan.FromMilliseconds(10));
                                foreach (var desc in fileItem.XEL.Descendants())
                                {
                                    if (desc.To<FileItem>() is { } remove)
                                    {
                                        WDTBusy.StartOrRestart();
                                        FileItems.Remove(remove);
                                    }
                                }
                                fileItem.PlusMinus = "+";
                                break;
                            default:
                                if (fileItem?.IsEmptyFolder == true)
                                {   /* G T K */
                                    // N O O P
                                }
                                else if (fileItem?.NodeType == NodeType.file)
                                {   /* G T K */
                                    // N O O P
                                }
                                else
                                {
                                    Debug.Fail("Unexpected");
                                }
                                break;
                        }
                    });
                }
                return _plusMinusToggleCommand;
            }
        }

        /// <summary>
        /// Synchronizes the <see cref="FileItems"/> collection with the current state of visible elements in <see cref="XRoot"/>.
        /// </summary>
        /// <remarks>
        /// This method ensures the order and presence of items in <see cref="FileItems"/> match the structure defined by
        /// <c>XRoot.VisibleElements()</c>. Items are inserted, moved, or removed as needed, using object identity for comparison.
        /// </remarks>
        public void Refresh()
        {
            var index = 0;
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
            while (index < FileItems.Count)
            {
                FileItems.RemoveAt(index);
            }
        }

        /// <summary>
        /// Dynamically loads the contents of the specified directory into the internal structure,
        /// adding folder and file nodes only when needed.
        /// </summary>
        /// <param name="selectedPath">The directory path whose contents are to be loaded.</param>
        /// <remarks>
        /// This method ensures the provided path ends with a separator when necessary (e.g., for root drives like "D:").
        /// It avoids reentrancy using a semaphore and skips over protected system folders like "$Recycle.Bin".
        /// Folders and files are placed using <see cref="XRoot.Place(string, out XElement)"/>, and represented using <see cref="FileItem"/> instances.
        /// System files and unauthorized entries are ignored gracefully.
        /// </remarks>
        public void LoadOnDemand(string selectedPath)
        {
            if(!Directory.Exists(selectedPath))
            {
                Debug.Fail("Unexpected");
            }
            XElement newXel;
            string plusMinus;
            if (_lodReentrancy.Wait(0))
            {
                try
                {
                    string[] directories;
                    string[] files;
                    var builder = new List<string>();
                    var joined = string.Join(Environment.NewLine, builder);
                    // ============================================================
                    // Different! We're ADDING the path separator in this case!
                    // In particular, "D:" can be interpreted as current directory.
                    selectedPath = selectedPath.AddDirectorySeparatorAfterTrailingColon();
                    // ============================================================
                    directories = Directory.GetDirectories(selectedPath, "*", SearchOption.TopDirectoryOnly);
                    files = Directory.GetFiles(selectedPath);

                    foreach (var directory in
                        directories
                        .Select(_ => _.Trim(new char[] { Path.DirectorySeparatorChar })))
                    {
                        bool isEmptyFolder;
                        WDTBusy.StartOrRestart();
                        if (directory.EndsWith("$Recycle.Bin", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        if (directory.EndsWith("System Volume Information", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        try
                        {
                            isEmptyFolder = !FindAccessibleDescendants(directory, out string[] _, trySpecialFolders: false);
                            if (isEmptyFolder)
                            {
                                // No entries but no exception either. It's empty.
                                plusMinus = string.Empty;
                            }
                            else
                            {
                                plusMinus = "+";
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            continue;
                        }
                        catch
                        {
                            continue;
                        }
                        switch (XRoot.Place(directory, out newXel))
                        {
                            case PlacerResult.Created:
                                _ = new FileItem(newXel)
                                {
                                    Text = newXel.Attribute(nameof(NodeSortOrder.text))?.Value ?? "Folder ERR",
                                    Space = SPACE_FACTOR * newXel.Ancestors().Skip(1).Count(),
                                    PlusMinus = plusMinus,
                                    NodeType = NodeType.folder,
                                    IsEmptyFolder = isEmptyFolder,
                                };
                                break;
                            case PlacerResult.Exists:
                                break;
                            default:
                                Debug.Fail($"Unexpected");
                                break;
                        }
                    }
                    try
                    {
                        foreach (var file in files)
                        {
                            WDTBusy.StartOrRestart();
                            try
                            {
                                var attributes = File.GetAttributes(file);
                                if (attributes.HasFlag(FileAttributes.System))
                                {
                                    continue;
                                }
                            }
                            catch (UnauthorizedAccessException)
                            { continue; }
                            catch (IOException)
                            { continue; }
                            switch (XRoot.Place(file, out newXel))
                            {
                                case PlacerResult.Created:
                                    _ = new FileItem(newXel)
                                    {
                                        Text = newXel.Attribute(nameof(NodeSortOrder.text))?.Value ?? "Folder ERR",
                                        Space = SPACE_FACTOR * newXel.Ancestors().Skip(1).Count(),
                                        PlusMinus = string.Empty,
                                        NodeType = NodeType.file,
                                    };
                                    break;
                                case PlacerResult.Exists:
                                    break;
                                default:
                                    Debug.Fail($"Unexpected");
                                    break;
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Debug.Fail("");
                    }
                    catch
                    {
                        Debug.Fail("");
                    }
                }
                finally
                {
                    _lodReentrancy.Release();
                }
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
        bool _isBusy = false;

        private SemaphoreSlim _lodReentrancy = new SemaphoreSlim(1,1);

        public WatchdogTimer WDTBusy
        {
            get
            {
                if (_wdtBusy is null)
                {
                    _wdtBusy = new WatchdogTimer { Interval = TimeSpan.FromSeconds(0.5) };
                    _wdtBusy.PropertyChanged += (sender, e) =>
                    {
                        switch (e.PropertyName)
                        {
                            case nameof(WatchdogTimer.Running):
                                IsBusy = _wdtBusy.Running;
                                break;
                        }
                    };
                }
                return _wdtBusy;
            }
        }
        WatchdogTimer? _wdtBusy = null;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Initializes the application's file structure by scanning all logical drives on the system.
        /// For each accessible drive or descendant path, nodes are created and added to the internal data model.
        /// If a drive is inaccessible, a debug message is logged.
        /// </summary>
        /// <remarks>
        /// This method trims trailing directory separators from drive names, attempts to verify access
        /// to each drive or its accessible descendants, and constructs XML-based representations using
        /// the <see cref="Placer"/> class.
        /// Special handling is included for root directories on platforms like Android (e.g., "/").
        /// </remarks>
        public void InitDrives()
        {
            var drives =
                Directory
                .GetLogicalDrives()
                .Select(_=>_.Trim(new char[] { Path.DirectorySeparatorChar }))
                .ToArray();
            foreach (var drive in drives.Select(_=>_.AddDirectorySeparatorAfterTrailingColon()))
            {
                if (string.IsNullOrWhiteSpace(drive))
                {
                    var root = Path.DirectorySeparatorChar.ToString();
                    // For example, "/" on Android.
                    if (Directory.Exists(root))
                    {
                        try
                        {
                            _ = Directory.EnumerateFileSystemEntries(root).Any();
                            Debug.Fail("Expecting root access is forbidden! You should not be here.");
                        }
                        catch (UnauthorizedAccessException) { continue; }
                        catch { continue; }
                    }
                }
                if (FindAccessibleDescendants(drive, out string[] accessiblePaths, trySpecialFolders: true))
                {
                    foreach (var deepPath in accessiblePaths.Select(_ => _.Trim(new char[] { Path.DirectorySeparatorChar })))
                    {
                        switch (new Placer(
                            XRoot,
                            deepPath,
                            onBeforeAdd: (sender, e) =>
                            {
                                if (e.IsPathMatch)
                                {   /* G T K */
                                }
                                else
                                {
                                    if (!e.Xel.TryGetAccess())
                                    {
                                        e.Xel.SetAttributeValue(nameof(NodeSortOrder.accessdenied), true);
                                    }
                                }
                            }).PlacerResult)
                        {
                            case PlacerResult.Exists:
                            case PlacerResult.Created:
                                break;
                            default:
                                Debug.Fail("Expecting path was found or created.");
                                break;
                        }
                    }
                }
                else
                {   /* G T K */
                    // N O O P
                    // No accessible paths down this road.
                    Debug.WriteLineIf(true, $"ADVISORY: LOGICAL DRIVE INACCESSIBLE: {drive}");
                }
            }
            foreach (var child in XRoot.Elements())
            {
                FileItems.Add(localFileItemFactory(child));
                foreach(var desc in child.Descendants())
                {
                    // Do not add to file items
                    _ = localFileItemFactory(desc);
                }

                #region L o c a l F x
                FileItem localFileItemFactory(XElement xel)
                    => new FileItem(xel)
                    {
                        Text = xel.Attribute(nameof(NodeSortOrder.text))?.Value ?? "Error",
                        NodeType = NodeType.drive,
                        Space = SPACE_FACTOR * xel.Ancestors().Skip(1).Count(),
                        PlusMinus = "+"

                    };
                #endregion L o c a l F x
            }
#if DEBUG
            var actual = XRoot.SortAttributes<NodeSortOrder>().ToString();
            { }
#endif
        }

        /// <summary>
        /// Attempts to determine whether the specified directory or any of its accessible descendants contain
        /// files or subdirectories that can be accessed without throwing exceptions. If found, the method
        /// outputs the accessible paths.
        /// </summary>
        /// <param name="current">The root directory path to begin the search from.</param>
        /// <param name="accessiblePaths">
        /// — When the method returns true, this contains one or more paths to accessible directories or files.
        /// — When false, this is an empty array.</param>
        /// <param name="trySpecialFolders">If true, the method will also search well-known system folders (e.g., AppData)
        /// that start with the supplied argument when the initial search fails.</param>
        /// <returns>true if any accessible descendants or special folders are found; otherwise, false.</returns>
        bool FindAccessibleDescendants(string current, out string[] accessiblePaths, bool trySpecialFolders)
        {
            accessiblePaths = Array.Empty<string>();
            var builder = new List<string>();
            try
            {
                // We don't care if this is true of false. We just want to
                // know whether trying to do this throws an access exception.
                if (Directory.EnumerateFiles(current).Any())
                {
                    bool foundFile = false;
                    foreach (var file in Directory.EnumerateFiles(current))
                    {
                        try
                        {
                            if(File.GetAttributes(file).HasFlag(FileAttributes.System))
                            {
                                continue;
                            }
                            foundFile = true;
                            break;
                        }
                        catch (UnauthorizedAccessException) { }
                        catch { }
                    }
                    if (foundFile)
                    {   /* G T K */
                        // Found a folder with access.
                        accessiblePaths = [current];
                        return true;
                    }
                    else
                    {   /* G T K */
                        accessiblePaths = Array.Empty<string>();
                        return false;
                    }
                }
                else
                {
                    bool foundDirectory = false;
                    foreach (var childDirectory in Directory.EnumerateDirectories(current))
                    {
                        try
                        {
                            _ = Directory.EnumerateFileSystemEntries(childDirectory).Any();
                            foundDirectory = true;
                            break;
                        }
                        catch (UnauthorizedAccessException) { }
                        catch { }
                    }
                    if (foundDirectory)
                    {   /* G T K */
                        // Found a folder with access.
                        accessiblePaths = [current];
                        return true;
                    }
                    else
                    {   /* G T K */
                        accessiblePaths = Array.Empty<string>();
                        return false;
                    }
                }
            }
            catch (UnauthorizedAccessException) { }
            catch { }
            if (trySpecialFolders)
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                foreach (var specialFolder in Enum.GetValues<Environment.SpecialFolder>())
                {
                    var folderPath = Environment.GetFolderPath(specialFolder);
                    if (Directory.Exists(folderPath))
                    {
                        if (folderPath?.Trim(new char[] { Path.DirectorySeparatorChar }).StartsWith(current) == true)
                        {
                            try
                            {
                                if (FindAccessibleDescendants(folderPath, out string[] _, trySpecialFolders = false))
                                {
                                    builder.Add(folderPath);
                                }
                                else
                                {   /* G T K */
                                }
                            }
                            catch (UnauthorizedAccessException) { }
                            catch { }
                        }
                    }
                }
            }
            accessiblePaths = builder.ToArray();
            return accessiblePaths.Any();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

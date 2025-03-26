using IVSoftware.Portable.Xml.Linq.XBoundObject;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace FilesAndFolders.Portable
{
    [DebuggerDisplay("{Text}")]
    public class FileItem : INotifyPropertyChanged
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
                        if (NodeType == NodeType.file)
                        {
                            return "\uE805";
                        }
                        else
                        {
                            return "\uE803";
                        }
                }
            }
        }
        public bool IsEmptyFolder
        {
            get => _isEmptyFolder;
            set
            {
                if (!Equals(_isEmptyFolder, value))
                {
                    _isEmptyFolder = value;
                    OnPropertyChanged();
                }
            }
        }
        bool _isEmptyFolder = default;


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

        public NodeType NodeType
        {
            get => _nodeType;
            set
            {
                if (!Equals(_nodeType, value))
                {
                    _nodeType = value;
                    OnPropertyChanged();
                }
            }
        }
        NodeType _nodeType = default;


        /// <summary>
        /// Enable O(1) removal, mainly for WinForms use.
        /// </summary>
        public object? DataTemplate { get; internal set; }

        int _space = default;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

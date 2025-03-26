using IVSoftware.Portable.Xml.Linq.XBoundObject;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace FilesAndFolders.Portable
{
    static partial class Extensions
    {
        public static string GetPath(this XElement @this, Enum pathAttribute)
        {
            var builder = new List<string>();
            foreach (var anc in @this.AncestorsAndSelf().Reverse())
            {
                if (anc.Attribute(pathAttribute.ToString())?.Value is { } value)
                {
                    builder.Add(value);
                }
            }
            return Path.Combine(builder.ToArray());
        }

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
        public static bool TryGetAccess(this XElement @this)
        {
            try
            {
                _ = Directory.EnumerateFileSystemEntries(@this.GetPath(NodeSortOrder.text)).Any();
                return true;
            }
            catch (UnauthorizedAccessException) { }
            catch { }
            return false;
        }
        public static bool IsAccessDenied(this XElement @this) 
            => @this.Attribute(nameof(NodeSortOrder.accessdenied))?.Value.ToLower() == "true";

        // ============================================================
        // Different! We're ADDING the path separator in this case!
        // In particular, "D:" can be interpreted as current directory.
        public static string AddDirectorySeparatorAfterTrailingColon(this string @this)
            => @this.EndsWith(":")
                ? $@"{@this}\"
                : @this;
        // ============================================================                        
    }
}

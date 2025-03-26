using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesAndFolders.WinForms
{
    public static class Provider
    {
        public static FontFamily LoadFamilyFromEmbeddedFont(this string ttf)
        {
            var asm = typeof(Provider).Assembly;
#if DEBUG
            var names = asm.GetManifestResourceNames();
            { }
#endif
            var fontFamily = privateFontCollection.Families.FirstOrDefault(_ => _.Name.Equals(ttf));
            if (fontFamily == null)
            {
                var resourceName = asm.GetManifestResourceNames().FirstOrDefault(_ => _.Contains(ttf));
                if (string.IsNullOrWhiteSpace(resourceName))
                {
                    throw new InvalidOperationException("Expecting font file is embedded resource.");
                }
                else
                {
                    // Get the embedded font resource stream
                    using (Stream fontStream = asm.GetManifestResourceStream(resourceName)!)
                    {
                        if (fontStream == null)
                        {
                            throw new InvalidOperationException($"Font resource '{resourceName}' not found.");
                        }
                        else
                        {
                            // Load the font into the PrivateFontCollection
                            byte[] fontData = new byte[fontStream.Length];
                            fontStream.Read(fontData, 0, (int)fontStream.Length);

                            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
                            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                            privateFontCollection.AddMemoryFont(fontPtr, fontData.Length);

                            fontFamily = privateFontCollection.Families.First(_ => _.Name.Equals(ttf));
                        }
                    }
                }
            }
            else
            {   /* G T K */
                // Already loaded
            }
            return fontFamily;
        }
        public static void Dispose() => privateFontCollection?.Dispose();
        private static PrivateFontCollection privateFontCollection { get; } = new PrivateFontCollection();
    }
}

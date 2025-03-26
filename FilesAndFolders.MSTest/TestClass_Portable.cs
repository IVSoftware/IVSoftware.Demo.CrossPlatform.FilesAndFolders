
using IVSoftware.Portable.Xml.Linq.XBoundObject.Placement;
using IVSoftware.WinOS.MSTest.Extensions;
using System.Diagnostics;
using System.Xml.Linq;

namespace FilesAndFolders.MSTest
{
    [TestClass]
    public sealed class TestClass_Portable
    {
        [TestMethod]
        // <PackageReference Include = "IVSoftware.Portable.Xml.Linq.XBoundObject" Version="1.4.1-rc" />
        public void Test_BasicPlace()
        {
            string actual, expected;


            XElement xroot = new XElement("root");
            string path = @"C:\files-and-folders\FilesAndFolders\FilesAndFolders.csproj";
            xroot.Place(path);
            actual = xroot.ToString();

            actual.ToClipboard();
            actual.ToClipboardAssert();
            { }
            expected = @" 
<root>
  <xnode text=""C:"">
    <xnode text=""files-and-folders"">
      <xnode text=""FilesAndFolders"">
        <xnode text=""FilesAndFolders.csproj"" />
      </xnode>
    </xnode>
  </xnode>
</root>";

            Debug.Assert(string.Equals(
                expected.Trim(),
                actual.Trim()),
                "Expecting values to match."
            );
        }
    }
}

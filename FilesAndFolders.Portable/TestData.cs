
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FilesAndFolders.Winforms")]
[assembly: InternalsVisibleTo("FilesAndFolders.Maui")]
namespace FilesAndFolders.Portable
{
    class TestData
    {
        public static string FILES { get; } = @"
D:\files-and-folders\.gitattributes
D:\files-and-folders\.gitignore
D:\files-and-folders\files-and-folders.sln
D:\files-and-folders\LICENSE.txt
D:\files-and-folders\README.md
D:\files-and-folders\.git\config
D:\files-and-folders\.git\description
D:\files-and-folders\.git\HEAD
D:\files-and-folders\.git\ms-persist.xml
D:\files-and-folders\FilesAndFolders\FilesAndFolders.csproj
D:\files-and-folders\FilesAndFolders\FilesAndFolders.csproj.user
D:\files-and-folders\FilesAndFolders\MainForm.cs
D:\files-and-folders\FilesAndFolders\MainForm.Designer.cs
D:\files-and-folders\FilesAndFolders\MainForm.resx
D:\files-and-folders\FilesAndFolders\Placer.cs
D:\files-and-folders\FilesAndFolders\Program.cs
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.deps.json
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.dll
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.exe
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.pdb
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.runtimeconfig.json
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\IVSoftware.Portable.Threading.dll
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\IVSoftware.Portable.Xml.Linq.XBoundObject.dll
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\.NETCoreApp,Version=v8.0.AssemblyAttributes.cs
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\apphost.exe
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAnd.77B0DA9F.Up2Date
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.AssemblyInfo.cs
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.AssemblyInfoInputs.cache
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.assets.cache
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.AssemblyReference.cache
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.BuildWithSkipAnalyzers
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.CoreCompileInputs.cache
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.FileListAbsolute.txt
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.GenerateResource.cache
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.designer.deps.json
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.designer.runtimeconfig.json
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.dll
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.GeneratedMSBuildEditorConfig.editorconfig
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.genruntimeconfig.cache
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.GlobalUsings.g.cs
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.MainForm.resources
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.pdb
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\ref\FilesAndFolders.dll
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\refint\FilesAndFolders.dll".Replace('\\', Path.DirectorySeparatorChar).Trim();
    }
}

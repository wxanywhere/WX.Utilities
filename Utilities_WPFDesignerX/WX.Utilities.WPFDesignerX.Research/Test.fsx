
#r ""

open System


EnvDTE80.DTE2 dte2;
dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.
GetActiveObject("VisualStudio.DTE.11.0");

__LINE__

__SOURCE_DIRECTORY__

__SOURCE_FILE__

__PROJECT_DIRECTORY__

System.IO.Directory.GetCurrentDirectory()

System.Windows.Forms.Application.StartupPath

System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)

System.Environment.GetFolderPath(System.Environment.SpecialFolder.Programs)


Application.Info.DirectoryPath

System.AppDomain.CurrentDomain.GetAssemblies()
|>Seq.filter(fun a->a.CodeBase.EndsWith(".exe"))
|>Seq.map(fun a->System.IO.Path.GetDirectoryName(a.CodeBase.Replace("file:///", "")))
|>Seq.toArray

System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
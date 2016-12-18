
#I @"D:\Workspace\WX\Dev\WPFDesignerX\ReferenceDll"
#r "envdte.dll"
#r "envdte80.dll"
#r "envdte100.dll"
#r "Microsoft.VisualStudio.Shell.11.0.dll"
#r "Microsoft.VisualStudio.Shell.Interop.dll"
#r "Microsoft.VisualStudio.Shell.Interop.8.0.dll"
#r "Microsoft.VisualStudio.Shell.Interop.9.0.dll"
#r "Microsoft.VisualStudio.OLE.Interop.dll"

//#r "Microsoft.VisualStudio.Shell.Design"

//typeof<>

System.Environment.CurrentDirectory
System.IO.Directory.GetCurrentDirectory() 
System.Environment.GetEnvironmentVariables()
System.Environment.CurrentManagedThreadId
System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)
System.Environment.

open Microsoft.VisualStudio.Shell
//open Microsoft.VisualStudio.Shell.Design
//open Microsoft.VisualStudio.OLE.Interop

let x01=Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider
x01.GetService(typeof<EnvDTE80.DTE2>)

let dte2 = System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE"):?>EnvDTE80.DTE2

dte2.MainWindow.ToString()

let dte10 = System.Runtime.InteropServices.Marshal.GetActiveObject("VPDExpress.DTE.11.0"):?>EnvDTE80.DTE2

let dte=dte2:?>EnvDTE.DTE
//Microsoft.VisualStudio.OLE.Interop. IServiceProvider
//let x00=new ServiceProvider(

let x05=Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof<Microsoft.VisualStudio.Shell.Interop.SDTE>)

let x01=Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof<EnvDTE80.DTE2>)

let x02=Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof<EnvDTE.DTE>)

let instance=Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof<EnvDTE80.DTE2>):?>EnvDTE80.DTE2

System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase

dte2.ActiveDocument.ProjectItem.ContainingProject.FileName

for n in dte2.Documents do
  printfn "%A" n.ProjectItem.ContainingProject.FileName

System.AppDomain.CurrentDomain.InitializeLifetimeService()
System.AppDomain.CurrentDomain.GetLifetimeService()

System.Diagnostics.Process.GetCurrentProcess().Id

System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName

System.Runtime.InteropServices.Marshal.GenerateProgIdForType(typeof<EnvDTE.DTE>)
System.Runtime.InteropServices.

dte.FullName

dte

let provider=new ServiceProvider(dte:?>IServiceProvider)

let typeService = provider.GetService<DynamicTypeService>(
Debug.Assert(typeService != null, "No dynamic type service registered.");

IVsSolution sln = GetService<IVsSolution>();
IVsHierarchy hier;
sln.GetProjectOfUniqueName(CurrentProject.Project.UniqueName, out hier);

Debug.Assert(hier != null, "No active hierarchy is selected.");

return typeService.GetTypeResolutionService(hier);

(*
http://stackoverflow.com/questions/10864595/getting-the-current-envdte-or-iserviceprovider-when-not-coding-an-addin
http://stackoverflow.com/questions/4724381/get-the-reference-of-the-dte2-object-in-visual-c-sharp-2010/4724924#4724924
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE80;

[DllImport("ole32.dll")]
private static extern void CreateBindCtx(int reserved, out IBindCtx ppbc);
[DllImport("ole32.dll")]
private static extern void GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

  internal static DTE2 GetCurrent()
  {

     //rot entry for visual studio running under current process.
     string rotEntry = String.Format("!VisualStudio.DTE.10.0:{0}", Process.GetCurrentProcess().Id);
     IRunningObjectTable rot;
     GetRunningObjectTable(0, out rot);
     IEnumMoniker enumMoniker;
     rot.EnumRunning(out enumMoniker);
     enumMoniker.Reset();
     IntPtr fetched = IntPtr.Zero;
     IMoniker[] moniker = new IMoniker[1];
     while (enumMoniker.Next(1, moniker, fetched) == 0)
     {
        IBindCtx bindCtx;
        CreateBindCtx(0, out bindCtx);
        string displayName;
        moniker[0].GetDisplayName(bindCtx, null, out displayName);
        if (displayName == rotEntry)
        {
           object comObject;
           rot.GetObject(moniker[0], out comObject);
           return (EnvDTE80.DTE2)comObject;
        }
     }
     return null;
  }

  And at the point that I want to access the current IDE:
var dte = CurrentIde.GetCurrent();
var sol = dte.Solution;

But remember.... This code will NOT work during debugging!!! The line of code starting with string rotEntry... has a call to the Process.GetCurrentProcess to get the ID of the current process.

While debugging some functionality in my addin (using MME http://mme.codeplex.com/) I call a method that needs the current IDE. I test this with a ConsoleApp that calls the addin method. At the point of getting the current IDE, the currentprocess is NOT the IDE, but the ConsoleApp.vshost.exe. So my code did not work during debugging, but DID work after building the addin and installing this addin.
*)
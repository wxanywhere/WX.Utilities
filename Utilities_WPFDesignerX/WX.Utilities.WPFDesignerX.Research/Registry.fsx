open System
open System.IO
open Microsoft.Win32

let progidInfo=
  use regClis = Registry.ClassesRoot.OpenSubKey("CLSID")
  seq{
    for clsid in regClis.GetSubKeyNames() do
        let regClsidKey = regClis.OpenSubKey(clsid)
        let ProgID = regClsidKey.OpenSubKey("ProgID")
        let regPath =ref (regClsidKey.OpenSubKey("InprocServer32"))
        if !regPath = null then
            regPath:= regClsidKey.OpenSubKey("LocalServer32")
      
        if (!regPath) <> null && ProgID <> null then
            let pid = ProgID.GetValue("")
            let filePath = (!regPath).GetValue("")
            yield string pid,string filePath
            (!regPath).Close()
        regClsidKey.Close()
  }
  |>Seq.sortBy (fun (a,_)->a)
  |>Seq.toArray

progidInfo
//|>Seq.filter (fun (_,a)->a.ToLower().Contains("blend")) 
|>Seq.filter (fun (_,a)->a.ToLower().Contains("devenv")) 
|>Seq.filter (fun (_,a)->a.ToLower().Contains("xdesproc"))
//|>Seq.filter (fun (a,_)->a.ToLower().Contains("dte")) 
//|>Seq.filter (fun (_,a)->a.ToLower().Contains("office")) 
|>Seq.iter (fun (a,b)->printfn "%A" (a,b))


let regClis = Registry.ClassesRoot.OpenSubKey("CLSID")
regClis.GetSubKeyNames() 
(*
Get All ProgID on System for COM Automation
http://procbits.com/2010/11/08/get-all-progid-on-system-for-com-automation
var regClis = Registry.ClassesRoot.OpenSubKey("CLSID");
var progs = new List<string>();

foreach (var clsid in regClis.GetSubKeyNames()) {
    var regClsidKey = regClis.OpenSubKey(clsid);
    var ProgID = regClsidKey.OpenSubKey("ProgID");
    var regPath = regClsidKey.OpenSubKey("InprocServer32");

    if (regPath == null)
        regPath = regClsidKey.OpenSubKey("LocalServer32");

    if (regPath != null && ProgID != null) {
        var pid = ProgID.GetValue("");
        var filePath = regPath.GetValue("");
        progs.Add(pid + " -> " + filePath);
        regPath.Close();
    }

    regClsidKey.Close();
}

regClis.Close();

progs.Sort();

var sw = new StreamWriter(@"c:\ProgIDs.txt");
foreach (var line in progs)
    sw.WriteLine(line);

sw.Close();
*)
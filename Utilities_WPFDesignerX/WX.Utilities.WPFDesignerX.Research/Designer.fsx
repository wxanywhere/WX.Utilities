
#I @"c:\Windows\Assembly\GAC\"
#r "Microsoft.Windows.Design.Extensibility"
#r "Microsoft.Windows.Design.Interaction"
open Microsoft.Windows.Design

type EditingContextX()=
  inherit EditingContext.CreateServiceManager()
  override this.CreateServiceManager()

let x01=new Microsoft.Windows.Design.EditingContext()
x01.Services

let x0=new Microsoft.Windows.Design.ServiceManager

Microsoft.Windows.Design.ServiceManager



#r "FSharp.Data.TypeProviders"
#r "System.ServiceModel"
#r "System.Runtime.Serialization"

open Microsoft.FSharp.Data.TypeProviders

type TerraService = WsdlService<"http://msrmaps.com/TerraService2.asmx?WSDL">
let r=TerraService.GetTerraServiceSoap()
r.GetTile(TerraService.ServiceTypes.msrmaps.com.TileId())
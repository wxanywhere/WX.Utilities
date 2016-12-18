namespace WX

[<AutoOpen>]
module EnumModule=
  type EditStatus=
    | Add=0
    | Modify=1
    | Delete=2

  type ChangedType=
    | Unchanged=0
    | Added=1
    | Modified=2
    | Deleted=3
    | Dirtied = 4 //备用
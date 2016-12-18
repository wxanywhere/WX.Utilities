using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WX.Utilities.WPFDesignerX.Office.Word
{
    public class WordServiceInfo
    {
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string ServiceCode { get; set; }
        public int ReferenceCount { get; set; }
        public ChangedType ServiceChangedType { get; set; }
        public ChangedType ServiceDescriptionChangedType { get; set; }
        public ChangedType ServiceCodeChangedType { get; set; }
        public ChangedType ReferenceCountChangedType { get; set; }
    }
}

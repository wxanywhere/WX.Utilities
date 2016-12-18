using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WX.Utilities.WPFDesignerX.Office.Word
{
    public class WordUIInfo
    {
        public string UITypeName { get; set; }
        public string UIName { get; set; }
        public bool IsCommonUI { get; set; }
        public bool IsMainUI { get; set; }
        public bool IsAnnotated { get; set; }
        public ChangedType AnnotationChangedType { get; set; }
        public ChangedType UIChangedType { get; set; }
    }
}

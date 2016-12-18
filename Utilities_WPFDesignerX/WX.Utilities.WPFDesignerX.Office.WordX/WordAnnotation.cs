using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WX.Utilities.WPFDesignerX.Office.Word
{
    public class WordAnnotation
    {
        public int SerialNumber { get; set; }
        public Guid UIElementID { get; set; }
        public string UIElementName { get; set; }
        public string UITypeName { get; set; }
        public string UIName { get; set; }
        public bool IsInTabItem { get; set; }
        public bool IsTabItem { get; set; } 
        public int TabControlNumber { get; set; }
        public int TabItemNumber { get; set; }
        public string TabItemHeader { get; set; }
        public string RequirementDescription { get; set; }
        public string BehaviorDescription { get; set; }
        public WordServiceInfo[] WordServiceInfos { get; set; }
        public ChangedType UIElementNameChangedType { get; set; }
        public ChangedType RequirementDescriptionChangedType { get; set; }
        public ChangedType BehaviorDescriptionChangedType { get; set; }
        public ChangedType ServiceInfosChangedType { get; set; }
        public ChangedType AnnotationChangedType { get; set; }
    }
}

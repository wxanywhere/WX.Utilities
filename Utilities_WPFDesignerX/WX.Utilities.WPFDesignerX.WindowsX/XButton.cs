using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WX.Utilities.WPFDesignerX.Windows
{
    public class XButton:Button
    {
        //[Editor("WX.Utilities.WPFDesignerX.Windows.ControlsX.ControlBusinessUITypeEditor,WX.Utilities.WPFDesignerX.Windows.ControlsX", typeof(UITypeEditor))] //Is right.
        //[Editor("WX.DataManage.Workflow.Activities.VOM_SQL_UITypeEditor,WX.DataManage.Workflow.Activities", "System.Drawing.Design.UITypeEditor,System.Drawing.Design")] //Is wrong.
        //[Editor("WX.DataManage.Workflow.Activities.VOM_SQL_UITypeEditor,WX.DataManage.Workflow.Activities", "System.Drawing.Design.UITypeEditor")] //Is right.
        //[Editor("WX.DataManage.Workflow.Activities.VOM_SQL_UITypeEditor", "System.Drawing.Design.UITypeEditor")] //Is right.
        [Editor(typeof(ControlBusinessUITypeEditor), typeof(UITypeEditor))]
        [Description("业务信息编辑")]
        [Category("WX")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Grid UIElementID
        {
            get { return (Grid)GetValue(UIElementIDProperty); }
            set { SetValue(UIElementIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UIElementID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UIElementIDProperty =
            DependencyProperty.Register("UIElementID", typeof(Guid), typeof(XButton), new PropertyMetadata(new Guid()));

        
    }
}

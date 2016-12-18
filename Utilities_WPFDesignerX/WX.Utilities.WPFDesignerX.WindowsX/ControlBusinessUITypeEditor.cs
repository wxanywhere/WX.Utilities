using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;

namespace WX.Utilities.WPFDesignerX.Windows
{
    public class ControlBusinessUITypeEditor:UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            //var editor = new UC_BusinessEditor();
            //editor.DataContext = new FVM_BusinessEditor();
            var form = new System.Windows.Forms.Form();
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return Guid.NewGuid();
            }
            else
            {
                return base.EditValue(context, provider, value);
            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext typeDescriptorContext)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}


/*
http://nonocast.cn/adorner-in-wpf-part-2/
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace WX.Utilities.WPFDesignerX.Windows
{
    public class ChromeAdorner : Adorner
    {
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public ChromeAdorner(UIElement AdornedElement)
            : base(AdornedElement)
        {
            this.IsHitTestVisible = true;

            this.chrome = new Button();
            this.AddVisualChild(chrome);
        }

        protected override void OnRender(DrawingContext dc)
        {
            Rect adornedElementDesiredRect = new Rect(this.AdornedElement.DesiredSize);
            dc.DrawRectangle(null, pen, adornedElementDesiredRect);
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.chrome;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        private Button chrome;
        private Pen pen = new Pen(Brushes.Red, 1.0);
    }
}

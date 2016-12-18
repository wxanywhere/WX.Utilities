/*

http://www.ageektrapped.com/blog/the-missing-net-4-cue-banner-in-wpf-i-mean-watermark-in-wpf/
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
    public class CueBannerAdorner : Adorner
    {
        private readonly ContentPresenter contentPresenter;

        public CueBannerAdorner(UIElement adornedElement, object cueBanner) :
            base(adornedElement)
        {
            this.IsHitTestVisible = false;

            contentPresenter = new ContentPresenter();
            contentPresenter.Content = cueBanner;
            contentPresenter.Opacity = 0.7;
            contentPresenter.Margin =
               new Thickness(Control.Margin.Left + Control.Padding.Left,
                             Control.Margin.Top + Control.Padding.Top, 0, 0);
        }

        public CueBannerAdorner(UIElement adornedElement) :
            base(adornedElement)
        {
            this.IsHitTestVisible = false;

            contentPresenter = new ContentPresenter();
            contentPresenter.Content = new Button() { Height=16,Width=16};
            contentPresenter.Opacity = 0.7;
            contentPresenter.Margin =
               new Thickness(Control.Margin.Left + Control.Padding.Left,
                             Control.Margin.Top + Control.Padding.Top, 0, 0);
        }

        private Control Control
        {
            get { return (Control)this.AdornedElement; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return contentPresenter;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            //here's the secret to getting the adorner
            //to cover the whole control
            contentPresenter.Measure(Control.RenderSize);
            return Control.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            contentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}

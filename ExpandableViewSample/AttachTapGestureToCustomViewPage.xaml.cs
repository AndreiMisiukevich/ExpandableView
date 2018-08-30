using System;
using System.Collections.Generic;
using Expandable;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ExpandableViewSample
{
    public partial class AttachTapGestureToCustomViewPage : ContentPage
    {
        public AttachTapGestureToCustomViewPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            expandableView.IsExpandChanged += IsExpandChangedHandler;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            expandableView.IsExpandChanged -= IsExpandChangedHandler;
        }

        private async void IsExpandChangedHandler(object sender, ExpandChangedEventArgs e)
        {
            var rotation = 180;
            if (!e.IsExpanded)
            {
                rotation = 0;
            }

            Debug.WriteLine(e.IsExpanded, "IsExpanded");

            await arrow.RotateTo(rotation, 250, Easing.BounceIn);
        }
    }
}

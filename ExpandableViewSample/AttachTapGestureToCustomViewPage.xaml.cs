using Expandable;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            expandableView.StatusChanged += OnStatusChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            expandableView.StatusChanged -= OnStatusChanged;
        }

        private async void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            var rotation = 180;
            if (e.Status == ExpandStatus.Collapsing)
            {
                rotation = 0;
            }

            await arrow.RotateTo(rotation, 250, Easing.BounceIn);
        }
    }
}

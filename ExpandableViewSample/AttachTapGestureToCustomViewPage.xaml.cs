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
            var rotation = -1;
            switch(e.Status)
            {
                case ExpandStatus.Collapsing:
                    rotation = 0;
                    break;
                case ExpandStatus.Expanding:
                    rotation = 180;
                    break;
                default:
                    return;
            }

            await arrow.RotateTo(rotation, 250, Easing.BounceIn);
        }
    }
}

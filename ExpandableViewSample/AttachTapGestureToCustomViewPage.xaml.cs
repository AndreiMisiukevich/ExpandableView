using Expandable;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Windows.Input;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ExpandableViewSample
{
    public partial class AttachTapGestureToCustomViewPage : ContentPage
    {
        public AttachTapGestureToCustomViewPage()
        {
            InitializeComponent();
        }

        private ICommand _tapCommand;
        public ICommand TapCommand => _tapCommand ?? (_tapCommand = new Command(p =>
        {
            DisplayAlert("Tapped", p.ToString(), "Ok");
        }));

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
            var rotation = 0;
            switch(e.Status)
            {
                case ExpandStatus.Collapsing:
                    break;
                case ExpandStatus.Expanding:
                    rotation = 180;
                    break;
                default:
                    return;
            }

            await arrow.RotateTo(rotation, 200, Easing.CubicInOut);
        }
    }
}

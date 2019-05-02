using Xamarin.Forms;
using Expandable;
namespace ExpandableViewSample
{
    public class NestedExpandablePage : ContentPage
    {
        public NestedExpandablePage()
        {
            var nestedExp = new ExpandableView
            {
                SecondaryViewHeightRequest = 200,
                PrimaryView = new Label { Text = "NESTED EXP 2", FontSize = 30 },
                SecondaryViewTemplate = new DataTemplate(() =>
                {
                    return new BoxView { Color = Color.Purple };
                })
            };

            var mainExp = new ExpandableView
            {
                BackgroundColor = Color.Green,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                PrimaryView = new Label { Text = "EXPANDABLE 1", FontSize = 40, FontAttributes = FontAttributes.Bold },
                SecondaryViewTemplate = new DataTemplate(() =>
                {
                    return new StackLayout
                    {
                        Children =
                        {
                            new BoxView { Color = Color.Black, HeightRequest = 150 },
                            nestedExp
                        }
                    };
                })
            };

            nestedExp.Command = new Command(() =>
            {
                mainExp.SecondaryView.HeightRequest = -1;
            });

            Content = new StackLayout
            {
                Children =
                {
                    mainExp
                }
            };
        }
    }
}

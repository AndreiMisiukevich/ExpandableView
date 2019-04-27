using System;
using Expandable;
using Xamarin.Forms;
namespace ExpandableViewSample
{
    public class ManyViewsPage : ContentPage
    {
        public ManyViewsPage()
        {
            var mainStack = new StackLayout();

            for (int i = 0; i < 15; ++i)
            {
                mainStack.Children.Add(CreateExpandable(i));
            }

            BackgroundColor = Color.White;
            Content = new ScrollView
            {
                Padding = new Thickness(20, Device.RuntimePlatform == Device.iOS ? 20 : 0, 20, 0),
                Content = mainStack
            };
        }

        private View CreateExpandable(int number)
        {
            var secondLabel = new Label
            {
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Italic,
                HeightRequest = 40,
                BackgroundColor = Color.Black,
                TextColor = Color.White,
                Text = $"The Label of {number}"
            };

            return new ExpandableView
            {
                PrimaryView = new Label
                {
                    FontSize = 22,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontAttributes = FontAttributes.Bold,
                    HeightRequest = 60,
                    BackgroundColor = Color.Black,
                    TextColor = Color.White,
                    Text = $"Click to expand {number}"
                },

                SecondaryViewTemplate = new DataTemplate(() => new StackLayout
                {
                    Spacing = 10,
                    Padding = new Thickness(20, 0),
                    Children = {
                        new Button
                        {
                            CornerRadius = 0,
                            FontAttributes = FontAttributes.Italic,
                            HeightRequest = 40,
                            BackgroundColor = Color.Black,
                            TextColor = Color.White,
                            Text = $"Increase height of label",
                            Command = new Command(() => ChangeHeight(secondLabel, 20))
                        },
                        secondLabel,
                        new Button
                        {
                            CornerRadius = 0,
                            FontAttributes = FontAttributes.Italic,
                            HeightRequest = 40,
                            BackgroundColor = Color.Black,
                            TextColor = Color.White,
                            Text = $"Decrease height of the label",
                            Command = new Command(() => ChangeHeight(secondLabel, -20))
                        }
                    }
                })
            };
        }

        private void ChangeHeight(Label target, double sizeChange)
        {
            target.HeightRequest += sizeChange;
            //TODO: call redraw
        }
    }
}

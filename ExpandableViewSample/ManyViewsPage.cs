using System;
using Expandable;
using Xamarin.Forms;
using TouchEffect;
using System.Diagnostics;
namespace ExpandableViewSample
{
    public class ManyViewsPage : ContentPage
    {
        public ManyViewsPage()
        {
            var mainStack = new StackLayout
            {
                Spacing = 15
            };

            for (int i = 0; i < 15; ++i)
            {
                mainStack.Children.Add(CreateExpandable(i));
            }

            BackgroundColor = Color.Black;
            Content = new ScrollView
            {
                Padding = new Thickness(20, Device.RuntimePlatform == Device.iOS ? 20 : 0, 20, 0),
                Content = mainStack
            };
        }

        private View CreateExpandable(int number)
        {
            var middleImage = new Image
            {
                HeightRequest = 40,
                Aspect = Aspect.Fill,
                Source = "grad.jpg"
            };

            var arrowImage = new Image
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                Source = "arrow_drop_down.png",
                HeightRequest = 45,
                WidthRequest = 45
            };

            var exp = new ExpandableView
            {
                IsTouchToExpandEnabled = false,
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
                            BackgroundColor = Color.White,
                            TextColor = Color.Black,
                            Text = $"Increase height",
                            Command = new Command(() => ChangeHeight(middleImage, 20))
                        },
                        middleImage,
                        new Button
                        {
                            CornerRadius = 0,
                            FontAttributes = FontAttributes.Italic,
                            HeightRequest = 40,
                            BackgroundColor = Color.White,
                            TextColor = Color.Black,
                            Text = $"Decrease height",
                            Command = new Command(() => ChangeHeight(middleImage, -20))
                        }
                    }
                })
            };

            exp.StatusChanged += (sender, e) =>
            {
                var rotation = 0;
                switch (e.Status)
                {
                    case ExpandStatus.Collapsing:
                        break;
                    case ExpandStatus.Expanding:
                        rotation = 180;
                        break;
                    default:
                        return;
                }
                arrowImage.RotateTo(rotation, 200, Easing.CubicInOut);
            };

            exp.PrimaryView = new TouchView
            {
                RegularAnimationEasing = Easing.CubicInOut,
                PressedAnimationEasing = Easing.CubicInOut,
                RegularAnimationDuration = 600,
                PressedAnimationDuration = 600,
                RegularBackgroundColor = Color.White,
                RippleCount = -1,
                PressedBackgroundColor = Color.LightGray,
                PressedScale = 1.2,
                Command = new Command(() => exp.IsExpanded = !exp.IsExpanded)
            };

            exp.PrimaryView.ChildAdded += (sender, e) => ExpandableOnChildAdded(e.Element);

            (exp.PrimaryView as TouchView).Children.Add(new Frame
            {
                CornerRadius = 5,
                Padding = new Thickness(10, 0),
                Content = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Children =
                                    {
                                        new Label
                                        {
                                            FontSize = 22,
                                            VerticalTextAlignment = TextAlignment.Center,
                                            HorizontalTextAlignment = TextAlignment.Center,
                                            FontAttributes = FontAttributes.Bold,
                                            HeightRequest = 60,
                                            TextColor = Color.Black,
                                            Text = $"Click to expand {number}"
                                        },
                                        arrowImage
                                    }
                }
            });


            (exp.PrimaryView as TouchView).StatusChanged += (sender, args) =>
            {
                if(args.Status == TouchEffect.Enums.TouchStatus.Canceled)
                {
                    (exp.PrimaryView as TouchView).Command.Execute(null);
                }
            };

            return exp;
        }

        protected void ExpandableOnChildAdded(Element child)
        {
            AbsoluteLayout.SetLayoutFlags(child, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(child, new Rectangle(0, 0, 1, 1));
            base.OnChildAdded(child);
        }

        private void ChangeHeight(View target, double sizeChange)
        {
            target.HeightRequest += sizeChange;
            (target.Parent.Parent as ExpandableView).ForceUpdateSize();
        }
    }
}

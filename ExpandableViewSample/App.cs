using System;
using Xamarin.Forms;
using Expandable;

namespace ExpandableViewSample
{
	public class App : Application
	{
		public App()
		{
			var mainStack = new StackLayout();

			for (int i = 0; i < 15; ++i)
			{
				mainStack.Children.Add(CreateExpandable(i));
			}

            //MainPage = new ContentPage
            //{
            //	BackgroundColor = Color.White,
            //	Content = new ScrollView
            //	{
            //		Padding = new Thickness(20, Device.RuntimePlatform == Device.iOS ? 20 : 0, 20, 0),
            //		Content = mainStack
            //	}
            //};
            MainPage = new AttachTapGestureToCustomViewPage();
		}

		private View CreateExpandable(int number)
		{
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
						new Label
						{
							VerticalTextAlignment = TextAlignment.Center,
							HorizontalTextAlignment = TextAlignment.Center,
							FontAttributes = FontAttributes.Italic,
							HeightRequest = 40,
							BackgroundColor = Color.Black,
							TextColor = Color.White,
							Text = $"The First of {number}"
						},
						new Label
						{
							VerticalTextAlignment = TextAlignment.Center,
							HorizontalTextAlignment = TextAlignment.Center,
							FontAttributes = FontAttributes.Italic,
							HeightRequest = 40,
							BackgroundColor = Color.Black,
							TextColor = Color.White,
							Text = $"The Second {number}"
						},
						new Label
						{
							VerticalTextAlignment = TextAlignment.Center,
							HorizontalTextAlignment = TextAlignment.Center,
							FontAttributes = FontAttributes.Italic,
							HeightRequest = 40,
							BackgroundColor = Color.Black,
							TextColor = Color.White,
							Text = $"The Third {number}"
						}
					}
				})
			};
		}
	}
}

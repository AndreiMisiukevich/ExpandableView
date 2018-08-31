﻿using System;
using Xamarin.Forms;
using Expandable;

namespace ExpandableViewSample
{
    public class App : Application
    {
        public App()
        {
            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    Children = {
                        new Button
                        {
                            Text = "Many views",
                            Command = new Command(() => {
                                MainPage.Navigation.PushAsync(new ManyViewsPage());
                            })
                        },
                        new Button
                        {
                            Text = "Arrow view",
                            Command = new Command(() => {
                                MainPage.Navigation.PushAsync(new AttachTapGestureToCustomViewPage());
                            })
                        }
                    }
                }
            };

            MainPage = new NavigationPage(MainPage);
        }
    }
}

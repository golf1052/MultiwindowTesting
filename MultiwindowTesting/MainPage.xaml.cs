using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using System.Threading.Tasks;
using System.Diagnostics;
using MultiwindowClassLibrary;
using System.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MultiwindowTesting
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Random random;
        NumberCruncher numberCruncher;
        SynchronizationContext synchronizationContext;

        public MainPage()
        {
            this.InitializeComponent();
            random = new Random();
            numberCruncher = new NumberCruncher();
            numberCruncher.NumberEvent += NumberCruncher_NumberEvent;
            synchronizationContext = SynchronizationContext.Current;
        }

        private async void NumberCruncher_NumberEvent(object sender, NumberEventArgs e)
        {
            Debug.WriteLine($"{ApplicationView.GetForCurrentView().Id} is trying to display {e.Id} {e.Number} on {Environment.CurrentManagedThreadId}");
            listView.Items.Add($"{e.Id} sent {e.Number}");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NumberCruncher.Register(ApplicationView.GetForCurrentView().Id, Window.Current.Dispatcher, numberCruncher);
            UpdateText();
            //ApplicationView.GetForCurrentView().Consolidated += MainPage_Consolidated;
            //CoreApplication.GetCurrentView().CoreWindow.Closed += CoreWindow_Closed;
        }

        private void MainPage_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            Debug.WriteLine($"{ApplicationView.GetForCurrentView().Id} was consolidated");
            if (!CoreApplication.GetCurrentView().IsMain)
            {
                CoreApplication.GetCurrentView().CoreWindow.Close();
            }
        }

        private void CoreWindow_Closed(CoreWindow sender, CoreWindowEventArgs args)
        {
            Debug.WriteLine($"{ApplicationView.GetForCurrentView().Id} was closed");
        }

        private async void spawnWindowButton_Click(object sender, RoutedEventArgs e)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(MainPage));
                Window.Current.Content = frame;
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        private async Task UpdateText()
        {
            bool doubleCheck = false;
            while (true)
            {
                var view = CoreApplication.GetCurrentView();
                var id = ApplicationView.GetForCurrentView().Id;
                var isMain = view.IsMain.ToString();
                idTextBlock.Text = id.ToString();
                mainWindowTextBlock.Text = isMain;
                Debug.WriteLine($"{id}: {isMain}\n");
                if (!Window.Current.Visible)
                {
                    Debug.WriteLine("window is not visible");
                    if (!doubleCheck)
                    {
                        doubleCheck = true;
                    }
                    else
                    {
                        //Window.Current.Close();
                        //break;
                    }
                }
                await Task.Delay(500);
            }
        }

        private async void viewCountButton_Click(object sender, RoutedEventArgs e)
        {
            numberCruncher.SendInNumber(ApplicationView.GetForCurrentView().Id, random.Next());
        }
    }
}

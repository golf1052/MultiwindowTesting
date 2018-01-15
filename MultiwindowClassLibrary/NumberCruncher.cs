using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Core;
using Windows.UI.Notifications;

namespace MultiwindowClassLibrary
{
    public class NumberCruncher
    {
        private static Dictionary<int, Tuple<CoreDispatcher, NumberCruncher>> StaticDispatchers { get; set; }

        public event EventHandler<object> RandomNumberEvent;

        static NumberCruncher()
        {
            StaticDispatchers = new Dictionary<int, Tuple<CoreDispatcher, NumberCruncher>>();
        }

        public NumberCruncher()
        {
        }
        
        public event EventHandler<NumberEventArgs> NumberEvent;

        public static void Register(int id, CoreDispatcher dispatcher, NumberCruncher numberCruncher)
        {
            StaticDispatchers.Add(id, new Tuple<CoreDispatcher, NumberCruncher>(dispatcher, numberCruncher));
        }

        public async Task SendInNumber(int id, int value)
        {
            foreach (var dispatcher in StaticDispatchers)
            {
                await dispatcher.Value.Item1.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Debug.WriteLine($"invoking {dispatcher.Key}");
                    // invoking on the dispatcher's EventHandler works
                    dispatcher.Value.Item2.NumberEvent?.Invoke(null, new NumberEventArgs(id, value));

                    // invoking on this class's EventHandler doesn't work
                    // however if we wrap the listView.Add in a SynchronizationContext.Post it does work
                    //NumberEvent?.Invoke(null, new NumberEventArgs(id, value));
                });
            }
        }

        public async Task StartNumberFetcher()
        {
            while (true)
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.GetAsync("https://random.org/integers?num=1&min=0&max=1000000&col=1&base=10&format=plain");
                if (responseMessage.IsSuccessStatusCode)
                {
                    string message = await responseMessage.Content.ReadAsStringAsync();
                    try
                    {
                        int number = int.Parse(message);
                        //RandomNumberEvent?.Invoke(this, new NumberModel(number));
                        //NumberEvent?.Invoke(this, new NumberEventArgs(0, number));
                        SendNotification(number.ToString());
                    }
                    catch (FormatException ex)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private void SendNotification(string content)
        {
            ToastContent toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = content
                            }
                        }
                    }
                }
            };
            var toast = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}

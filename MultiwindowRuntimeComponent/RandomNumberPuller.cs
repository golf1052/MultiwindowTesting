using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Background;
using System.Net.Http;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using MultiwindowClassLibrary;

namespace MultiwindowRuntimeComponent
{
    public sealed class RandomNumberPuller : IBackgroundTask, IRuntimeInterface
    {
        private BackgroundTaskDeferral deferral;

        private DateTime taskEndTime;
        private NumberCruncher numberCruncher;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            taskEndTime = DateTime.Now + TimeSpan.FromSeconds(25);
            numberCruncher = new NumberCruncher();
            //numberCruncher.RandomNumberEvent += NumberCruncher_RandomNumberEvent;
            numberCruncher.NumberEvent += NumberCruncher_NumberEvent;
            numberCruncher.StartNumberFetcher();
            await CheckTimer();
            deferral.Complete();
        }

        private void NumberCruncher_NumberEvent(object sender, NumberEventArgs e)
        {
            SendNotification(e.Number.ToString());
        }

        private void NumberCruncher_RandomNumberEvent(object sender, object e)
        {
            NumberModel numberModel = (NumberModel)e;
            SendNotification(numberModel.Number.ToString());
        }

        private async Task CheckTimer()
        {
            while (true)
            {
                if (DateTime.Now >= taskEndTime)
                {
                    return;
                }
                await Task.Delay(TimeSpan.FromMilliseconds(250));
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

        public void Numbering()
        {
            throw new NotImplementedException();
        }

        public void Hello()
        {
            throw new NotImplementedException();
        }
    }
}

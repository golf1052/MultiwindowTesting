using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace MultiwindowClassLibrary
{
    public class NumberCruncher
    {
        private static Dictionary<int, Tuple<CoreDispatcher, NumberCruncher>> StaticDispatchers { get; set; }

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
    }
}

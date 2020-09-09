using System;
using System.Diagnostics;
using System.Threading;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers.TaskFactories;
using GtaKeyboardHook.Infrastructure.Helpers;
using GtaKeyboardHook.Model.Messages;
using GtaKeyboardHook.Model.Parameters;
using TinyMessenger;

namespace GtaKeyboardHook.Infrastructure.BackgroundWorkers
{
    public class PixelTrackerBackgroundWorker : BaseBackgoundWorker<CheckPixelDifferenceParameter>
    {
        private readonly ITinyMessengerHub _messageBus;

        public PixelTrackerBackgroundWorker(ITaskFactory factory, ITinyMessengerHub messageBus) : base(factory)
        {
            _messageBus = messageBus;
        }

        protected override void ExecuteInternal(CheckPixelDifferenceParameter param, CancellationToken token)
        {
            var stopWatch = new Stopwatch();
            
            while (true)
            {
                token.ThrowIfCancellationRequested();

                stopWatch.Restart();
                var color = Win32ApiHelper.GetPixelColorFromDesktop(param.Pixel.X, param.Pixel.Y);
                stopWatch.Stop();

                // For some reason, the GetPixel method of the WinAPI is executed every time with different duration (from 15 to 35 ms). 
                // The rest of code HAS TO be executed every time after the same duration, so we'll wait up to 100ms
                Thread.Sleep(100 - (int) stopWatch.ElapsedMilliseconds);
                
                if (!(color.R == param.HookedColor.R && color.G == param.HookedColor.G &&
                      color.B == param.HookedColor.B))
                    continue;

                _messageBus.Publish(new PixelColorChangedMessage(this));
                break;
            }
        }
    }
}
using System;
using System.Threading;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers.TaskFactories;
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

        protected override async void ExecuteInternal(CheckPixelDifferenceParameter param, CancellationToken token)
        {
            while (true)
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    var color = Win32ApiHelper.GetPixelColor(param.Pixel.X, param.Pixel.Y);

                    if (!(color.R == param.HookedColor.R && color.G == param.HookedColor.G &&
                          color.B == param.HookedColor.B))
                        continue;

                    _messageBus.Publish(new PixelColorChangedMessage(this));
                    break;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Exception occured while executing action");
                }
            }
        }
    }
}
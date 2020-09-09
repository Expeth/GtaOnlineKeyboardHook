using System;
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
            while (true)
            {
                token.ThrowIfCancellationRequested();

                var color = Win32ApiHelper.GetPixelColorFromDesktop(param.Pixel.X, param.Pixel.Y);

                if (!(color.R == param.HookedColor.R && color.G == param.HookedColor.G &&
                      color.B == param.HookedColor.B))
                    continue;

                _messageBus.Publish(new PixelColorChangedMessage(this));
                break;
            }
        }
    }
}
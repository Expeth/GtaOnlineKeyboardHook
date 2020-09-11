using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers.TaskFactories;

namespace GtaKeyboardHook.Infrastructure.BackgroundWorkers
{
    public class PreviewUpdateWorker : BaseBackgoundWorker<(Func<Point> hookedPixel, Predicate<object> isHookEnabled)>
    {
        public PreviewUpdateWorker(ITaskFactory factory, PreviewImageHolder previewWindow) : base(factory)
        {
            PreviewWindow = previewWindow;
        }

        public PreviewImageHolder PreviewWindow { get; }

        protected override async void ExecuteInternal((Func<Point> hookedPixel, Predicate<object> isHookEnabled) param, CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                var point = param.hookedPixel.Invoke();
                var isEnabled = param.isHookEnabled.Invoke(this);
                PreviewWindow.Execute((point, isEnabled ? Color.Red : Color.Green), token);

                await Task.Delay(40, token);
            }
        }
    }
}
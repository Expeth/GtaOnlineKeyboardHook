using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers.TaskFactories;

namespace GtaKeyboardHook.Infrastructure.BackgroundWorkers
{
    public class PreviewUpdateWorker : BaseBackgoundWorker<Func<Point>>
    {
        public PreviewUpdateWorker(ITaskFactory factory, PreviewImageHolder previewWindow) : base(factory)
        {
            PreviewWindow = previewWindow;
        }

        public PreviewImageHolder PreviewWindow { get; }

        protected override async void ExecuteInternal(Func<Point> param, CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                var point = param.Invoke();
                PreviewWindow.Execute(point, token);

                await Task.Delay(40, token);
            }
        }
    }
}
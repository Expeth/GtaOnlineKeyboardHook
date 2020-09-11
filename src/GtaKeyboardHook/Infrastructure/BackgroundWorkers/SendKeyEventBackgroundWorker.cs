using System.Threading;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers.TaskFactories;
using GtaKeyboardHook.Infrastructure.Helpers;
using GtaKeyboardHook.Model.Parameters;

namespace GtaKeyboardHook.Infrastructure.BackgroundWorkers
{
    public class SendKeyEventBackgroundWorker : BaseBackgoundWorker<SendKeyEventParameter>
    {
        public SendKeyEventBackgroundWorker(ITaskFactory factory) : base(factory)
        {
        }

        protected override void ExecuteInternal(SendKeyEventParameter param, CancellationToken token)
        {
            Logger.Information("Sending KeyPressedEvent with {@sendKeyEventParameter}", param);

            Thread.Sleep(param.DelayDuration);
            Win32ApiHelper.SendKeyPressedEvent(param.HookedKey);
        }
    }
}
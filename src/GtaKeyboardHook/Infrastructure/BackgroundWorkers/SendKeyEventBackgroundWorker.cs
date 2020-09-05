using System.Threading;
using System.Threading.Tasks;
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

        protected override async void ExecuteInternal(SendKeyEventParameter param, CancellationToken token)
        {
            Logger.Information("Sending KeyPressedEvent with {@sendKeyEventParameter}", param);

            await Task.Delay(param.DelayDuration, token);
            Win32ApiHelper.SendKeyPressedEvent(param.HookedKey);
        }
    }
}
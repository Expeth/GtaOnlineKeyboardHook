using System.Threading;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers.TaskFactories;
using GtaKeyboardHook.Infrastructure.Configuration;

namespace GtaKeyboardHook.Infrastructure.BackgroundWorkers
{
    public class ConfigSaverBackgroundWorker : BaseBackgoundWorker<IProfileConfigurationProvider>
    {
        public ConfigSaverBackgroundWorker(ITaskFactory factory) : base(factory)
        {
        }

        protected override async void ExecuteInternal(IProfileConfigurationProvider param, CancellationToken token)
        {
            await param.SaveAsync();
        }
    }
}
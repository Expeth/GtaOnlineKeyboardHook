using System;
using System.Threading;
using System.Threading.Tasks;

namespace GtaKeyboardHook.Infrastructure.BackgroundWorkers.TaskFactories
{
    public interface ITaskFactory
    {
        Task GetInstance(Action action, CancellationToken token);
    }
}
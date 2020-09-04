using System;
using System.Threading;
using System.Threading.Tasks;

namespace GtaKeyboardHook.Infrastructure.BackgroundWorkers.TaskFactories
{
    public class MultipleTaskFactory : ITaskFactory
    {
        public Task GetInstance(Action action, CancellationToken token)
        {
            return new Task(action, token);
        }
    }
}
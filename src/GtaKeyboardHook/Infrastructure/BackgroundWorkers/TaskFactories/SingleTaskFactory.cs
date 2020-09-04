using System;
using System.Threading;
using System.Threading.Tasks;

namespace GtaKeyboardHook.Infrastructure.BackgroundWorkers.TaskFactories
{
    public class SingleTaskFactory : ITaskFactory
    {
        private Task _task;

        public Task GetInstance(Action action, CancellationToken token)
        {
            if (_task == null || _task.Status == TaskStatus.Canceled || _task.Status == TaskStatus.Faulted ||
                _task.Status == TaskStatus.RanToCompletion)
                _task = new Task(action, token);

            if (_task.Status == TaskStatus.Running || _task.Status == TaskStatus.WaitingToRun) return null;

            return _task;
        }
    }
}
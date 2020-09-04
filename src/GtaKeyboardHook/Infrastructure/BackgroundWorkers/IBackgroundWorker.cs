using System.Threading;

namespace GtaKeyboardHook.Infrastructure.BackgroundWorkers
{
    public interface IBackgroundWorker<TParameter>
    {
        void Execute(TParameter param, CancellationToken token);
    }
}
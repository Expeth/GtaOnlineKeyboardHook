using System.Threading;

namespace GtaKeyboardHook.Infrastructure.BackgroundWorkers
{
    public interface IExecutable<TParameter>
    {
        void Execute(TParameter param, CancellationToken token);
    }
}
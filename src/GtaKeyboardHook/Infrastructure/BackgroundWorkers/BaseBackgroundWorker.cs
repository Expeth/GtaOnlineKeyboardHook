using System;
using System.Threading;
using System.Threading.Tasks;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers.TaskFactories;
using Serilog;
using Serilog.Context;

namespace GtaKeyboardHook.Infrastructure.BackgroundWorkers
{
    public abstract class BaseBackgoundWorker<TParameter> : IExecutable<TParameter>
    {
        protected static readonly ILogger Logger = Log.ForContext<BaseBackgoundWorker<TParameter>>();

        private Task _backgroundTask;
        private readonly ITaskFactory _taskFactory;

        protected BaseBackgoundWorker(ITaskFactory factory)
        {
            _taskFactory = factory;
        }

        public void Execute(TParameter param, CancellationToken token)
        {
            _backgroundTask = _taskFactory.GetInstance(() => ExecuteInternal(param, token), token);

            using var _ = LogContext.PushProperty("CorrelationID", new Guid());

            Logger.Information("Attempting to start a background task");
            try
            {
                _backgroundTask?.Start();
            }
            catch (OperationCanceledException e)
            {
                Logger.Information(e, "Background task was cancelled");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Exception occured while executing a job");
                throw;
            }
        }

        protected abstract void ExecuteInternal(TParameter param, CancellationToken token);
    }
}
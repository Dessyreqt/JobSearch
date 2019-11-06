namespace JobSearch.Infrastructure.CommandProcessing
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using JobSearch.Infrastructure.Logging;
    using MediatR;

    public class PerformanceLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var watch = new Stopwatch();
            watch.Start();
            Logger.Instance.Information("Handling {RequestName}", typeof(TRequest).FullName);

            TResponse response;

            try
            {
                response = await next();
            }
            finally
            {
                watch.Stop();
                Logger.Instance.Information("Handled {RequestName} in {Elapsed:000}ms", typeof(TRequest).FullName, watch.ElapsedMilliseconds);
            }

            return response;
        }
    }
}

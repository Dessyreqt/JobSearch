namespace JobSearch.Features.JobApplications.GetJobApplications
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthRequest, IRequest<List<JobApplicationResponse>>
    {
    }

    public class Handler : IRequestHandler<Request, List<JobApplicationResponse>>
    {
        private readonly Func<IDbConnection> _connectionFactory;

        public Handler(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<List<JobApplicationResponse>> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = request.GetUser();
            using var connection = _connectionFactory();
            var response = connection.GetList<JobApplication>("UserId = @UserId", new { UserId = user.Id }).Select(JobApplicationResponse.MapFrom).ToList();

            return Task.Run(() => response, cancellationToken);
        }
    }
}

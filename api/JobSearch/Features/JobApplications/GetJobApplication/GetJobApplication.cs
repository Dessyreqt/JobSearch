namespace JobSearch.Features.JobApplications.GetJobApplication
{
    using System;
    using System.Data;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthIdRequest, IRequest<JobApplicationResponse>
    {
    }

    public class Handler : IRequestHandler<Request, JobApplicationResponse>
    {
        private readonly Func<IDbConnection> _connectionFactory;

        public Handler(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<JobApplicationResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var id = request.GetId();
            var user = request.GetUser();
            using var connection = _connectionFactory();
            var jobApplication = connection.GetById<JobApplication>(id);

            if (jobApplication.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            return Task.Run(() => JobApplicationResponse.MapFrom(jobApplication));
        }
    }
}

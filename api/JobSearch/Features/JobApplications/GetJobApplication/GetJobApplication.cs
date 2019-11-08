namespace JobSearch.Features.JobApplications.GetJobApplication
{
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
        private readonly IDbConnection _connection;

        public Handler(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<JobApplicationResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var id = request.GetId();
            var user = request.GetUser();
            var jobApplication = _connection.GetById<JobApplication>(id);

            if (jobApplication.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            return Task.Run(() => JobApplicationResponse.MapFrom(jobApplication));
        }
    }
}

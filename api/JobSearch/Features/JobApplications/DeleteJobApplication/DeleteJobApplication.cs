namespace JobSearch.Features.JobApplications.DeleteJobApplication
{
    using System.Data;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthIdRequest, IRequest<Response>
    {
    }

    public class Response
    {
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IDbConnection _connection;

        public Handler(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var id = request.GetId();
            var user = request.GetUser();
            var jobApplication = _connection.GetById<JobApplication>(id);

            if (jobApplication == null)
            {
                throw new FileNotFoundException();
            }

            if (jobApplication.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            _connection.Delete(jobApplication);

            return Task.Run(() => new Response(), cancellationToken);
        }
    }
}

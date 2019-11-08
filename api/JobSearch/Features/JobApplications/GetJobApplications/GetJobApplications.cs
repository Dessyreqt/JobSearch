namespace JobSearch.Features.JobApplications.GetJobApplications
{
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
        private readonly IDbConnection _connection;

        public Handler(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<List<JobApplicationResponse>> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = request.GetUser();
            var response = _connection.GetList<JobApplication>("UserId = @UserId", new { UserId = user.Id }).Select(JobApplicationResponse.MapFrom).ToList();

            return Task.Run(() => response, cancellationToken);
        }
    }
}

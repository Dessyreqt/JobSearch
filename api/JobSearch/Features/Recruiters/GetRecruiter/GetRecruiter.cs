namespace JobSearch.Features.Recruiters.GetRecruiter
{
    using System.Data;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthRequest, IRequest<RecruiterResponse>
    {
        public int Id { get; set; }
    }

    public class Validation : AuthValidator<Request>
    {
    }

    public class Handler : IRequestHandler<Request, RecruiterResponse>
    {
        private readonly IDbConnection _connection;

        public Handler(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<RecruiterResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = request.GetUser();
            var recruiter = _connection.GetById<Recruiter>(request.Id);

            if (recruiter.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            return Task.Run(() => RecruiterResponse.MapFrom(recruiter));
        }
    }
}

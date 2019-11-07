namespace JobSearch.Features.Recruiters.GetRecruiters
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthRequest, IRequest<List<RecruiterResponse>>
    {
    }

    public class Validation : AuthValidator<Request>
    {
    }

    public class Handler : IRequestHandler<Request, List<RecruiterResponse>>
    {
        private readonly IDbConnection _connection;

        public Handler(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<List<RecruiterResponse>> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = request.GetUser();
            var response = _connection.GetList<Recruiter>("UserId = @UserId", new { UserId = user.Id }).Select(RecruiterResponse.MapFrom).ToList();

            return Task.Run(() => response, cancellationToken);
        }
    }
}

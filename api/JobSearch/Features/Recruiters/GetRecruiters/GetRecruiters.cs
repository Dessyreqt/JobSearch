namespace JobSearch.Features.Recruiters.GetRecruiters
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Dapper;
    using MediatR;

    public class Request : AuthRequest, IRequest<List<Response>>
    {
    }

    public class Response
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class Validation : AuthValidator<Request>
    {
    }

    public class Handler : IRequestHandler<Request, List<Response>>
    {
        private readonly IDbConnection _connection;

        public Handler(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = "select * from [Recruiter] where UserId = @UserId";
            var user = request.GetUser();
            var retVal = (await _connection.QueryAsync<Response>(query, new { UserId = user.Id })).ToList();

            return retVal;
        }
    }
}

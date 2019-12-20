namespace JobSearch.Features.Recruiters.GetRecruiters
{
    using System;
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

    public class Handler : IRequestHandler<Request, List<RecruiterResponse>>
    {
        private readonly Func<IDbConnection> _connectionFactory;

        public Handler(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<List<RecruiterResponse>> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = request.GetUser();
            using var connection = _connectionFactory();
            var response = connection.GetList<Recruiter>("UserId = @UserId", new { UserId = user.Id }).Select(RecruiterResponse.MapFrom).ToList();

            return Task.Run(() => response, cancellationToken);
        }
    }
}

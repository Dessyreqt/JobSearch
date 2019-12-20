namespace JobSearch.Features.Followups.GetFollowups
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthRequest, IRequest<List<FollowupResponse>>
    {
    }

    public class Handler : IRequestHandler<Request, List<FollowupResponse>>
    {
        private readonly Func<IDbConnection> _connectionFactory;

        public Handler(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<List<FollowupResponse>> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = request.GetUser();
            using var connection = _connectionFactory();
            var response = connection.GetList<Followup>("UserId = @UserId", new { UserId = user.Id }).Select(FollowupResponse.MapFrom).ToList();

            return Task.Run(() => response, cancellationToken);
        }
    }
}

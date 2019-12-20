namespace JobSearch.Features.Followups.GetFollowup
{
    using System;
    using System.Data;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthIdRequest, IRequest<FollowupResponse>
    {
    }

    public class Handler : IRequestHandler<Request, FollowupResponse>
    {
        private readonly Func<IDbConnection> _connectionFactory;

        public Handler(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<FollowupResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var id = request.GetId();
            var user = request.GetUser();
            using var connection = _connectionFactory();
            var followup = connection.GetById<Followup>(id);

            if (followup.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            return Task.Run(() => FollowupResponse.MapFrom(followup));
        }
    }
}

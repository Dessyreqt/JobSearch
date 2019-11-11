namespace JobSearch.Features.Followups.GetFollowup
{
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
        private readonly IDbConnection _connection;

        public Handler(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<FollowupResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var id = request.GetId();
            var user = request.GetUser();
            var followup = _connection.GetById<Followup>(id);

            if (followup.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            return Task.Run(() => FollowupResponse.MapFrom(followup));
        }
    }
}

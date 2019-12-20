namespace JobSearch.Features.Followups.DeleteFollowup
{
    using System;
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
        private readonly Func<IDbConnection> _connectionFactory;

        public Handler(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var id = request.GetId();
            var user = request.GetUser();
            using var connection = _connectionFactory();
            var followup = connection.GetById<Followup>(id);

            if (followup == null)
            {
                throw new FileNotFoundException();
            }

            if (followup.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            connection.Delete(followup);

            return Task.Run(() => new Response(), cancellationToken);
        }
    }
}

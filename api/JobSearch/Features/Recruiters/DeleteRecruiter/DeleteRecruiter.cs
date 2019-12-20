namespace JobSearch.Features.Recruiters.DeleteRecruiter
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
            var recruiter = connection.GetById<Recruiter>(id);

            if (recruiter == null)
            {
                throw new FileNotFoundException();
            }

            if (recruiter.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            connection.Delete(recruiter);

            return Task.Run(() => new Response(), cancellationToken);
        }
    }
}

namespace JobSearch.Features.Recruiters.GetRecruiter
{
    using System;
    using System.Data;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthIdRequest, IRequest<RecruiterResponse>
    {
    }

    public class Handler : IRequestHandler<Request, RecruiterResponse>
    {
        private readonly Func<IDbConnection> _connectionFactory;

        public Handler(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<RecruiterResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var id = request.GetId();
            var user = request.GetUser();
            using var connection = _connectionFactory();
            var recruiter = connection.GetById<Recruiter>(id);

            if (recruiter.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            return Task.Run(() => RecruiterResponse.MapFrom(recruiter));
        }
    }
}

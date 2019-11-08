﻿namespace JobSearch.Features.Recruiters.GetRecruiter
{
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
        private readonly IDbConnection _connection;

        public Handler(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<RecruiterResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var id = request.GetId();
            var user = request.GetUser();
            var recruiter = _connection.GetById<Recruiter>(id);

            if (recruiter.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            return Task.Run(() => RecruiterResponse.MapFrom(recruiter));
        }
    }
}
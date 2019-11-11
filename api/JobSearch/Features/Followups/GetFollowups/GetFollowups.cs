﻿namespace JobSearch.Features.Followups.GetFollowups
{
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
        private readonly IDbConnection _connection;

        public Handler(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<List<FollowupResponse>> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = request.GetUser();
            var response = _connection.GetList<Followup>("UserId = @UserId", new { UserId = user.Id }).Select(FollowupResponse.MapFrom).ToList();

            return Task.Run(() => response, cancellationToken);
        }
    }
}

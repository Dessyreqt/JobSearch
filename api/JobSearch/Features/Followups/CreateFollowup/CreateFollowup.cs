namespace JobSearch.Features.Followups.CreateFollowup
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthRequest, IRequest<FollowupResponse>
    {
        public DateTime ActivityDate { get; set; }
        public int JobApplicationId { get; set; }
        public string FollowupDescription { get; set; }
    }

    public class Validation : AuthValidator<Request>
    {
        public Validation()
        {
            RuleFor(x => x.ActivityDate).NotNull();
            RuleFor(x => x.FollowupDescription).NotNull();
        }
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
            var user = request.GetUser();
            var followup = new Followup { UserId = user.Id, ActivityDate = request.ActivityDate, JobApplicationId = request.JobApplicationId, FollowupDescription = request.FollowupDescription };
            using var connection = _connectionFactory();

            connection.Save(followup);
            return Task.Run(() => FollowupResponse.MapFrom(followup), cancellationToken);
        }
    }
}

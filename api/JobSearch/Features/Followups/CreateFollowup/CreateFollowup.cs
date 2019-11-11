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
        private readonly IDbConnection _connection;

        public Handler(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<FollowupResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = request.GetUser();
            var followup = new Followup { UserId = user.Id, ActivityDate = request.ActivityDate, JobApplicationId = request.JobApplicationId, FollowupDescription = request.FollowupDescription };

            _connection.Save(followup);
            return Task.Run(() => FollowupResponse.MapFrom(followup), cancellationToken);
        }
    }
}

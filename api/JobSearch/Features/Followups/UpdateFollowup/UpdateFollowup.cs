namespace JobSearch.Features.Followups.UpdateFollowup
{
    using System;
    using System.Data;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthIdRequest, IRequest<FollowupResponse>
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
            var id = request.GetId();
            using var connection = _connectionFactory();
            var followup = connection.GetById<Followup>(id);

            if (followup.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            followup.ActivityDate = request.ActivityDate;
            followup.JobApplicationId = request.JobApplicationId;
            followup.FollowupDescription = request.FollowupDescription;

            connection.Save(followup);

            return Task.Run(() => FollowupResponse.MapFrom(followup), cancellationToken);
        }
    }
}

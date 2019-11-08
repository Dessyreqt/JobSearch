namespace JobSearch.Features.JobApplications.CreateJobApplication
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthRequest, IRequest<JobApplicationResponse>
    {
        public string Position { get; set; }
        public string Company { get; set; }
        public int? RecruiterId { get; set; }
        public int ApplicationStatusId { get; set; }
        public DateTime InitialContactDate { get; set; }
        public string JobDescriptionUrl { get; set; }
    }

    public class Validation : AuthValidator<Request>
    {
        public Validation()
        {
            RuleFor(x => x.Position).NotNull().MaximumLength(100);
            RuleFor(x => x.Company).NotNull().MaximumLength(50);
            RuleFor(x => x.ApplicationStatusId).InclusiveBetween(1, 4);
        }
    }

    public class Handler : IRequestHandler<Request, JobApplicationResponse>
    {
        private readonly IDbConnection _connection;

        public Handler(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<JobApplicationResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = request.GetUser();
            var jobApplication = new JobApplication
            {
                UserId = user.Id,
                Position = request.Position,
                Company = request.Company,
                RecruiterId = request.RecruiterId,
                ApplicationStatusId = request.ApplicationStatusId,
                InitialContactDate = request.InitialContactDate,
                JobDescriptionUrl = request.JobDescriptionUrl
            };

            _connection.Save(jobApplication);
            return Task.Run(() => JobApplicationResponse.MapFrom(jobApplication), cancellationToken);
        }
    }
}

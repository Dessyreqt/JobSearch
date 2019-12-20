namespace JobSearch.Features.JobApplications.UpdateJobApplication
{
    using System;
    using System.Data;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthIdRequest, IRequest<JobApplicationResponse>
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
        private readonly Func<IDbConnection> _connectionFactory;

        public Handler(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<JobApplicationResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = request.GetUser();
            var id = request.GetId();
            using var connection = _connectionFactory();
            var jobApplication = connection.GetById<JobApplication>(id);

            if (jobApplication.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            jobApplication.Position = request.Position;
            jobApplication.Company = request.Company;
            jobApplication.RecruiterId = request.RecruiterId;
            jobApplication.ApplicationStatusId = request.ApplicationStatusId;
            jobApplication.InitialContactDate = request.InitialContactDate;
            jobApplication.JobDescriptionUrl = request.JobDescriptionUrl;

            connection.Save(jobApplication);

            return Task.Run(() => JobApplicationResponse.MapFrom(jobApplication), cancellationToken);
        }
    }
}

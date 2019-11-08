﻿namespace JobSearch.Features.Recruiters.UpdateRecruiter
{
    using System.Data;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using JobSearch.Domain;
    using MediatR;

    public class Request : AuthIdRequest, IRequest<RecruiterResponse>
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class Validation : AuthValidator<Request>
    {
        public Validation()
        {
            RuleFor(x => x.Name).NotNull().MaximumLength(50);
            RuleFor(x => x.Phone).MaximumLength(20);
            RuleFor(x => x.Email).MaximumLength(200);
        }
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
            var user = request.GetUser();
            var id = request.GetId();
            var recruiter = _connection.GetById<Recruiter>(id);

            if (recruiter.UserId != user.Id)
            {
                throw new FileNotFoundException();
            }

            recruiter.Name = request.Name;
            recruiter.Phone = request.Phone;
            recruiter.Email = request.Email;

            _connection.Save(recruiter);

            return Task.Run(() => RecruiterResponse.MapFrom(recruiter), cancellationToken);
        }
    }
}
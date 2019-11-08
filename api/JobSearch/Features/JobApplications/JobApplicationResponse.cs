namespace JobSearch.Features.JobApplications
{
    using System;
    using JobSearch.Domain;

    public class JobApplicationResponse
    {
        public int Id { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public int? RecruiterId { get; set; }
        public int ApplicationStatusId { get; set; }
        public DateTime InitialContactDate { get; set; }
        public string JobDescriptionUrl { get; set; }

        public static JobApplicationResponse MapFrom(JobApplication jobApplication)
        {
            if (jobApplication == null)
            {
                return null;
            }

            var response = new JobApplicationResponse
            {
                Id = jobApplication.Id,
                Position = jobApplication.Position,
                Company = jobApplication.Company,
                RecruiterId = jobApplication.RecruiterId,
                ApplicationStatusId = jobApplication.ApplicationStatusId,
                InitialContactDate = jobApplication.InitialContactDate,
                JobDescriptionUrl = jobApplication.JobDescriptionUrl
            };
            return response;
        }
    }
}

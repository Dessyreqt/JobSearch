namespace JobSearch.Features.Followups
{
    using System;
    using JobSearch.Domain;

    public class FollowupResponse
    {
        public int Id { get; set; }
        public DateTime ActivityDate { get; set; }
        public int JobApplicationId { get; set; }
        public string FollowupDescription { get; set; }

        public static FollowupResponse MapFrom(Followup followup)
        {
            if (followup == null)
            {
                return null;
            }

            var response = new FollowupResponse { Id = followup.Id, ActivityDate = followup.ActivityDate, JobApplicationId = followup.JobApplicationId, FollowupDescription = followup.FollowupDescription };
            return response;
        }
    }
}

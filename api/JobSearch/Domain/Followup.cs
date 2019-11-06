namespace JobSearch.Domain
{
    using System;

    [IdColumn("FollowupId")]
    public class Followup : DbIdObject
    {
        public int UserId { get; set; }
        public DateTime ActivityDate { get; set; }
        public int JobApplicationId { get; set; }
        public string FollowupDescription { get; set; }
    }
}

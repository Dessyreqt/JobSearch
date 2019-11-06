namespace JobSearch.Domain
{
    using System;

    [IdColumn("JobApplicationId")]
    public class JobApplication : DbIdObject
    {
        public int UserId { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public int? RecruiterId { get; set; }
        public int ApplicationStatusId { get; set; }
        public DateTime InitialContactDate { get; set; }
        public string JobDescriptionUrl { get; set; }
    }
}

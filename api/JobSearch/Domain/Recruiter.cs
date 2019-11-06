namespace JobSearch.Domain
{
    [IdColumn("RecruiterId")]
    public class Recruiter : DbIdObject
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}

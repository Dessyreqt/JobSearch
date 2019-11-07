namespace JobSearch.Features.Recruiters
{
    using JobSearch.Domain;

    public class RecruiterResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public static RecruiterResponse MapFrom(Recruiter recruiter)
        {
            if (recruiter == null)
            {
                return null;
            }

            var response = new RecruiterResponse { Id = recruiter.Id, Name = recruiter.Name, Phone = recruiter.Phone, Email = recruiter.Email };
            return response;
        }
    }
}

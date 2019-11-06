namespace JobSearch.Domain
{
    [IdColumn("ApplicationStatusId")]
    public class ApplicationStatus : DbIdObject
    {
        public int Pending = 1;
        public int Rejected = 2;
        public int OfferExtended = 3;
        public int OfferAccepted = 4;

        public string Name { get; set; }
    }
}

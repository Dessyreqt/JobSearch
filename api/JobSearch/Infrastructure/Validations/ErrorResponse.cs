namespace JobSearch.Infrastructure.Validations
{
    using System.Collections.Generic;

    public class ErrorResponse
    {
        public List<ErrorMessage> Errors { get; set; }
    }
}

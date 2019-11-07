namespace JobSearch.Features
{
    public class AuthIdRequest : AuthRequest
    {
        private int _id;

        public int GetId()
        {
            return _id;
        }

        public void SetId(int id)
        {
            _id = id;
        }
    }
}

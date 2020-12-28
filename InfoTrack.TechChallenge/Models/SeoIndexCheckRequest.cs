namespace InfoTrack.TechChallenge.Models
{
    public class SeoIndexCheckRequest
    {
        public string SearchEngine { get; set; }
        public string Query { get; set; }
        public bool UseStaticPages { get; set; }
        public int MaximumResults { get; set; }
    }
}

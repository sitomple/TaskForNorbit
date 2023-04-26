namespace WebApplication1
{
    public class RatingModel
    {
        public int percent { get; set; }
        public string group { get; set; }

        public RatingModel(string groups, int percents)
        {
            group = groups;
            percent = percents;
        }
    }
}

namespace WebApplication1
{
    public class BodyMassIndexModel
    {
        public double Index { get; set; }
        public string Coment { get; set; }

        public BodyMassIndexModel(string coment, double answer) 
        {
            Index = answer;
            Coment = coment;
        }
    }
}

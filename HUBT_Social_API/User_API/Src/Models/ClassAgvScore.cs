namespace User_API.Src.Models
{
    public class ClassAgvScore
    {
        public int Excellent { get; set; } = 0;
        public int Good { get; set; } = 0;
        public int Fair { get; set; } = 0;
        public int Average { get; set; } = 0;
        public int BelowAverage { get; set; } = 0;
        public int Fail { get; set; } = 0;

        public void AddScore(double score)
        {
            if (score >= 8.5)
            {
                Excellent++;
            }
            else if (score >= 7.0)
            {
                Good++;
            }
            else if (score >= 6.5)
            {
                Fair++;
            }
            else if (score >= 5.0)
            {
                Average++;
            }
            else if (score >= 4.0)
            {
                BelowAverage++;
            }
            else
            {
                Fail++;
            }
        }
    }
}

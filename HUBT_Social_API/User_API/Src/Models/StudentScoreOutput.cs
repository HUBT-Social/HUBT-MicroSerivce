using HUBT_Social_Core.Models.OutSourceDataDTO;

namespace User_API.Src.Models
{
    public class StudentScoreOutput(ScoreDTO scoreDTO)
    {

        public string Subject { get => scoreDTO.TenMonHoc; }
        public double Score10 { get => scoreDTO.Diem;} 
        public double Score4 => scoreDTO.Diem switch
        {
            < 4.0 => 0.0f,
            < 4.7 => 1.0f,
            < 5.4 => 1.5f,
            < 6.2 => 2.0f,
            < 6.9 => 2.5f,
            < 7.7 => 3.0f,
            < 8.5 => 3.5f,
            _ => 4.0f
        };

        public string Grade => scoreDTO.Diem switch
        {
            < 4 => "F",
            < 4.7 => "D",
            < 5.4 => "D+",
            < 6.2 => "C",
            < 6.9 => "C+",
            < 7.7 => "B",
            < 8.5 => "B+",
            _ => "A",
        };
    }
}

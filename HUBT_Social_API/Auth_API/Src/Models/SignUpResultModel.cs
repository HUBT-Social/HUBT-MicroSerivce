namespace Auth_API.Src.Models
{
    public class SignUpResultModel
    {
        public bool Succeeded { get; set; }
        public string[]? Error { get; set; }
    }
}

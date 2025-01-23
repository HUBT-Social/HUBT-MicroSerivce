namespace HUBT_Social_Core.Models.Requests.LoginRequest;

public interface ILoginRequest
{
    string Identifier { get; }
    string Password { get; }
}
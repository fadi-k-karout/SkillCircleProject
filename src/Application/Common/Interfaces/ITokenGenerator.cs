namespace Application.Common.Interfaces;

public interface ITokenGenerator
{
    public string GenerateToken(string userId,  IList<string> roles);
}
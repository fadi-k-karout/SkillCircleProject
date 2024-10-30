namespace Application.Common.Interfaces;

public interface ITokenGenerator
{
    public string GenerateToken(string userId, string userName, IList<string> roles);
}
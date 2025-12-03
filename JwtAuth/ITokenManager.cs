namespace Infusive_back.JwtAuth
{
    public interface ITokenManager
    {
        string NewToken(string userId);
    }
}

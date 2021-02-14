using buddiesApi.Models;
namespace buddiesApi.Middlewares.AuthenticationManager {
    public interface IAuthenticationManager {
        string Authenticate(UserCred userCred, User user);
        string CreateBearerToken(User user);
        public bool IsCorrectPassword(string password, User user);
    }
}

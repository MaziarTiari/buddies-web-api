using buddiesApi.Models;
namespace buddiesApi.Middlewares.AuthenticationManager {
    public interface IAuthenticationManager {
        string Authenticate(UserCred userCred, User user);
    }
}

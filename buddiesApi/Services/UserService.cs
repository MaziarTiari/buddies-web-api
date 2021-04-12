using System;
using System.Linq;
using buddiesApi.Helpers;
using buddiesApi.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace buddiesApi.Services
{
    public class UserService : Service<User>
    {
        public UserService(IBuddiesDbContext settings) : base(settings)
        {
            collection = Database.GetCollection<User>(settings.UsersCollectionName);
        }

        public UserResponce GetUser(string id)
        {
            var query = collection.AsQueryable()
                .Where(u => u.Id == id)
                .Select(u => new UserResponce {
                    Id = u.Id, Email = u.Email, Phone = u.Phone
                });
            return query.First();
        }

        public User GetUserByEmail(string email) {
            return collection.Find<User>(u => u.Email == email).FirstOrDefault();
        }

        public override void Create(User obj)
        {
            obj.Email = obj.Email.ToLower();
            base.Create(obj);
        }

        public override ReplaceOneResult Update(string id, User newObj)
        {
            newObj.Email = newObj.Email.ToLower();
            return base.Update(id, newObj);
        }

        public UpdateResult UpdateUserInfo(string id, UserInfo userInfo) {
            var update = Builders<User>.Update
                .Set(u => u.Phone, userInfo.Phone)
                .Set(u => u.Email, userInfo.Email);
            var result = collection.UpdateOne<User>(u => u.Id == id, update);
            return result;
        }

        public ReplaceOneResult UpdatePassword(User user, string password) {
            var sec = SecurityManager.ComputeHash(
                password,
                SecurityManager.ConvertStringSalt(user.Salt));
            user.Password = sec.HashedSaltedPassword;
            return base.Update(user.Id, user);
        }
    }
}

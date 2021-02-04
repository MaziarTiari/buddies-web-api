using System;
using buddiesApi.Models;
using MongoDB.Driver;

namespace buddiesApi.Services
{
    public class UserService : Service<User>
    {
        public UserService(IBuddiesDbContext settings) : base(settings)
        {
            this.collection =
                base.Database.GetCollection<User>(settings.UsersCollectionName);
        }

        public override User Get(string email)
        {
            return collection.Find<User>(
                u => u.Email == email.ToLower()).FirstOrDefault();
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
    }
}

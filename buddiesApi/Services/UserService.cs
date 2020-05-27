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
                base.GetDatabase.GetCollection<User>(settings.UsersCollectionName);
        }

        public override User Get(string email) =>
            collection.Find<User>(u => u.Email == email).FirstOrDefault();
    }
}

using System;
using System.Collections.Generic;
using buddiesApi.Models;
using Microsoft.AspNetCore.Mvc;
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

        public override void Create(User obj) => collection.InsertOne(obj);

        public override List<User> Get() => collection.Find<User>(u => true).ToList();

        public override User Get(string id) =>
            collection.Find<User>(u => u.Id == id).FirstOrDefault();

        public User GetByEmail(string email) =>
            collection.Find<User>(u => u.Email == email).FirstOrDefault();

        public override void Remove(User newObj) =>
            collection.DeleteOne<User>(u => u.Id == newObj.Id);

        public override void Remove(string id) =>
            collection.DeleteOne<User>(u => u.Id == id);

        public override void Update(string id, User newObj) =>
            collection.ReplaceOne<User>(u => u.Id == id, newObj);
    }
}

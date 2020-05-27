using System;
using System.Collections.Generic;
using buddiesApi.Helpers;
using buddiesApi.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace buddiesApi.Services
{
    public interface IService<T> where T : IMongoDbDocument
    {
        public List<T> Get();

        public T Get(string identifier);

        public void Create(T obj);

        public void Update(string identifier, T newObj);

        public void Remove(T newObj);

        public void Remove(string identifier);
    }

    public abstract class Service<T>: IService<T> where T : IMongoDbDocument
    {
        protected IMongoCollection<T> collection;
        private readonly MongoClient client;
        private readonly IMongoDatabase database;

        public Service(IBuddiesDbContext settings)
        {
            this.client = new MongoClient(settings.ConnectionString);
            this.database = client.GetDatabase(settings.DatabaseName);
        }

        protected MongoClient GetClient => this.client;

        protected IMongoDatabase GetDatabase => this.database;

        public virtual void Create(T obj) => collection.InsertOne(obj);

        public virtual List<T> Get() => collection.Find(u => true).ToList();

        public virtual T Get(string id) =>
            collection.Find(o => o.Id == id).FirstOrDefault();

        public virtual void Remove(T newObj) =>
            collection.DeleteOne(o => o.Id == newObj.Id);

        public virtual void Remove(string id) => collection.DeleteOne(o => o.Id == id);

        public virtual void Update(string id, T newObj) =>
            collection.ReplaceOne(o => o.Id == id, newObj);
    }
}

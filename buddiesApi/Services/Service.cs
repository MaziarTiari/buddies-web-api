using System;
using System.Collections.Generic;
using buddiesApi.Helpers;
using buddiesApi.Models;
using MongoDB.Driver;

namespace buddiesApi.Services
{
    public interface IService<T>
    {
        public List<T> Get();

        public T Get(string identifier);

        public void Create(T obj);

        public void Update(string identifier, T newObj);

        public void Remove(T newObj);

        public void Remove(string identifier);
    }

    public abstract class Service<T>: IService<T>
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

        public abstract void Create(T obj);

        public abstract List<T> Get();

        public abstract T Get(string identifier);

        public abstract void Remove(T newObj);

        public abstract void Remove(string identifier);

        public abstract void Update(string identifier, T newObj);
    }
}

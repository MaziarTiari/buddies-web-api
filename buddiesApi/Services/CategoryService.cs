using System;
using System.Collections.Generic;
using buddiesApi.Models;
using MongoDB.Driver;

namespace buddiesApi.Services
{
    public class CategoryService: Service<Category>
    {
        public CategoryService(IBuddiesDbContext settings) : base(settings) 
        {
            this.collection = base.GetDatabase.GetCollection<Category>(
                settings.CategoriesCollectionName);
        }

        public override List<Category> Get()
        {
            return base.Get();
        }

        public override Category Get(string title)
        {
            return collection.Find(c => c.Title == title).FirstOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace buddiesApi.Models
{
    public class Category: IMongoDbDocument
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string Title { get; set; }

        [BsonRequired]
        public List<CategoryList> Categories { get; set; }
    }

    public class CategoryList
    {
        public List<string> De { get; set; }
        public List<string> En { get; set; }
    }
}

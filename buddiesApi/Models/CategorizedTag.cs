using System;
using MongoDB.Bson.Serialization.Attributes;

namespace buddiesApi.Models
{
    public class CategorizedTag
    {
        [BsonRequired]
        public string Category { get; set; }

        [BsonRequired]
        public string Title { get; set; }

        public string Place { get; set; }
    }
}

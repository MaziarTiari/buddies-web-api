using System;
using MongoDB.Bson.Serialization.Attributes;

namespace buddiesApi.Models
{
    public class Image
    {
        [BsonRequired]
        public int Width { get; set; }

        [BsonRequired]
        public int Height { get; set; }

        [BsonRequired]
        public string Base64 { get; set; }
    }
}

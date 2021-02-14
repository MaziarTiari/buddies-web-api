using System;
namespace buddiesApi.Models {
    public interface IMongoDbUserDocument : IMongoDbDocument {
        public string UserId { get; set; }
    }
}

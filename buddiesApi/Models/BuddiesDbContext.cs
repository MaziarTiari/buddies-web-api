namespace buddiesApi.Models
{
    public class BuddiesDbContext: IBuddiesDbContext
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string UsersCollectionName { get; set; }
        public string UserProfilesCollectionName { get; set; }
    }

    public interface IBuddiesDbContext
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string UsersCollectionName { get; set; }
        public string UserProfilesCollectionName { get; set; }
    }
}

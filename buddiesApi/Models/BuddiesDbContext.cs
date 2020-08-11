namespace buddiesApi.Models
{
    public class BuddiesDbContext: IBuddiesDbContext
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        // Collections
        public string UsersCollectionName { get; set; }
        public string UserProfilesCollectionName { get; set; }
        public string CategoriesCollectionName { get; set; }
        public string PhotoGalleriesCollectionName { get; set; }
        public string ActivitiesCollectionName {get; set; }
    }

    public interface IBuddiesDbContext
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        // Collections
        public string UsersCollectionName { get; set; }
        public string UserProfilesCollectionName { get; set; }
        public string CategoriesCollectionName { get; set; }
        public string PhotoGalleriesCollectionName { get; set; }
        public string ActivitiesCollectionName { get; set; }
    }
}

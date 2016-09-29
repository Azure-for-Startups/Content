namespace VideoSharing.DataAccess.Model
{
    using System.Data.Entity;

    public class VideoSharingDataContext : DbContext
    {
        // Your context has been configured to use a 'VideoSharingDataContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'VideoSharing.DataAccess.Model.VideoSharingDataContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'VideoSharingDataContext' 
        // connection string in the application configuration file.
        public VideoSharingDataContext()
            : base("name=VideoSharingDataContext")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<VideoElement> VideoElements { get; set; }
    }
}
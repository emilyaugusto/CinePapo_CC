using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CinePapo.Models
{
    public class SavedMovie
    {
        [BsonElement("movieId")] public int MovieId { get; set; }
        [BsonElement("title")] public string Title { get; set; } = null!;
        [BsonElement("posterPath")] public string? PosterPath { get; set; }
        [BsonElement("addedAt")] public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }

    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; } 

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("favoriteGenres")]
        public List<string> FavoriteGenres { get; set; } = new List<string>();

        [BsonElement("followersCount")]
        public int FollowersCount { get; set; } = 0;

        [BsonElement("followingCount")]
        public int FollowingCount { get; set; } = 0;

        [BsonElement("isPrivate")]
        public bool IsPrivate { get; set; } = false;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = null!;

        [BsonElement("platformGoals")]
        public List<string> PlatformGoals { get; set; } = new List<string>();

        [BsonElement("reviewCount")]
        public int ReviewCount { get; set; } = 0;

        [BsonElement("streamingPlatforms")]
        public List<string> StreamingPlatforms { get; set; } = new List<string>();

        [BsonElement("profilePicture")]
        public string? ProfilePicture { get; set; } 

        [BsonElement("username")]
        public string? Username { get; set; }

        [BsonElement("followingIds")]
        public List<string> FollowingIds { get; set; } = new List<string>();
        
        [BsonElement("watched")]
        public List<SavedMovie> Watched { get; set; } = new List<SavedMovie>();

        [BsonElement("wantToSee")]
        public List<SavedMovie> WantToSee { get; set; } = new List<SavedMovie>();

        [BsonElement("favorites")]
        public List<SavedMovie> Favorites { get; set; } = new List<SavedMovie>();

        public string? CoverPhoto { get; set; }


    }
    public class UserMovieItem
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = null!;
        public string? PosterPath { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CinePapo.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        // TEM QUE TER ESTA LINHA PARA SABER DE QUAL RESENHA É O COMENTÁRIO:
        [BsonElement("reviewId")]
        public string ReviewId { get; set; } = null!;

        [BsonElement("userId")]
        public string UserId { get; set; } = null!;

        [BsonElement("userName")]
        public string UserName { get; set; } = null!;

        [BsonElement("userPhoto")]
        public string? UserPhoto { get; set; }

        [BsonElement("content")]
        public string Content { get; set; } = null!;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [BsonIgnoreExtraElements]
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;

        [BsonElement("movieId")]
        public int MovieId { get; set; }

        [BsonElement("movieTitle")]
        public string MovieTitle { get; set; } = null!;

        [BsonElement("posterPath")]
        public string? PosterPath { get; set; }

        [BsonElement("rating")]
        public double Rating { get; set; }

        [BsonElement("content")]
        public string Content { get; set; } = null!;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- SISTEMA DE CURTIDAS E COMENTÁRIOS ---

        [BsonElement("likedByUsers")]
        public List<string> LikedByUsers { get; set; } = new List<string>();

        [BsonIgnore]
        public List<Comment> Comments { get; set; } = new List<Comment>();

        [BsonElement("comentsCount")]
        public int CommentsCount { get; set; } = 0;

        [BsonElement("likesCount")]
        public int LikesCount { get; set; } = 0;
    }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CinePapo.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // ID do usuário que VAI RECEBER a notificação
        public string UserId { get; set; } = null!;

        // ID do usuário que GEROU a notificação (ex: quem curtiu, quem comentou)
        public string? TriggeredByUserId { get; set; }

        // Nome de usuário de quem gerou a notificação
        public string? TriggeredByUsername { get; set; }

        // Tipo da notificação (LikeReview, CommentReview, Follow, etc.)
        public string Type { get; set; } = null!;

        // Conteúdo da notificação (ex: "curtiu sua resenha", "comentou sua resenha")
        public string Message { get; set; } = null!;

        // Link para onde a notificação deve levar (ex: /Review/Details/{reviewId})
        public string Link { get; set; } = null!;

        // Data e hora que a notificação foi criada
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Se a notificação foi lida ou não
        public bool IsRead { get; set; } = false;

        // Opcional: ID da resenha/comentário/etc. relacionado, para facilitar buscas
        public string? RelatedEntityId { get; set; }
    }
}

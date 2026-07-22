using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace CinePapo.Models
{
    public class TmdbResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbMovie>? Results { get; set; }
    }

    public class TmdbMovie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("runtime")]
        public int? Runtime { get; set; }

        [JsonPropertyName("vote_average")]
        public double? VoteAverage { get; set; }

        [JsonPropertyName("genres")]
        public List<TmdbGenre>? Genres { get; set; }

        [JsonPropertyName("credits")]
        public TmdbCredits? Credits { get; set; }
    }

    // Classes auxiliares para mapear as listas internas do TMDB
    public class TmdbGenre
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class TmdbCredits
    {
        [JsonPropertyName("cast")]
        public List<TmdbCastMember>? Cast { get; set; }

        [JsonPropertyName("crew")]
        public List<TmdbCrewMember>? Crew { get; set; }
    }

    public class TmdbCastMember
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class TmdbCrewMember
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("job")]
        public string? Job { get; set; }
    }
}
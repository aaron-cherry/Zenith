using System.Text.Json.Serialization;

namespace WorkoutApp.Models
{
    public class ExerciseLogDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("setNumber")]
        public int SetNumber { get; set; }

        [JsonPropertyName("completedAt")]
        public DateTime CompletedAt { get; set; }

        [JsonPropertyName("metrics")]
        public Dictionary<string, double> Metrics { get; set; } = new();
    }

    public class ExerciseLogCreateDto
    {
        [JsonPropertyName("setNumber")]
        public int SetNumber { get; set; }

        [JsonPropertyName("metrics")]
        public Dictionary<string, double> Metrics { get; set; } = new();
    }

    public class ExerciseLogUpdateDto
    {
        [JsonPropertyName("setNumber")]
        public int SetNumber { get; set; }

        [JsonPropertyName("metrics")]
        public Dictionary<string, double> Metrics { get; set; } = new();
    }
}
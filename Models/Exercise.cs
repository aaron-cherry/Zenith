using System.Text.Json.Serialization;

namespace WorkoutApp.Models
{
    public class Exercise
    {
        [JsonPropertyName("id")]
        public int ExerciseId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        public string LastPerformed { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
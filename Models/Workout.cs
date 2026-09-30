using System.Text.Json.Serialization;

namespace WorkoutApp.Models
{
    public class Workout
    {
        private List<Exercise> exercisesList = new List<Exercise>();

        [JsonPropertyName("id")]
        public int WorkoutId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        public List<Exercise> GetExercises() => exercisesList;
        public void AddExercise(Exercise exercise) => exercisesList.Add(exercise);

        public Workout() { }
    }
}
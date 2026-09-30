using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Models;

namespace WorkoutApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = NetworkService.CreateClient();
        }

        public async Task<List<Workout>> GetWorkoutsAsync()
        {
            try
            {
                var workouts = await _httpClient.GetFromJsonAsync<List<Workout>>("api/workouts");
                return workouts ?? new List<Workout>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching workouts: {ex.Message}");
                return new List<Workout>();
            }
        }

        public async Task<Workout?> CreateWorkoutAsync(string name)
        {
            try
            {
                var payload = new { Name = name };
                var response = await _httpClient.PostAsJsonAsync("api/workouts", payload);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Workout>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating workout: {ex.Message}");
            }

            return null;
        }

        public async Task<bool> DeleteWorkoutAsync(int workoutId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/workouts/{workoutId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting workout: {ex.Message}");
                return false;
            }
        }

        //Exercises
        public async Task<List<Exercise>> GetExercisesForWorkoutAsync(int workoutId)
        {
            try
            {
                var exercises = await _httpClient.GetFromJsonAsync<List<Exercise>>($"api/workouts/{workoutId}/exercises");
                return exercises ?? new List<Exercise>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching exercises: {ex.Message}");
                return new List<Exercise>();
            }
        }

        public async Task<Exercise?> AddExerciseToWorkoutAsync(int workoutId, string exerciseName)
        {
            try
            {
                var payload = new { Name = exerciseName };
                var response = await _httpClient.PostAsJsonAsync($"api/workouts/{workoutId}/exercises/", payload);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Exercise>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding exercise to workout: {ex.Message}");
            }

            return null;
        }

        // Logs/Sets

        public async Task<List<ExerciseLogDto>> GetLogsAsync(int workoutId, int exerciseId)
        {
            try
            {
                var logs = await _httpClient.GetFromJsonAsync<List<ExerciseLogDto>>($"api/workouts/{workoutId}/exercises/{exerciseId}/logs");
                return logs ?? new List<ExerciseLogDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching logs: {ex.Message}");
                return new List<ExerciseLogDto>();
            }
        }

        public async Task<ExerciseLogDto?> CreateLogAsync(int workoutId, int exerciseId, int setNumber, Dictionary<string, double> metrics)
        {
            try
            {
                var payload = new ExerciseLogCreateDto
                {
                    SetNumber = setNumber,
                    Metrics = metrics
                };
                var response = await _httpClient.PostAsJsonAsync($"api/workouts/{workoutId}/exercises/{exerciseId}/logs", payload);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ExerciseLogDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating log: {ex.Message}");
            }

            return null;
        }

        public async Task<bool> UpdateLogAsync(int workoutId, int exerciseId, int logId, int setNumber, Dictionary<string, double> metrics)
        {
            try
            {
                var payload = new ExerciseLogUpdateDto
                {
                    SetNumber = setNumber,
                    Metrics = metrics
                };

                var response = await _httpClient.PutAsJsonAsync(
                    $"api/workouts/{workoutId}/exercises/{exerciseId}/logs/{logId}", payload);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating log: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteExerciseFromWorkoutAsync(int workoutId, int exerciseId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/workouts/{workoutId}/exercises/{exerciseId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting exercise: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteLogAsync(int workoutId, int exerciseId, int logId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/workouts/{workoutId}/exercises/{exerciseId}/logs/{logId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting set log: {ex.Message}");
                return false;
            }
        }

        public async Task<Exercise?> GetExerciseAsync(int workoutId, int exerciseId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Exercise>($"api/workouts/{workoutId}/exercises/{exerciseId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching exercise: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateExerciseAsync(int workoutId, int exerciseId, string name, string note)
        {
            try
            {
                var payload = new { Name = name, Note = note };
                var response = await _httpClient.PutAsJsonAsync($"api/workouts/{workoutId}/exercises/{exerciseId}", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating exercise: {ex.Message}");
                return false;
            }
        }
    }
}

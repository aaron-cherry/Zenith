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
    }
}

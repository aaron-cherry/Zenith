using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Models;
using SQLite;

namespace WorkoutApp.DataAccess
{
    public class ExerciseWorkoutRepository
    {
        private string _dbPath;
        private SQLiteAsyncConnection conn;
        public string StatusMessage { get; set; }

        private async Task Init()
        {
            //Create table if it doesn't exist
            //if (conn != null) return;

            conn = new SQLiteAsyncConnection(_dbPath);
            await conn.CreateTableAsync<ExerciseWorkout>();
        }

        public ExerciseWorkoutRepository(string dbPath)
        {
            _dbPath = dbPath;
        }
        public async Task<List<ExerciseWorkout>> GetExerciseWorkouts()
        {
            try
            {
                await Init();

                return await conn.Table<ExerciseWorkout>().ToListAsync();

            }
            catch (Exception e)
            {
                StatusMessage = $"Failed to load data: {e.Message}";
            }

            return [];
        }

        public async Task AddNewExerciseWorkout(string exerciseName, int workoutId)
        {
            int result = 0;
            try
            {
                await Init();

                //Check Exercise table to see if the excerciseName already exists in the Exercises table
                Exercise exercise = await App.ExerciseRepository.GetExercise(exerciseName);
                //If it doesn't exist, add it to the Exercise table
                if (exercise == null)
                {
                    await App.ExerciseRepository.AddNewExercise(exerciseName);
                }
                //If it does exist, get the ExerciseId
                int exerciseId = exercise.ExerciseId;
                //Then add to exerciseId and workoutId to ExerciseWorkout table
                result = await conn.InsertAsync(new ExerciseWorkout { ExerciseId = exerciseId, WorkoutId = workoutId });
                StatusMessage = $"{result} records added (Exercise ID: {exerciseName}, Workout ID: {workoutId})";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to add {exerciseName}. Error: {ex.Message}";
            }
        }
    }
}

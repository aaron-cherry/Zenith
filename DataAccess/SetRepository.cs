using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Models;
using SQLite;

namespace WorkoutApp.DataAccess
{
    public class SetRepository
    {
        private string _dbPath;
        private SQLiteAsyncConnection conn;
        public string StatusMessage { get; set; }

        private async Task Init()
        {
            //Create table if it doesn't exist
            //if (conn != null) return;

            conn = new SQLiteAsyncConnection(_dbPath);
            await conn.CreateTableAsync<Set>();
        }

        public SetRepository(string dbPath)
        {
            _dbPath = dbPath;
        }
        public async Task<List<Set>> GetAllSets()
        {
            try
            {
                await Init();

                return await conn.Table<Set>().ToListAsync();

            }
            catch (Exception e)
            {
                StatusMessage = $"Failed to load data: {e.Message}";
            }

            return [];
        }

        public async Task<Set?> GetSet(int exerciseId, int workoutId, int setNumber)
        {
            int result = 0;
            try
            {
                await Init();
                List<Set> allSets = await App.SetRepository.GetAllSets();
                var set = allSets.FirstOrDefault(x => x.ExerciseId == exerciseId && x.WorkoutId == workoutId && x.SetNumber == setNumber);
                if (set == null)
                {
                    StatusMessage = $"Set {setNumber} not found";
                    return null;
                }
                else
                {
                    return set;
                }
            }
            catch (Exception e)
            {
                StatusMessage = $"Failed to retrieve set: {e.Message}";
                return null;
            }
        }

        public async Task AddSet(string workoutName, string exerciseName, int setNumber, int reps, double weight)
        {
            int result = 0;
            try
            {
                await Init();
                //NOTE: Existence of workout and exercise should already be verified by this point

                //Get current workout and exercise
                Exercise exercise = await App.ExerciseRepository.GetExercise(exerciseName);
                List<Workout> allWorkouts = await App.WorkoutRepository.GetWorkouts();
                Workout? workout = allWorkouts.FirstOrDefault(x => x.Name == workoutName);
                //Check Set table to see if the exerciseId, workoutId, and setNumber already exist in the Set table
                Set set = await App.SetRepository.GetSet(exercise.ExerciseId, workout.WorkoutId, setNumber);
                //If it doesn't exist, add it to the Set table
                if (set == null)
                {
                    await conn.InsertAsync(new Set { ExerciseId = exercise.ExerciseId, WorkoutId = workout.WorkoutId, SetNumber = setNumber, Reps = reps, Weight = weight });
                }
            }
            catch(Exception e)
            {
                StatusMessage = $"Failed to add set: {e.Message}";
            }
        }
    }
}

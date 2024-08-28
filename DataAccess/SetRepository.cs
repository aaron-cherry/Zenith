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

        public async Task<Set?> GetSet(int setId)
        {
            int result = 0;
            try
            {
                await Init();
                List<Set> allSets = await App.SetRepository.GetAllSets();
                var set = allSets.FirstOrDefault(x => x.SetId == setId);
                if (set == null)
                {
                    StatusMessage = $"Set {setId} not found";
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

        public async Task<Set?> GetSet(string workoutTitle, string exerciseTitle, int setNumber)
        {
            int result = 0;
            try
            {
                await Init();
                List<Set> allSets = await App.SetRepository.GetAllSets();
                Workout workout = await App.WorkoutRepository.GetWorkout(workoutTitle);
                Exercise exercise = await App.ExerciseRepository.GetExercise(exerciseTitle);
                var set = allSets.FirstOrDefault(x => x.ExerciseId == exercise.ExerciseId && x.WorkoutId == workout.WorkoutId && x.SetNumber == setNumber);
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

        public async Task AddSet(string workoutName, string exerciseName, int setNumber)
        {
            int result = 0;
            try
            {
                await Init();
                //NOTE: Existence of workout and exercise should already be verified by this point

                //Get current workout and exercise
                Exercise exercise = await App.ExerciseRepository.GetExercise(exerciseName);
                Workout workout = await App.WorkoutRepository.GetWorkout(workoutName);
                //Check Set table to see if the exerciseId, workoutId, and setNumber already exist in the Set table
                Set? set = await App.SetRepository.GetSet(workoutName, exerciseName, setNumber);
                //If it doesn't exist, add it to the Set table
                if (set == null)
                {
                    await conn.InsertAsync(new Set { ExerciseId = exercise.ExerciseId, WorkoutId = workout.WorkoutId, SetNumber = setNumber, Reps = 0, Weight = 0 });
                }
            }
            catch(Exception e)
            {
                StatusMessage = $"Failed to add set: {e.Message}";
            }
        }

        public async Task UpdateSet(int setId, double? weight, double? reps)
        {
            int result = 0;
            try
            {
                await Init();
                //Check Set table to see if the setId already exists in the Set table
                Set? set = await App.SetRepository.GetSet(setId);
                //If it doesn't exist, throw an error
                if (set == null)
                {
                    StatusMessage = $"Set {setId} not found";
                    throw new Exception($"Set {setId} not found");
                }
                else
                {
                    //If it does exist, update the reps and weight
                    if (reps != null)
                    {
                        set.Reps = reps.Value;
                    }
                    if (weight != null)
                    {
                        set.Weight = weight.Value;
                    }
                    result = await conn.UpdateAsync(set);
                    StatusMessage = $"{result} records updated (Set ID: {setId})";
                    //Also update lastPerformed for Exercise
                    List<Exercise> allExercises = await App.ExerciseRepository.GetAllExercises();
                    Exercise currentExercise = allExercises.Where(e => e.ExerciseId == set.ExerciseId).FirstOrDefault();
                    currentExercise.LastPerformed = DateTime.Now.ToShortDateString();
                    App.ExerciseRepository.UpdateExercise(currentExercise);
                }
            }
            catch (Exception e)
            {
                StatusMessage = $"Failed to update set: {e.Message}";
            }
        }

        public async Task DeleteSet(int setId)
        {
            int result = 0;
            try
            {
                await Init();
                //Check Set table to see if the setId already exists in the Set table
                Set? set = await App.SetRepository.GetSet(setId);
                //If it doesn't exist, throw an error
                if (set == null)
                {
                    StatusMessage = $"Set {setId} not found";
                    throw new Exception($"Set {setId} not found");
                }
                else
                {
                    //If it does exist, delete the set
                    result = await conn.DeleteAsync(set);
                    StatusMessage = $"{result} records deleted (Set ID: {setId})";
                }
            }
            catch (Exception e)
            {
                StatusMessage = $"Failed to delete set: {e.Message}";
            }
        }
    }
}

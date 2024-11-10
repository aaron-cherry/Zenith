using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using WorkoutApp.Models;

namespace WorkoutApp.DataAccess
{
    public class ExerciseRepository
    {
        private string _dbPath;
        private SQLiteAsyncConnection conn;
        public string StatusMessage { get; set; }

        private async Task Init()
        {
            //Create table if it doesn't exist
            if (conn != null) return;

            conn = new SQLiteAsyncConnection(_dbPath);
            await conn.CreateTableAsync<Exercise>();
        }

        public ExerciseRepository(string dbPath)
        {
            _dbPath = dbPath;
        }
        public async Task<List<Exercise>> GetAllExercises()
        {
            try
            {
                await Init();

                return await conn.Table<Exercise>().ToListAsync();

            }
            catch (Exception e)
            {
                StatusMessage = $"Failed to load data: {e.Message}";
            }

            return [];
        }

        public async Task<Exercise> GetExercise(string exerciseName)
        {
            int result = 0;
            try
            {
                Init();
                List<Exercise> allExercises = await App.ExerciseRepository.GetAllExercises();
                var exercise = allExercises.FirstOrDefault(x => x.Name == exerciseName);
                if (exercise == null)
                {
                    StatusMessage = $"Exercise {exerciseName} not found";
                    return null;
                }
                else
                {
                    return exercise;
                }
            }
            catch(Exception e)
            {
                StatusMessage = $"Error: {e.Message}";
                return null;
            }
        }

        public async Task<Exercise> GetExerciseById(int id)
        {
            int result = 0;
            try
            {
                Init();
                List<Exercise> allExercises = await App.ExerciseRepository.GetAllExercises();
                Exercise exercise = allExercises.FirstOrDefault(e => e.ExerciseId == id);
                if (exercise != null)
                {
                    return exercise;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                StatusMessage = $"Error: {e.Message}";
                return null;
            }
        }
        public async Task AddNewExercise(string exerciseName)
        {
            int result = 0;
            try
            {
                await Init();
                
                //Let's take the current workout title and add it to the ExerciseWorkout table

                if (string.IsNullOrEmpty(exerciseName)) throw new Exception("Exercise name required");
                //Check Exercise table to see if the excerciseName already exists in the Exercises table
                Exercise exercise = await GetExercise(exerciseName);
                if(exercise == null)
                {
                    //If it doesn't exist, add it to the Exercise table
                    result = await conn.InsertAsync(new Exercise { Name = exerciseName });
                    StatusMessage = $"{result} records added (Exercise: {exerciseName})";
                }
                else
                {
                    StatusMessage = $"Exercise {exerciseName} already exists";
                }

                StatusMessage = $"{result} records added (Exercise: {exerciseName})";
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", exerciseName, ex.Message);
            }
        }

        public async Task DeleteExercise(string exerciseName)
        {
            int result = 0;
            try
            {
                await Init();

                if (string.IsNullOrEmpty(exerciseName)) throw new Exception("Exercise name required");
                Exercise exercise = await GetExercise(exerciseName);
                //Delete any records in ExerciseWorkout table with exercise.Id
                await App.ExWorkRepo.DeleteExerciseWorkout(null, exercise.ExerciseId);
                //Delete exercise from Exercise table
                result = await conn.DeleteAsync(exercise);
                //Delete all sets related to exercise
                List<Set> allSets = await App.SetRepository.GetAllSets();
                foreach (Set set in allSets) {
                    if (set.ExerciseId == exercise.ExerciseId) App.SetRepository.DeleteSet(set.SetId);
                }

                StatusMessage = $"{result} records deleted (Exercise: {exerciseName})";
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete {0}. Error: {1}", exerciseName, ex.Message);
            }
        }

        public async Task UpdateExercise(Exercise newExercise)
        {
            int result = 0;
            try
            {
                await Init();

                //See if exercise doesn't exist yet
                //List<Exercise> allExercises = await App.ExerciseRepository.GetAllExercises();
                //Exercise exercise = allExercises.Where(e => e.ExerciseId == newExercise.ExerciseId).FirstOrDefault();
                Exercise exercise = await GetExerciseById(newExercise.ExerciseId);

                if (exercise != null)
                {
                    result = await conn.UpdateAsync(newExercise);
                }

                StatusMessage = $"{result} records updated (Exercise: {newExercise.Name})";
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to update {0}. Error: {1}", newExercise.Name, ex.Message);
            }
        }

    }
}

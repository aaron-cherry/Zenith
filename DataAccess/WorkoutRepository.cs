using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using WorkoutApp.Models;

namespace WorkoutApp.DataAccess;

public class WorkoutRepository
{
    private string _dbPath;
    private SQLiteAsyncConnection conn;
    public string StatusMessage { get; set; }

    private async Task Init()
    {
        //Create table if it doesn't exist
        if (conn != null) return;

        conn = new SQLiteAsyncConnection(_dbPath);
        await conn.CreateTableAsync<Workout>();            
    }

    public async Task<List<Workout>> GetWorkouts()
    {
        try
        {
            await Init();

            return await conn.Table<Workout>().ToListAsync();

        }
        catch(Exception e)
        {
            StatusMessage = $"Failed to load data: {e.Message}";
        }
        
        return [];
    }
    public async Task AddNewWorkout(string workoutName)
    {
        int result = 0;
        try
        {
            await Init();

            if (string.IsNullOrEmpty(workoutName)) throw new Exception("Workout name required");
            result = await conn.InsertAsync(new Workout { Name = workoutName });

            StatusMessage = $"{result} records added (Workout: {workoutName})";
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to add {0}. Error: {1}", workoutName, ex.Message);
        }
    }

    public async Task deleteWorkout(string workoutName)
    {
        int result = 0;
        try
        {
            await Init();

            //Check table for workout
            var workout = await conn.Table<Workout>().Where(w => w.Name == workoutName).FirstOrDefaultAsync();
            if (workout == null)
            {
                StatusMessage = $"{workoutName} not found in database";
                return;
            }

            result = await conn.Table<Workout>().Where(w => w.Name == workoutName).DeleteAsync();
            StatusMessage = $"{workoutName} deleted. Result:{result}";
        }
        catch(Exception e)
        {
            StatusMessage = $"Error: {e.Message}";
        }
    }

    public async Task updateWorkout(string workoutName)
    {
        int result = 0;
        try
        {
            await Init();

            if (string.IsNullOrEmpty(workoutName)) throw new Exception("Exercise name required");
            result = await conn.UpdateAsync(new Exercise { Name = workoutName });

            StatusMessage = $"{result} records updated (Exercise: {workoutName})";
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to update {0}. Error: {1}", workoutName, ex.Message);
        }
    }

    public WorkoutRepository(string dbPath)
    {
        _dbPath = dbPath;
    }
}

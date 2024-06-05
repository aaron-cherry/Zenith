using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace WorkoutApp.Services
{
    public class DatabaseService
    {
        private static SQLiteConnection _db;

        public DatabaseService(string dbPath)
        {
            _db = new SQLiteConnection(dbPath);
        }

        public static async Task ChangePrimaryKeyAsync()
        {

            await Task.Run(() =>
            {
                _db.Execute("PRAGMA foreign_keys=off");
                _db.BeginTransaction();
                try
                {
                    //Create migration to remove PrimaryKey constraint from ExerciseWorkout table
                    _db.Execute(@"CREATE TABLE TempExerciseWorkout(
                                  ExerciseId INTEGER,
                                  WorkoutId INTEGER,
                                  PRIMARY KEY (ExerciseId, WorkoutId));");
                    _db.Execute(@"INSERT INTO TempExerciseWorkout
                                  SELECT ExerciseId, WorkoutId
                                  FROM ExerciseWorkout;");
                    _db.Execute("DROP TABLE ExerciseWorkout");
                    _db.Execute("ALTER TABLE TempExerciseWorkout RENAME TO ExerciseWorkout");

                    _db.Commit();
                }
                catch
                {
                    _db.Rollback();
                    throw;
                }
                finally
                {
                    _db.Execute("PRAGMA foreign_keys=on");
                }
            });
        }
    }
}

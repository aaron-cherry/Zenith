using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using WorkoutApp.Pages;

namespace WorkoutApp.Models
{
    [Table("Workout")]
    public class Workout
    {
        //internal variable for exercise list property
        private List<Exercise> exercisesList = new List<Exercise>();

        //Properties
        [PrimaryKey, AutoIncrement]
        public int WorkoutId { get; set; }
        
        [MaxLength(50), Unique]
        public string Name { get; set; }

        public List<Exercise> GetExercises()
        {
            return exercisesList;
        }
        public void AddExercise(Exercise exercise)
        {
            exercisesList.Add(exercise);
        }

        //Arbritary constructor
        public Workout() { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace WorkoutApp.Models
{
    [Table("ExerciseWorkout")]
    public class ExerciseWorkout
    {
        [PrimaryKey]
        public int ExerciseId { get; set; }
        public int WorkoutId { get; set; }
    }
}

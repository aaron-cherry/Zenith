using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace WorkoutApp.Models
{
    [Table("Sets")]
    public class Set
    {
        //Properties: Reps, Weight
        [PrimaryKey, AutoIncrement]
        public int SetId { get; set; }
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int SetNumber { get; set; }
        public double Reps { get; set; }
        public double Weight { get; set; }
    }
}

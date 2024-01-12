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
        [PrimaryKey]
        public int WorkoutId { get; set; }
        [PrimaryKey]
        public int ExerciseId { get; set; }
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
    }
}

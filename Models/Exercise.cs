using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace WorkoutApp.Models
{
    [Table("Exercises")]
    public class Exercise
    {
        //Properties to create: Name, Sets, Reps, Weight
        [PrimaryKey, AutoIncrement]
        public int ExerciseId { get; set; }
        [MaxLength(50), Unique]
        public string Name { get; set; }
        public string LastPerformed { get; set; }
        private List<Set> Sets { get; set; }

        public void AddSet(Set set)
        {
            Sets.Add(set);
        }

        public List<Set> GetSets()
        {
            return Sets;
        }
        //public int? Reps { get; set; }
        //public double? Weight { get; set; }
    }
}

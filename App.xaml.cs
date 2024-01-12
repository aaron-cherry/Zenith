using WorkoutApp.Pages;
using WorkoutApp.DataAccess;

namespace WorkoutApp
{
    public partial class App : Application
    {
        public static WorkoutRepository WorkoutRepository { get; set; }
        public static ExerciseRepository ExerciseRepository { get; set; }
        public static ExerciseWorkoutRepository ExWorkRepo { get; set; }
        public App(WorkoutRepository workoutRepo, ExerciseRepository exerciseRepo, ExerciseWorkoutRepository exWorkRepo)
        {
            InitializeComponent();

            MainPage = new AppShell();

            //Initialize all repository objects with WorkoutRepository singelton object
            ExWorkRepo = exWorkRepo;
            WorkoutRepository = workoutRepo;
            ExerciseRepository = exerciseRepo;
        }
    }
}

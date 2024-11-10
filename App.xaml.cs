using WorkoutApp.Pages;
using WorkoutApp.DataAccess;
using WorkoutApp.Services;

namespace WorkoutApp
{
    public partial class App : Application
    {
        public static WorkoutRepository WorkoutRepository { get; set; }
        public static ExerciseRepository ExerciseRepository { get; set; }
        public static ExerciseWorkoutRepository ExWorkRepo { get; set; }
        public static SetRepository SetRepository { get; set; }
        public static DatabaseService DatabaseService { get; set; }

        public App(WorkoutRepository workoutRepo, ExerciseRepository exerciseRepo, ExerciseWorkoutRepository exWorkRepo, SetRepository setRepo, DatabaseService databaseService)
        {
            InitializeComponent();

            MainPage = new AppShell();

            // Initialize all repository objects with corresponding singleton object
            ExWorkRepo = exWorkRepo;
            WorkoutRepository = workoutRepo;
            ExerciseRepository = exerciseRepo;
            SetRepository = setRepo;
            DatabaseService = databaseService;

            // Call the method to change the primary key
            DatabaseService.ChangePrimaryKeyAsync();
        }
    }
}

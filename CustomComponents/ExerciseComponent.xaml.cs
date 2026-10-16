using WorkoutApp.Pages;

namespace WorkoutApp.CustomComponents;

public partial class ExerciseComponent : ContentView
{
    public int ExerciseId { get; set; }
    public int WorkoutId { get; set; }
    public string CurrentWorkoutTitle { get; set; }

    public async void ExerciseTapped(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        string exerciseTitle = exerciseName.Text;
        Routing.RegisterRoute("exercise", typeof(ExercisePage));
        await Shell.Current.GoToAsync($"exercise?exerciseId={ExerciseId}&workoutId={WorkoutId}&exerciseTitle={exerciseTitle}&workoutTitle={CurrentWorkoutTitle}");
    }

    public ExerciseComponent(int exerciseId, string dbExerciseName, int workoutId, string workoutTitle)
    {
        InitializeComponent();
        ExerciseId = exerciseId;
        WorkoutId = workoutId;
        CurrentWorkoutTitle = workoutTitle;
        exerciseName.Text = dbExerciseName;
    }
}
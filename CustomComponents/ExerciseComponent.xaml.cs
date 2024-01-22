using WorkoutApp.Pages;

namespace WorkoutApp.CustomComponents;

public partial class ExerciseComponent : ContentView
{
    public string CurrentWorkoutTitle { get; set; }
    public async void ExerciseTapped(object sender, EventArgs e)
    {
        //We need to get the current workout here
        string exerciseTitle = exerciseName.Text;
        Routing.RegisterRoute("exercise", typeof(ExercisePage));
        await Shell.Current.GoToAsync($"exercise?exerciseTitle={exerciseTitle}&workoutTitle={CurrentWorkoutTitle}");
    }
    
    public ExerciseComponent(string dbExerciseName, string workoutTitle)
	{
		InitializeComponent();
        CurrentWorkoutTitle = workoutTitle;
        exerciseName.Text = dbExerciseName;
	}
}
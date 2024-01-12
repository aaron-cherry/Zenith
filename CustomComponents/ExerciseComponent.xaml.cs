using WorkoutApp.Pages;

namespace WorkoutApp.CustomComponents;

public partial class ExerciseComponent : ContentView
{
    //public static readonly BindableProperty ExerciseNameProperty = BindableProperty.Create(nameof(exerciseName), typeof(string), typeof(ExerciseComponent), default(string));

    //public string ExerciseName1
    //{
    //    get => (string)GetValue(ExerciseNameProperty);
    //    set => SetValue(ExerciseNameProperty, value);
    //}

    public async void ExerciseTapped(object sender, EventArgs e)
    {
        string exerciseTitle = exerciseName.Text;
        Routing.RegisterRoute("exercise", typeof(ExercisePage));
        await Shell.Current.GoToAsync($"exercise?exerciseTitle={exerciseTitle}");
    }
    
    public ExerciseComponent(string dbExerciseName)
	{
		InitializeComponent();
        //exerciseName.SetBinding(Label.TextProperty, new Binding(nameof(ExerciseName), source: this));
        //exerciseName.BindingContext = this.ExerciseName;
        exerciseName.Text = dbExerciseName;
	}
}
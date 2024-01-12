using WorkoutApp.Pages;

namespace WorkoutApp.CustomComponents;

public partial class WorkoutComponent : ContentView
{
    public static readonly BindableProperty WorkoutNameProperty = BindableProperty.Create(nameof(WorkoutName), typeof(string), typeof(WorkoutComponent), default(string));

    public string WorkoutName
    {
        get => (string)GetValue(WorkoutNameProperty);
        set => SetValue(WorkoutNameProperty, value);
    }
    private async void OnWorkoutClicked(object sender, EventArgs e)
    {
        string workoutTitle = workoutName.Text;
        Routing.RegisterRoute("workout", typeof(WorkoutPage));
        await Shell.Current.GoToAsync($"workout?workoutTitle={workoutTitle}");
    }

    public WorkoutComponent()
    {
        InitializeComponent();
        workoutName.SetBinding(Label.TextProperty, new Binding(nameof(WorkoutName), source: this));
    }
}
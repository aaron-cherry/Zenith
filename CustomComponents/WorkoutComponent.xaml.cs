using WorkoutApp.Pages;

namespace WorkoutApp.CustomComponents;

public partial class WorkoutComponent : ContentView
{
    public static readonly BindableProperty WorkoutNameProperty =
        BindableProperty.Create(nameof(WorkoutName), typeof(string), typeof(WorkoutComponent), default(string));

    public static readonly BindableProperty WorkoutIdProperty =
        BindableProperty.Create(nameof(WorkoutId), typeof(int), typeof(WorkoutComponent), default(int));

    public string WorkoutName
    {
        get => (string)GetValue(WorkoutNameProperty);
        set => SetValue(WorkoutNameProperty, value);
    }

    public int WorkoutId
    {
        get => (int)GetValue(WorkoutIdProperty);
        set => SetValue(WorkoutIdProperty, value);
    }

    private async void OnWorkoutClicked(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        Routing.RegisterRoute("workout", typeof(WorkoutPage));
        await Shell.Current.GoToAsync($"workout?workoutId={WorkoutId}&workoutTitle={WorkoutName}");
    }

    public WorkoutComponent()
    {
        InitializeComponent();
        workoutName.SetBinding(Label.TextProperty, new Binding(nameof(WorkoutName), source: this));
    }
}
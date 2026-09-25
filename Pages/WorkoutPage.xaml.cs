using System.Web;
using WorkoutApp.CustomComponents;
using WorkoutApp.Models;
using WorkoutApp.Services;

namespace WorkoutApp.Pages;

[QueryProperty(nameof(WorkoutId), "workoutId")]
[QueryProperty(nameof(WorkoutTitle), "workoutTitle")]
public partial class WorkoutPage : ContentPage, IQueryAttributable
{
    private readonly ApiService _apiService;
    public int WorkoutId { get; set; }
    public string? WorkoutTitle { get; set; }

    public WorkoutPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        BindingContext = this;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("workoutId", out var idObj) && int.TryParse(idObj.ToString(), out int parsedId))
        {
            WorkoutId = parsedId;
        }

        if (query.TryGetValue("workoutTitle", out var titleObj))
        {
            WorkoutTitle = HttpUtility.UrlDecode(titleObj.ToString());
            workoutTitle.Text = WorkoutTitle;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await DisplayExercisesAsync();
    }

    private async Task DisplayExercisesAsync()
    {
        exerciseGrid.Clear();
        exerciseGrid.RowDefinitions.Clear();

        if (WorkoutId <= 0) return;

        var exercises = await _apiService.GetExercisesForWorkoutAsync(WorkoutId);

        foreach (var exercise in exercises)
        {
            RowDefinition newExerciseRow = new RowDefinition { Height = GridLength.Auto };
            exerciseGrid.RowDefinitions.Add(newExerciseRow);
            int lastRow = exerciseGrid.RowDefinitions.Count - 1;

            ExerciseComponent exerciseComponent = new ExerciseComponent(exercise.Name, WorkoutTitle);
            exerciseGrid.Add(exerciseComponent, 0, lastRow);
        }
    }

    private async void Entry_Completed(object sender, EventArgs e)
    {
        string exerciseName = exerciseEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(exerciseName))
        {
            await DisplayAlert("Error", "Exercise name cannot be blank", "OK");
            return;
        }

        var created = await _apiService.AddExerciseToWorkoutAsync(WorkoutId, exerciseName);
        if (created != null)
        {
            statusMessageLabel.Text = $"Added {created.Name}";
            await DisplayExercisesAsync();
        }
        else
        {
            statusMessageLabel.Text = "Failed to add exercise.";
        }

        exerciseEntry.Text = string.Empty;
        exerciseEntry.Focus();
    }

    private async void deleteWorkoutClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Delete Workout", "Are you sure you want to delete this workout?", "Yes", "No");
        if (!answer) return;

        bool success = await _apiService.DeleteWorkoutAsync(WorkoutId);
        if (success)
        {
            await DisplayAlert("Success", "Workout deleted", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await DisplayAlert("Error", "Failed to delete workout.", "OK");
        }
    }
}
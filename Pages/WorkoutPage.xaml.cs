using System.Web;
using WorkoutApp.CustomComponents;
using WorkoutApp.Models;
using WorkoutApp.Services;

namespace WorkoutApp.Pages;

[QueryProperty(nameof(WorkoutId), "workoutId")]
[QueryProperty(nameof(WorkoutTitle), "workoutTitle")]
public partial class WorkoutPage : ContentPage, IQueryAttributable
{
    private readonly ApiService _apiService = new ApiService();
    public int WorkoutId { get; set; }
    public string? WorkoutTitle { get; set; }

    private List<Exercise> _allAvailableExercises = new();

    public WorkoutPage()
    {
        InitializeComponent();
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
        await LoadAllExercisesForSuggestionsAsync();
    }

    private async Task LoadAllExercisesForSuggestionsAsync()
    {
        _allAvailableExercises = await _apiService.GetAllExercisesAsync();
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

            ExerciseComponent exerciseComponent = new ExerciseComponent(exercise.ExerciseId, exercise.Name, WorkoutId, WorkoutTitle ?? string.Empty);
            exerciseGrid.Add(exerciseComponent, 0, lastRow);
        }
    }

    private void OnExerciseTextChanged(object sender, TextChangedEventArgs e)
    {
        string query = e.NewTextValue?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(query))
        {
            suggestionsBorder.IsVisible = false;
            suggestionsView.ItemsSource = null;
            return;
        }

        var matches = _allAvailableExercises
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matches.Count > 0)
        {
            suggestionsView.ItemsSource = matches;
            suggestionsBorder.IsVisible = true;
        }
        else
        {
            suggestionsBorder.IsVisible = false;
            suggestionsView.ItemsSource = null;
        }
    }

    private async void OnSuggestionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Exercise selected)
        {
            suggestionsBorder.IsVisible = false;
            suggestionsView.SelectedItem = null;

            // Submit selected exercise directly
            await AddExerciseByNameAsync(selected.Name);
        }
    }

    private async void Entry_Completed(object sender, EventArgs e)
    {
        string exerciseName = exerciseEntry.Text?.Trim() ?? string.Empty;
        suggestionsBorder.IsVisible = false;

        if (string.IsNullOrWhiteSpace(exerciseName))
        {
            await DisplayAlert("Error", "Exercise name cannot be blank", "OK");
            return;
        }

        await AddExerciseByNameAsync(exerciseName);
    }

    private async Task AddExerciseByNameAsync(string name)
    {
        var created = await _apiService.AddExerciseToWorkoutAsync(WorkoutId, name);
        if (created != null)
        {
            statusMessageLabel.Text = $"Added {created.Name}";
            await DisplayExercisesAsync();
            await LoadAllExercisesForSuggestionsAsync(); // Keep suggestions updated
        }
        else
        {
            statusMessageLabel.Text = "Failed to add exercise.";
        }

        exerciseEntry.Text = string.Empty;
        exerciseEntry.Unfocus();
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
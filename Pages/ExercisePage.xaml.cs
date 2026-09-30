using System.Web;
using WorkoutApp.CustomComponents;
using WorkoutApp.Models;
using WorkoutApp.Services;

namespace WorkoutApp.Pages;

[QueryProperty(nameof(ExerciseId), "exerciseId")]
[QueryProperty(nameof(WorkoutId), "workoutId")]
[QueryProperty(nameof(ExerciseTitle), "exerciseTitle")]
[QueryProperty(nameof(WorkoutTitle), "workoutTitle")]
public partial class ExercisePage : ContentPage, IQueryAttributable
{
    private readonly ApiService _apiService;

    public int ExerciseId { get; set; }
    public int WorkoutId { get; set; }
    public string? ExerciseTitle { get; set; }
    public string? WorkoutTitle { get; set; }

    public ExercisePage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        BindingContext = this;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("exerciseId", out var exId) && int.TryParse(exId.ToString(), out int parsedExId))
            ExerciseId = parsedExId;

        if (query.TryGetValue("workoutId", out var wId) && int.TryParse(wId.ToString(), out int parsedWId))
            WorkoutId = parsedWId;

        if (query.TryGetValue("exerciseTitle", out var exTitle))
        {
            ExerciseTitle = HttpUtility.UrlDecode(exTitle.ToString());
            exerciseTitle.Text = ExerciseTitle;
        }

        if (query.TryGetValue("workoutTitle", out var wTitle))
        {
            WorkoutTitle = HttpUtility.UrlDecode(wTitle.ToString());
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await DisplaySetsAsync();
        await LoadExerciseDetailsAsync();
    }

    private async Task LoadExerciseDetailsAsync()
    {
        if (WorkoutId <= 0 || ExerciseId <= 0) return;

        var exercise = await _apiService.GetExerciseAsync(WorkoutId, ExerciseId);
        if (exercise != null)
        {
            exerciseNote.Text = exercise.Note;
        }
    }

    private async Task DisplaySetsAsync()
    {
        setGrid.Clear();
        setGrid.RowDefinitions.Clear();

        if (WorkoutId <= 0 || ExerciseId <= 0) return;

        var logs = await _apiService.GetLogsAsync(WorkoutId, ExerciseId);

        foreach (var log in logs)
        {
            RowDefinition newSetRow = new RowDefinition { Height = GridLength.Auto };
            setGrid.RowDefinitions.Add(newSetRow);

            int lastRow = setGrid.RowDefinitions.Count - 1;

            var newSetLabel = new Label
            {
                Text = $"Set {log.SetNumber}",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 10
            };
            setGrid.Add(newSetLabel, 0, lastRow);

            // Pull reps and weight from JSONB dictionary (defaulting to 0 if absent)
            log.Metrics.TryGetValue("weight", out double weight);
            log.Metrics.TryGetValue("reps", out double reps);

            var setComponent = new SetComponent(log.Id, WorkoutId, ExerciseId, log.SetNumber, weight, reps, lastRow);
            setComponent.SetChanged += OnSetChanged;
            setComponent.SetDeleted += async (s, e) => await DisplaySetsAsync();
            setGrid.Add(setComponent, 1, lastRow);
        }

        lastPerformedLabel.Text = logs.Count > 0
            ? $"Last logged: {logs.Max(l => l.CompletedAt).ToLocalTime():MM/dd/yyyy}"
            : "No sets recorded yet";
    }

    private void OnSetChanged(object? sender, EventArgs e)
    {
        // Updates label timestamp when a set finishes saving
        lastPerformedLabel.Text = $"Last logged: {DateTime.Now:MM/dd/yyyy}";
    }

    public async void OnAddSetButtonClicked(object sender, EventArgs e)
    {
        int nextSetNumber = setGrid.RowDefinitions.Count + 1;

        // Default set metrics dictionary
        var initialMetrics = new Dictionary<string, double>
        {
            { "weight", 0 },
            { "reps", 0 }
        };

        var createdLog = await _apiService.CreateLogAsync(WorkoutId, ExerciseId, nextSetNumber, initialMetrics);

        if (createdLog != null)
        {
            await DisplaySetsAsync();
        }
        else
        {
            await DisplayAlert("Error", "Failed to add set to database.", "OK");
        }
    }

    private async void OnDeleteExButtonClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert(
            "Delete Exercise",
            $"Are you sure you want to remove {ExerciseTitle} from this workout?",
            "Yes",
            "No");

        if (!answer) return;

        if (WorkoutId > 0 && ExerciseId > 0)
        {
            bool success = await _apiService.DeleteExerciseFromWorkoutAsync(WorkoutId, ExerciseId);
            if (success)
            {
                await DisplayAlert("Success", "Exercise removed.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert("Error", "Failed to delete exercise from server.", "OK");
            }
        }
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (WorkoutId <= 0 || ExerciseId <= 0) return;

        string content = exerciseNote.Text?.Trim() ?? string.Empty;
        bool success = await _apiService.UpdateExerciseAsync(WorkoutId, ExerciseId, ExerciseTitle ?? string.Empty, content);

        if (success)
        {
            await DisplayAlert("Saved", "Note saved successfully.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Failed to save note to server.", "OK");
        }
    }
}
using WorkoutApp.Services;

namespace WorkoutApp.CustomComponents;

public partial class SetComponent : ContentView
{
    private readonly ApiService _apiService = new ApiService();

    public EventHandler? SetChanged;
    public event EventHandler? SetDeleted;

    public int LogId { get; set; }
    public int WorkoutId { get; set; }
    public int ExerciseId { get; set; }
    public int SetNumber { get; set; }
    private int GridRow { get; set; }

    public SetComponent(int logId, int workoutId, int exerciseId, int setNumber, double weight, double reps, int gridRow)
    {
        InitializeComponent();

        LogId = logId;
        WorkoutId = workoutId;
        ExerciseId = exerciseId;
        SetNumber = setNumber;
        GridRow = gridRow;

        setIdLabel.Text = logId.ToString();
        weightEntry.Text = weight.ToString();
        repsEntry.Text = reps.ToString();
    }

    public SetComponent()
    {
        InitializeComponent();
    }

    public async void WeightEntryCompleted(object sender, EventArgs e)
    {
        await SaveSetAsync();
        repsEntry.Focus();
    }

    public void OnWeightEntryUnfocused(object sender, EventArgs e)
    {
        WeightEntryCompleted(sender, e);
    }

    public async void RepsEntryCompleted(object sender, EventArgs e)
    {
        await SaveSetAsync();
        SetChanged?.Invoke(this, EventArgs.Empty);
    }

    public void OnRepsEntryUnfocused(object sender, EventArgs e)
    {
        RepsEntryCompleted(sender, e);
    }

    private async Task SaveSetAsync()
    {
        double.TryParse(weightEntry.Text, out double weight);
        double.TryParse(repsEntry.Text, out double reps);

        var metrics = new Dictionary<string, double>
        {
            { "weight", weight },
            { "reps", reps }
        };

        if (WorkoutId > 0 && ExerciseId > 0 && LogId > 0)
        {
            await _apiService.UpdateLogAsync(WorkoutId, ExerciseId, LogId, SetNumber, metrics);
        }
    }

    public async void DeleteSetButtonClicked(object sender, EventArgs e)
    {
        if (Application.Current?.MainPage == null) return;

        bool answer = await Application.Current.MainPage.DisplayAlert(
            "Delete Set",
            $"Are you sure you want to delete Set {SetNumber}?",
            "Yes",
            "No");

        if (!answer) return;

        if (WorkoutId > 0 && ExerciseId > 0 && LogId > 0)
        {
            bool success = await _apiService.DeleteLogAsync(WorkoutId, ExerciseId, LogId);
            if (success)
            {
                SetDeleted?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete set from server.", "OK");
            }
        }
    }
}
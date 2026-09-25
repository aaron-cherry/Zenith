using Android.Views;
using WorkoutApp.CustomComponents;
using WorkoutApp.DataAccess;
using WorkoutApp.Models;
using WorkoutApp.Services;

namespace WorkoutApp.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService = new ApiService();
	public HomePage()
	{
		InitializeComponent();
        _apiService = new ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        //Db access and retrieve list
        await LoadWorkoutsAsync();
    }

    private async Task LoadWorkoutsAsync()
    {
        try
        {
            workoutStackLayout.Children.Clear();
            var allWorkouts = await _apiService.GetWorkoutsAsync();

            foreach (var workout in allWorkouts)
            {
                WorkoutComponent workoutComponent = new WorkoutComponent() { WorkoutName = workout.Name };
                workoutStackLayout.Add(workoutComponent);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not load workouts: {ex.Message}", "Ok");
        }
    }

    private void OnAddWorkoutButtonClicked(object sender, EventArgs e)
    {
        workoutEntry.IsVisible = true;
        workoutEntry.Focus();
    }

    private async void OnEntryCompleted(object sender, EventArgs e)
    {
        string workoutTitle = ((Entry)sender).Text.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(workoutTitle))
        {
            workoutEntry.IsVisible = false;
            await DisplayAlert("Error", "Workout title cannot be blank", "Ok");
            return;
        }

        //Add workout to db
        var createdWorkout = await _apiService.CreateWorkoutAsync(workoutTitle);

        //create new workout component and add it to the UI/register the page route
        if (createdWorkout != null)
        {
            var workoutComponent = new WorkoutComponent { WorkoutName = createdWorkout.Name };
            workoutStackLayout.Add(workoutComponent);

            Routing.RegisterRoute("workout", typeof(WorkoutPage));
            await Shell.Current.GoToAsync($"workout?workoutTitle={workoutTitle}");
        }
        
        workoutEntry.IsVisible = false;
        workoutTitleEntry.Text = string.Empty;
    }

    private void workoutTitleEntryUnfocused(object sender, FocusEventArgs e)
    {
        workoutEntry.IsVisible = false;
    }
}
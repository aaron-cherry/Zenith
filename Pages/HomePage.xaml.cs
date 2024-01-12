using WorkoutApp.Models;
using WorkoutApp.DataAccess;
using WorkoutApp.CustomComponents;

namespace WorkoutApp.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        //Db access and retrieve list
        try
        {
            workoutStackLayout.Children.Clear();
            List<Workout> allWorkouts = new List<Workout>();
            allWorkouts = await App.WorkoutRepository.GetWorkouts();

            //Check if workouts have already been loaded
            if (workoutStackLayout.Children.Count > 0) return;
            
            foreach (Workout workout in allWorkouts)
            {
                WorkoutComponent workoutComponent = new WorkoutComponent() { WorkoutName = workout.Name };
                workoutStackLayout.Add(workoutComponent);
            }
        }
        catch(Exception e)
        {
            await DisplayAlert("Exception at page load", e.Message, "Ok");
        }

    }

    private void OnAddWorkoutButtonClicked(object sender, EventArgs e)
    {
        workoutEntry.IsVisible = true;
        workoutEntry.Focus();
    }

    private async void OnEntryCompleted(object sender, EventArgs e)
    {
        string workoutTitle = ((Entry)sender).Text;

        //Add workout to db
        await App.WorkoutRepository.AddNewWorkout(workoutTitle);
        string statusMessage = App.WorkoutRepository.StatusMessage;
        DisplayAlert("Status", statusMessage, "Ok");

        //Create new workoutcomponent and add it
        WorkoutComponent workoutComponent = new WorkoutComponent() { WorkoutName = workoutTitle };
        workoutStackLayout.Add(workoutComponent);
        
        Routing.RegisterRoute("workout", typeof(WorkoutPage));
        await Shell.Current.GoToAsync($"workout?workoutTitle={workoutTitle}");
        workoutEntry.IsVisible = false;

        workoutTitleEntry.Text = "";
        workoutTitleEntry.Focus();
    }

    private void workoutTitleEntryUnfocused(object sender, FocusEventArgs e)
    {
        workoutEntry.IsVisible = false;
    }
}
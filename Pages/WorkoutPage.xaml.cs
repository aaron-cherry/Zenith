using System.Web;
using WorkoutApp.DataAccess;
using WorkoutApp.Models;
using WorkoutApp.CustomComponents;
using SQLite;
using Microsoft.Maui.Controls;

namespace WorkoutApp.Pages;

[QueryProperty(nameof(WorkoutTitle), "workoutTitle")]
public partial class WorkoutPage : ContentPage, IQueryAttributable
{
	public string? WorkoutTitle { get; set; }
    private Workout workout;
	public WorkoutPage()
	{
		InitializeComponent();
        BindingContext = this;
        workout = new Workout();
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        WorkoutTitle = HttpUtility.UrlDecode(query["workoutTitle"].ToString());
        // Use LabelText here
        workoutTitle.Text = WorkoutTitle;

        // Create a new Workout object
        workout = new Workout
        {
            Name = WorkoutTitle
        };
    }

    protected override async void OnAppearing()
    {
        try
        {
            DisplayExercises();
        }
        catch (Exception e)
        {
            await DisplayAlert("Database Error", $"{e.Message}", "Ok");
        }
        base.OnAppearing();

    }
    private async void DisplayExercises()
    {
        exerciseGrid.Clear();
        string path = FileAccessHelper.GetLocalFilePath("zenith.db3");
        List<ExerciseWorkout> allExerciseWorkouts;
        List<Exercise> allExercises = await App.ExerciseRepository.GetAllExercises();
        List<Exercise> currentWorkoutExercises = new List<Exercise>();
        allExerciseWorkouts = await App.ExWorkRepo.GetExerciseWorkouts();
        List<Workout> workouts = await App.WorkoutRepository.GetAllWorkouts();

        //Get current workoutId
        int workoutId = workouts.Where(w => w.Name == WorkoutTitle).Select(w => w.WorkoutId).FirstOrDefault();
        //If WorkoutTitle is null, then display all exercises
        if (WorkoutTitle == null)
        {
            this.Title = "All Exercises";
            foreach (Exercise exercise in allExercises)
            {
                RowDefinition newExerciseRow = new RowDefinition { Height = GridLength.Auto };
                int lastRow = exerciseGrid.RowDefinitions.Count - 1;
                ExerciseComponent exerciseComponent = new ExerciseComponent(exercise.Name, WorkoutTitle);
                //create new row definition
                exerciseGrid.RowDefinitions.Add(newExerciseRow);
                exerciseGrid.Add(exerciseComponent, 0, lastRow);
            }
            return;
        }
        //Get list of exercises associated with the Id of current workout
        List<ExerciseWorkout> filteredWorkoutExercises = allExerciseWorkouts.Where(e => e.WorkoutId == workoutId).ToList();

        //Display all exercises associated with the current workout as ExerciseComponents
        foreach (ExerciseWorkout exercise in filteredWorkoutExercises)
        {
            int exerciseId = exercise.ExerciseId;
            Exercise currentExercise = allExercises.Where(e => e.ExerciseId == exerciseId).FirstOrDefault();
            currentWorkoutExercises.Add(currentExercise);
        }

        foreach (Exercise exercise in currentWorkoutExercises)
        {
            RowDefinition newExerciseRow = new RowDefinition { Height = GridLength.Auto };
            int lastRow = exerciseGrid.RowDefinitions.Count - 1;
            ExerciseComponent exerciseComponent = new ExerciseComponent(exercise.Name, WorkoutTitle);
            //create new row definition
            exerciseGrid.RowDefinitions.Add(newExerciseRow);
            exerciseGrid.Add(exerciseComponent, 0, lastRow);
        }
    }

    private async void Entry_Completed(object sender, EventArgs e)
    {
        string exerciseName = exerciseEntry.Text;
        
        // Add exercise to the workout object
        if (exerciseName == null)
        {
            DisplayAlert("Error", "Exercise name cannot be blank", "OK");
        }
        else
        {
            Exercise exercise = new Exercise { Name = exerciseName };
            workout.AddExercise(exercise);
        }

        //Let's add the exercise to the ExerciseWorkout table
        await AddExerciseToDb(exerciseEntry.Text);
        statusMessageLabel.Text = $"{App.ExWorkRepo.StatusMessage}";
        DisplayExercises();

        //Refocus back to exerciseEntry and clear text
        exerciseEntry.Text = "";
        exerciseEntry.Focus();
    }

    private async Task AddExerciseToDb(string exTitle)
    {
        try
        {
            //await App.ExerciseRepository.AddNewExercise(exTitle);

            //Get id's of current workout and exercise
            List<Workout> workouts = await App.WorkoutRepository.GetAllWorkouts();
            int workoutId = workouts.Where(w => w.Name == WorkoutTitle).Select(w => w.WorkoutId).FirstOrDefault();
            
            //Add to corresponding dbs
            await App.ExWorkRepo.AddNewExerciseWorkout(exTitle, workoutId);

            statusMessageLabel.Text = $"{App.ExWorkRepo.StatusMessage}";
        }
        catch(Exception e)
        {
            await DisplayAlert("DbError", $"{e.Message}", "Ok");
        }
    }


    private async void deleteWorkoutClicked(object sender, EventArgs e)
    {
        //Ask user if they are sure they want to delete the workout
        bool answer = await DisplayAlert("Delete Workout", "Are you sure you want to delete this workout?", "Yes", "No");
        if (!answer) return;

        await App.WorkoutRepository.deleteWorkout(workoutTitle.Text);
        DisplayAlert("Delete Button", $"{App.WorkoutRepository.StatusMessage}", "Cancel");
        //Go to previous page
        await Shell.Current.GoToAsync("..");
    }
}
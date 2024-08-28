using System.Web;
using WorkoutApp.Models;
using WorkoutApp.CustomComponents;

namespace WorkoutApp.Pages;

[QueryProperty(nameof(ExerciseTitle), "exerciseTitle")]
[QueryProperty(nameof(WorkoutTitle), "workoutTitle")]
public partial class ExercisePage : ContentPage, IQueryAttributable
{
    public string? ExerciseTitle { get; set; }
    public string? WorkoutTitle { get; set; }
    private static List<Set> allSets;
    public ExercisePage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ExerciseTitle = HttpUtility.UrlDecode(query["exerciseTitle"].ToString());
        WorkoutTitle = HttpUtility.UrlDecode(query["workoutTitle"].ToString());
        // Use LabelText here
        exerciseTitle.Text = ExerciseTitle;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            DisplaySets();
        }
        catch (Exception e)
        {
            DisplayAlert("Database Error", $"{e.Message}", "Ok");
        }
    }

    private async void DisplaySets()
    {
        setGrid.Clear();
        string path = FileAccessHelper.GetLocalFilePath("zenith.db3");
        allSets = await App.SetRepository.GetAllSets();
        List<Set> currentExerciseSets = new List<Set>();
        List<Exercise> allExercises = await App.ExerciseRepository.GetAllExercises();

        //Get current exerciseId
        int exerciseId = allExercises.Where(e => e.Name == ExerciseTitle).Select(e => e.ExerciseId).FirstOrDefault();
        //Get list of sets associated with the Id of current exercise
        List<Set> filteredExerciseSets = allSets.Where(s => s.ExerciseId == exerciseId).ToList();

        //Order sets by setNumber
        filteredExerciseSets = filteredExerciseSets.OrderBy(s => s.SetNumber).ToList();

        //Add all sets associated with current exercise to setGrid
        foreach (Set set in filteredExerciseSets)
        {
            //Create new row definition
            RowDefinition newSetRow = new RowDefinition { Height = GridLength.Auto };
            //Add new row definition to grid
            setGrid.RowDefinitions.Add(newSetRow);
            //Create new set label based on number of rows in grid
            var newSetLabel = new Label
            {
                Text = $"Set {setGrid.RowDefinitions.Count}",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 10
            };



            //Find last row in setGrid and add newSetLabel to the first column
            int lastRow = setGrid.RowDefinitions.Count - 1;
            //Create new set label based on number of rows in grid
            setGrid.Add(newSetLabel, 0, lastRow);

            //Add customComponents.SetComponent to column 1 in new row
            SetComponent setComponent = new SetComponent(set.SetId, set.Weight, set.Reps, setGrid.RowDefinitions.Count);
            setComponent.SetChanged += OnSetChanged;
            setGrid.Add(setComponent, 1, lastRow);
        }
        Exercise currentExercise = allExercises.Where(e => e.ExerciseId == exerciseId).FirstOrDefault();
        lastPerformedLabel.Text = $"Last Performed: {currentExercise.LastPerformed}";
    }

    public async void OnSetChanged(object sender, EventArgs e)
    {
        List<Exercise> allExercises = await App.ExerciseRepository.GetAllExercises();
        Exercise currentExercise = allExercises.Where(e => e.Name == ExerciseTitle).FirstOrDefault();
        //Figure out why this isnt updating the 
        lastPerformedLabel.Text = $"Last Performed: {currentExercise.LastPerformed}";
    }

    public async void OnAddSetButtonClicked(object sender, EventArgs e)
    {
        //Create new row definition
        RowDefinition newSetRow = new RowDefinition { 
            Height = GridLength.Auto };
        //Add new row definition to grid
        setGrid.RowDefinitions.Add(newSetRow);

        //Create new set label based on number of rows in grid
        var newSetLabel = new Label
        {
            Text = $"Set {setGrid.RowDefinitions.Count}",
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Margin = 10
        };
        //Find last row in setGrid and add newSetLabel to the first column
        int lastRow = setGrid.RowDefinitions.Count - 1;
        setGrid.Add(newSetLabel, 0, lastRow);

        //Add customComponents.SetComponent to column 1 in new row
        await App.SetRepository.AddSet(WorkoutTitle, ExerciseTitle, setGrid.RowDefinitions.Count);
        var set = await App.SetRepository.GetSet(WorkoutTitle, ExerciseTitle, setGrid.RowDefinitions.Count);
        SetComponent setComponent = new SetComponent(set.SetId, 0, 0, lastRow);
        setComponent.SetChanged += OnSetChanged;
        setGrid.Add(setComponent, 1, lastRow);
    }

    private async void OnDeleteExButtonClicked(object sender, EventArgs e)
    {
        //Ask user if they are sure they want to delete the exercise
        bool answer = await DisplayAlert("Delete Exercise", "Are you sure you want to delete this exercise?", "Yes", "No");
        if (!answer) return;
        //Edit deleteExercise method to search ExWorkout table for exerciseId and delete all rows with that exerciseId
        await App.ExerciseRepository.DeleteExercise(exerciseTitle.Text);
        await Navigation.PopAsync();
        DisplayAlert("Exercise Deleted", $"{App.ExerciseRepository.StatusMessage}", "Ok");
    }
}
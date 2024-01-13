using System.Web;
using WorkoutApp.Models;
using WorkoutApp.CustomComponents;

namespace WorkoutApp.Pages;

[QueryProperty(nameof(ExerciseTitle), "exerciseTitle")]
public partial class ExercisePage : ContentPage, IQueryAttributable
{
    public string? ExerciseTitle { get; set; }
    public ExercisePage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ExerciseTitle = HttpUtility.UrlDecode(query["exerciseTitle"].ToString());
        // Use LabelText here
        exerciseTitle.Text = ExerciseTitle;
    }

    public void OnAddSetButtonClicked(object sender, EventArgs e)
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
        SetComponent setComponent = new SetComponent();
        setGrid.Add(setComponent, 1, lastRow);
    }

    private async void OnDeleteExButtonClicked(object sender, EventArgs e)
    {
        //Edit deleteExercise method to search ExWorkout table for exerciseId and delete all rows with that exerciseId
        await App.ExerciseRepository.DeleteExercise(exerciseTitle.Text);
        DisplayAlert("Exercise Deleted", $"{App.ExerciseRepository.StatusMessage}", "Ok");
    }
}
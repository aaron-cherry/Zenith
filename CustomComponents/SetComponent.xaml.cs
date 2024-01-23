using WorkoutApp.Models;

namespace WorkoutApp.CustomComponents;

public partial class SetComponent : ContentView
{
    private int GridRow { get; set; }
	public SetComponent(int setId, double weight,double reps, int gridRow)
	{
		InitializeComponent();
        setIdLabel.Text = setId.ToString();
        weightEntry.Text = weight.ToString();
        repsEntry.Text = reps.ToString();
        GridRow = gridRow;
	}

    public SetComponent()
    {
        InitializeComponent();
    }


    public async void WeightEntryCompleted(object sender, EventArgs e)
    {
        await App.SetRepository.UpdateSet(int.Parse(setIdLabel.Text), double.Parse(weightEntry.Text), null);
        repsEntry.Focus();
    }

    public async void OnWeightEntryUnfocused(object sender, EventArgs e)
    {
        WeightEntryCompleted(sender, e);
    }

    public async void RepsEntryCompleted(object sender, EventArgs e)
    {
        await App.SetRepository.UpdateSet(int.Parse(setIdLabel.Text), null, double.Parse(repsEntry.Text));
    }

    public async void OnRepsEntryUnfocused(object sender, EventArgs e)
    {
        RepsEntryCompleted(sender, e);
    }

	public async void DeleteSetButtonClicked(object sender, EventArgs e)
	{
        try
        {
            //Prepare your eyes for some really hacky UI shit 
            List<Set> allSets = await App.SetRepository.GetAllSets();
            Set? set = allSets.FirstOrDefault(x => x.SetId == int.Parse(setIdLabel.Text));
            int gridIndex = set.SetNumber - 1;
            if (set == null) return;

            // Get the parent of the button (the StackLayout)
            HorizontalStackLayout parent = (HorizontalStackLayout)((Button)sender).Parent;
            // Get the parent of the StackLayout (the Grid)
            VerticalStackLayout parentVSL = (VerticalStackLayout)parent.Parent;
            SetComponent parentSC = (SetComponent)parentVSL.Parent;
            Grid parentGrid = (Grid)parentSC.Parent;

            int setRow = parentGrid.GetRow(parentSC);
            foreach (var child in parentGrid.Children.ToList())
            {
                int childRow = parentGrid.GetRow(child);
                if (childRow == setRow)
                {
                    parentGrid.Children.Remove(child);
                    //break;
                }
            }
            parentGrid.RowDefinitions.RemoveAt(gridIndex);

            //Delete set from database
            await App.SetRepository.DeleteSet(set.SetId);
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
        }
    }
}
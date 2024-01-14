namespace WorkoutApp.CustomComponents;

public partial class SetComponent : ContentView
{
	public SetComponent()
	{
		InitializeComponent();
	}

	public void DeleteSetButtonClicked(object sender, EventArgs e)
	{
        // Get the parent of the button (the StackLayout)
        HorizontalStackLayout parent = (HorizontalStackLayout)((Button)sender).Parent;
        // Get the parent of the StackLayout (the Grid)
        VerticalStackLayout vStackLayout = (VerticalStackLayout)parent.Parent;
        // Remove the StackLayout from the Grid
        vStackLayout.Children.Remove(parent);
    }
}
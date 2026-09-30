using WorkoutApp.Pages;

namespace WorkoutApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            homeItem.Route = $"{nameof(HomePage)}";
        }
    }
}
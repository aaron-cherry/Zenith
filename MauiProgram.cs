using Microsoft.Extensions.Logging;
using WorkoutApp.DataAccess;

namespace WorkoutApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            //Db access
            string dbPath = FileAccessHelper.GetLocalFilePath("zenith.db3");
            builder.Services.AddSingleton<WorkoutRepository>(s => ActivatorUtilities.CreateInstance<WorkoutRepository>(s, dbPath));
            builder.Services.AddSingleton<ExerciseRepository>(s => ActivatorUtilities.CreateInstance<ExerciseRepository>(s, dbPath));
            builder.Services.AddSingleton<ExerciseWorkoutRepository>(s => ActivatorUtilities.CreateInstance<ExerciseWorkoutRepository>(s, dbPath));

            return builder.Build();
        }
    }
}

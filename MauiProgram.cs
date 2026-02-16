using Firebase.Database;
using Microsoft.Extensions.Logging;
using Week3_L1.Services;

using Week3_L1.ViewModels;
using Week3_L1.Views;
namespace Week3_L1
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
            string firebase_App_url = "your-db-url";

            var firebaseClient = new FirebaseClient(firebase_App_url);
            builder.Services.AddSingleton(firebaseClient);

            // Register Firebase service
            //builder.Services.AddSingleton<IStudentServices, FirebaseStudentService>();

            builder.Services.AddSingleton<IStudentServices, StudentService>();

            builder.Services.AddTransient<StartPageViewModel>();
            builder.Services.AddTransient<StudentViewModel>();
            builder.Services.AddTransient<StartPage>();
            builder.Services.AddTransient<StudentView>();
            return builder.Build();
        }
    }
}

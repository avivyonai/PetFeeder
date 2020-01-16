using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PetFeeder
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // register the dependencies in the same
            Xamarin.Forms.DependencyService.Register<DeviceInfo>();

            if (Application.Current.Properties.ContainsKey("RegisteredId"))
            {
                MainPage = new MainPage();
            }
            else
                MainPage = new StartPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

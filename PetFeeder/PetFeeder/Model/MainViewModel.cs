using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
//using Microsoft.Azure.Devices;
using System.Text;

namespace PetFeeder
{
    public class MainViewModel : INotifyPropertyChanged
    {
        HubConnection connection;
        static Microsoft.Azure.Devices.ServiceClient serviceClient;
        private static string connectionString = "HostName=skelton-test-hub.azure-devices.net;DeviceId=MyCDevice;SharedAccessKey=KPOS6risRzDE9E/p5aLaRev1vE5ENfqBUYmOluma++Q=";
        private static string deviceId = "MyCDevice";

        private int _counter1;
        public int Counter1
        {
            get => _counter1;
            set => SetProperty(ref _counter1, value);
        }

        private int _counter2;
        public int Counter2
        {
            get => _counter2;
            set => SetProperty(ref _counter2, value);
        }

        public ICommand IncrementCounterCommand { get; }

        public ICommand FeedCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            IncrementCounterCommand = new Command(IncrementCounter);
            FeedCommand = new Command(Feed);

            InitCounter();

            connection = new HubConnectionBuilder()
                .WithUrl("https://counterfunctions20190430083947.azurewebsites.net/api").Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<int>("CounterUpdate", (counterVal) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Counter1 = counterVal;
                });
            });

            try
            {
                connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void IncrementCounter()
        {
            UpdateCounter();
        }


        private async void Feed()
        {
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = Microsoft.Azure.Devices.ServiceClient.CreateFromConnectionString(connectionString);
            SendCloudToDeviceMessageAsync().Wait();
        }

        private async static Task SendCloudToDeviceMessageAsync()
        {
            var commandMessage = new
             Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes("Cloud to device message Ben."));
            try
            {
                await serviceClient.SendAsync("MyCDevice", commandMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async void InitCounter()
        {
            var client = new HttpClient();

             string url = "https://counterfunctions20190430083947.azurewebsites.net/api/get-counter/0";

            var getStringTask = await client.GetStringAsync(url);

            try
            {
                Counter1 = int.Parse(getStringTask);

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed GetCounter:{ex.Message}");
                Counter1 = -1;
            }
        }

        public async void UpdateCounter()
        {
            var client = new HttpClient();

            string url = "https://counterfunctions20190430083947.azurewebsites.net/api/update-counter?counter={counter}";
            url = url.Replace("{counter}", (Counter1+1).ToString());

            var getStringTask = await client.GetStringAsync(url);

            //InitCounter(); //temp
        }

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        }
}
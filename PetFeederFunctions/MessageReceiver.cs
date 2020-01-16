using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System;

namespace PetFeederFunctions
{
    public static class MessageReceiver
    {
        private static HttpClient client = new HttpClient();

        static int Counter = 0;

        [FunctionName("MessageReceiver")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "IotHubConnection")]EventData message, ILogger log)
        {

            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");

            UpdateCounter();
        }

        static async void UpdateCounter()
        {
            var client = new HttpClient();

            string url = "https://counterfunctions20190430083947.azurewebsites.net/api/get-counter/0";

            var getStringTask = await client.GetStringAsync(url);
            try
            {
                Counter = int.Parse(getStringTask);

                string urlUpdate = "https://counterfunctions20190430083947.azurewebsites.net/api/update-counter?counter={counter}";
                urlUpdate = urlUpdate.Replace("{counter}", (Counter + 1).ToString());
                client.GetStringAsync(urlUpdate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed GetCounter:{ex.Message}");
            }
        }

    }
}
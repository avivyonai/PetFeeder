using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace PetFeederFunctions
{
    public static class CounterFunctions
    {
        private const string c_AzureSignalRConnectionString = "Endpoint=https://counter-signalr.service.signalr.net;AccessKey=/SramyLlJyI85LBGVB3ipfFhz6Tofk5g1AXRv92niTA=;Version=1.0;";

        /*
        private static readonly AzureSignalR SignalR = new AzureSignalR(Environment.GetEnvironmentVariable("AzureSignalRConnectionString"));

        [FunctionName("negotiate")]
        public static async Task<SignalRConnectionInfo> NegotiateConnection(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage request,
            ILogger log)
        {
            try
            {
                ConnectionRequest connectionRequest = await ExtractContent<ConnectionRequest>(request);
                log.LogInformation($"Negotiating connection for user: <{connectionRequest.UserId}>.");

                string clientHubUrl = SignalR.GetClientHubUrl("CounterUpdate");
                string accessToken = SignalR.GenerateAccessToken(clientHubUrl, connectionRequest.UserId);
                return new SignalRConnectionInfo { AccessToken = accessToken, Url = clientHubUrl };
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to negotiate connection.");
                throw;
            }
        }
        */

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req,
            [SignalRConnectionInfo(HubName = "CounterUpdate", ConnectionStringSetting = "AzureSignalRConnectionString")]SignalRConnectionInfo connectionInfo,
            ILogger log)
        {
            log.LogInformation($"Negotiating connection");
            return connectionInfo;
        }

        [FunctionName("UpdatePlateWeight")]
        public static async Task UpdateCounter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest request,
            [Table("CountersTable")] CloudTable cloudTable,
            [SignalR(HubName = "CounterUpdate", ConnectionStringSetting = "AzureSignalRConnectionString")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation("Updating counter.");


            //Counter counterRequest = await ExtractContent<Counter>(request);
            string counterStr = request.Query["counter"];
            string requestBody = new StreamReader(request.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            counterStr = counterStr ?? data?.name;
            int counter = 0;
            try
            {
                if (counterStr != null)
                    counter = int.Parse(counterStr);

                Counter cloudCounter = new Counter();
                cloudCounter.Id = 0;
                cloudCounter.Count = counter;
                cloudCounter.PartitionKey = "counter";
                cloudCounter.RowKey = "0";

                //Counter cloudCounter = await GetOrCreateCounter(cloudTable, counterRequest.Id);
                TableOperation updateOperation = TableOperation.InsertOrReplace(cloudCounter);
                await cloudTable.ExecuteAsync(updateOperation);

                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        //UserId = "userId1",
                        Target = "CounterUpdate",
                        Arguments = new object[] { counter }
                    });
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
            }
        }

        [FunctionName("GetPlateWeight")]
        public static async Task<IActionResult> GetCounter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "get-counter/{id}")] HttpRequestMessage request,
            [Table("CountersTable")] CloudTable cloudTable,
            string id,
            ILogger log)
        {
            log.LogInformation("Getting counter.");

            Counter counter = await GetOrCreateCounter(cloudTable, int.Parse(id));
            // Create the Delete TableOperation and then execute it.

            if (counter != null)
            {
                Console.WriteLine("counter is " + counter.Count);
                return (ActionResult)new OkObjectResult(counter.Count);
            }

            Console.WriteLine("cant get counter");
            return new BadRequestObjectResult(counter.Count);
        }

        private static async Task<T> ExtractContent<T>(HttpRequestMessage request)
        {
            string connectionRequestJson = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(connectionRequestJson);
        }

        private static async Task<Counter> GetOrCreateCounter(CloudTable cloudTable, int counterId)
        {
            TableQuery<Counter> idQuery = new TableQuery<Counter>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, counterId.ToString()));

            TableQuerySegment<Counter> queryResult = await cloudTable.ExecuteQuerySegmentedAsync(idQuery, null);
            Counter cloudCounter = queryResult.FirstOrDefault();
            if (cloudCounter == null)
            {
                cloudCounter = new Counter { Id = counterId };

                TableOperation insertOperation = TableOperation.InsertOrReplace(cloudCounter);
                cloudCounter.PartitionKey = "counter";
                cloudCounter.RowKey = cloudCounter.Id.ToString();
                TableResult tableResult = await cloudTable.ExecuteAsync(insertOperation);
                return await GetOrCreateCounter(cloudTable, counterId);
            }

            return cloudCounter;
        }
    }
}

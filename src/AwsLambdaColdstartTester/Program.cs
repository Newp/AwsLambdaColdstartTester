using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AwsLambdaColdstartTester
{
    class Program
    {
        static readonly Amazon.Lambda.AmazonLambdaClient awsClient = new Amazon.Lambda.AmazonLambdaClient(Amazon.RegionEndpoint.APNortheast1);
        static readonly List<long> results = new List<long>();
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            HttpClient client = new HttpClient();
            
            Stopwatch watch = new Stopwatch();
            for (int i =0;i<100;i++)
            {
                await Refresh();
                await Task.Delay(100);
                watch.Restart();
                await client.GetAsync("https://67au6qz7z5.execute-api.ap-northeast-1.amazonaws.com/Prod/api/values");
                
                watch.Stop();

                AddResult(watch.ElapsedMilliseconds);
                await Task.Delay(100);
            }
        }

        static void AddResult(long elapsed)
        {
            results.Add(elapsed);


            var max = results.Max();
            var min = results.Min();
            var avg = results.Average();
            Console.WriteLine(new { elapsed, min, max, avg });
        }

        static async Task Refresh()
        {
            var refreshKey = new byte[64];
            new Random().NextBytes(refreshKey);
            await awsClient.UpdateFunctionConfigurationAsync(new Amazon.Lambda.Model.UpdateFunctionConfigurationRequest()
            {
                FunctionName= "dotnet5-lambda-container-test-AspNetCoreFunction-XSP6USO8OBHO",
                Environment = new Amazon.Lambda.Model.Environment() { Variables = new() { { "key", Convert.ToBase64String(refreshKey) } } }
            });
        }
    }
}

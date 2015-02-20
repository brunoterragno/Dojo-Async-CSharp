using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //DateTime t1 = DateTime.Now;
            //PrintPrimaryNumbers();
            //var ts1 = DateTime.Now.Subtract(t1);
            //Console.WriteLine("Finished Sync and started Async");
            //var t2 = DateTime.Now;
            //PrintPrimaryNumbersAsync();
            //var ts2 = DateTime.Now.Subtract(t2);

            //Console.WriteLine(string.Format("It took {0} for the sync call and {1} for the Async one", ts1, ts2));

            //Console.WriteLine("Any Key to terminate!!");
            //Console.ReadLine();

            // C# 4.0
            //Task t = new Task(Speak);
            //t.Start();
            //Task t = Task.Factory.StartNew(Speak);

            // C# 4.5
            //Task t = Task.Run(Speak);

            //Console.WriteLine("esperando para completar");
            //t.Wait();

            //Console.WriteLine("Feito!");

            //Task.Factory.StartNew(WhatTypeOfThreadAmI).Wait();
            //Task.Factory.StartNew(WhatTypeOfThreadAmI, TaskCreationOptions.LongRunning).Wait();

            //var importer = new DataImporter();
            //string importDirectory = @"C:\data";
            // CLOSURE
            //Task.Factory.StartNew(() => importer.Import(importDirectory));

            //Task<string> downloadTask = DownloadWebPageAsync("http://www.rocksolidknowledge.com/5SecondPage.aspx");
            //while (!downloadTask.IsCompleted)
            //{
            //    Console.Write(".");
            //    Thread.Sleep(250);
            //}
            //Console.WriteLine(downloadTask.Result);

            //Task<int> firstTask = Task.Factory
            //    .StartNew<int>(() => { Console.WriteLine("First Task"); return 42; });
            
            //Task secondTask = firstTask
            //    .ContinueWith(ft => Console.WriteLine("Second Task, First task returned {0}", ft.Result));
            //secondTask.Wait();

            //Task.Factory.StartNew(() =>
            //{
            //    Task nested = Task.Factory.StartNew((() => Console.WriteLine("Nested..")));
            //}).Wait();


            Console.ReadLine();
        }

        private static Task<string> DownloadWebPageAsync(string url)
        {
            return Task.Factory.StartNew(() => DownloadWebPage(url));
        }

        private static Task<string> BetterDownloadWebPageAsync(string url)
        {
            WebRequest request = WebRequest.Create(url);
            IAsyncResult ar = request.BeginGetResponse(null, null);
            Task<string> downloadTask =
            Task.Factory
            .FromAsync<string>(ar, iar =>
            {
                using (WebResponse response = request.EndGetResponse(iar))
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        return reader.ReadToEnd();
                    }
                }
            });
            return downloadTask;
        }

        private static string DownloadWebPage(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            {
                // this will return the content of the web page
                return reader.ReadToEnd();
            }
        }

        private static void WhatTypeOfThreadAmI()
        {
            Console.WriteLine("I'm a {0} thread", Thread.CurrentThread.IsThreadPoolThread ? "Thread Pool" : "Custom");
        }

        private static void Speak()
        {
            Console.WriteLine("hello nt!");
        }

        public static IEnumerable<int> getPrimes(int min, int count)
        {
            return Enumerable.Range(min, count).Where(n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
        }

        public static Task<IEnumerable<int>> getPrimesAsync(int min, int count)
        {
            return Task.Run(() => Enumerable.Range(min, count).Where
             (n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i =>
               n % i > 0)));
        }

        private static void PrintPrimaryNumbers()
        {
            for (int i = 0; i < 10; i++)
                getPrimes(i * 100000 + 1, i * 1000000)
                    .ToList();//.ForEach(x => Console.WriteLine(x));
        }

        private static async void PrintPrimaryNumbersAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                var result = await getPrimesAsync(i * 100000 + 1, i * 1000000);
                result.ToList();//.ForEach(x => Console.WriteLine(x));
            }
        }
    }

    public class DataImporter
    {
        public void Import(string directory)
        {

        }
    }
}

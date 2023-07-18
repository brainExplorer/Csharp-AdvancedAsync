using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Csharp_AdvancedAsync
{
    internal class DemoMethods
    {
        public static List<string> PrepData()
        {
            var list = new List<string>();
            list.Add("https://www.cnn.com");
            list.Add("https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios");
            list.Add("https://www.codewars.com/users/spero");
            list.Add("https://www.hackerrank.com/");
            list.Add("https://www.bbc.com/");
            list.Add("https://theguardian.com");
            return list;
        }
        public static async Task<List<WebsiteDataModel>> RunDownloadAsync(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();
            ProgressReportModel report = new ProgressReportModel();


            foreach (var item in websites)
            {
                WebsiteDataModel results = await DownloadWebsiteAsync(item);
                output.Add(results);

                cancellationToken.ThrowIfCancellationRequested();

                report.SitesDownloaded = output;
                report.PercentageComplete = (output.Count * 100) / websites.Count;
                progress.Report(report);
            }
            return output;
        }

        public static List<WebsiteDataModel> RunDownloadsync()
        {
           List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            foreach (var item in websites)
            {
                WebsiteDataModel results = DownloadWebsite(item);
                output.Add(results);
            }
            return output;
        }


        //Parallel.Foreach
        public static List<WebsiteDataModel> RunDownloadParallelsync()
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            Parallel.ForEach<string>(websites, (site) =>
            {
                WebsiteDataModel results = DownloadWebsite(site);
                output.Add(results);
            });
            return output;
        }

        // await Task.Run()....
        public static async Task<List<WebsiteDataModel>> RunDownloadParallelsync_2(IProgress<ProgressReportModel> progress)
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();
            ProgressReportModel report = new ProgressReportModel();
            await Task.Run(() =>
            {
                Parallel.ForEach<string>(websites, (site) =>
                {
                    WebsiteDataModel results = DownloadWebsite(site);
                    output.Add(results);
                    report.SitesDownloaded = output;
                    report.PercentageComplete = (output.Count * 100) / websites.Count;
                    progress.Report(report);
                });
            });
            
            return output;
        }

        private static WebsiteDataModel DownloadWebsite(string item)
        {
            WebsiteDataModel output = new();
            WebClient client = new WebClient();

            output.WebsiteUrl = item;
            output.WebsiteData = client.DownloadString(item);
            return output;
        }

        public static async Task<List<WebsiteDataModel>> RunDownloadparallelAsync()
        {
            List<string> websites = PrepData();
            List<Task<WebsiteDataModel>> output = new();

            foreach (var item in websites)
            {
                output.Add(DownloadWebsiteAsync(item));
            }

            var results = await Task.WhenAll(output);

            return new List<WebsiteDataModel>(results);
        }

        private static async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteURL ;
            output.WebsiteData = await client.DownloadStringTaskAsync(websiteURL);

            return output;

        }
    }
}
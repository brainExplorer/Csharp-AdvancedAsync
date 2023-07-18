using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Csharp_AdvancedAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        CancellationTokenSource cts = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
       

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var results = DemoMethods.RunDownloadsync();
            PrintResults(results);

            watch.Stop();
            var elapsedMS = watch.ElapsedMilliseconds;
            resultWindow.Text += $"Total execution time: {elapsedMS}";
        }

        private void ProgressReport(object? sender, ProgressReportModel e)
        {
            dashboardProgress.Value = e.PercentageComplete;
            PrintResults(e.SitesDownloaded);
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ProgressReport;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            // snippet with surround it ctrl+k and then ctrl+s
            try
            {
                var results = await DemoMethods.RunDownloadAsync(progress, cts.Token);
                PrintResults(results);
            }
            catch (OperationCanceledException)
            {

                resultWindow.Text +=$"The async download was cancelled{Environment.NewLine}";
            }

            watch.Stop();
            var elapsedMS = watch.ElapsedMilliseconds;
            resultWindow.Text += $"Total execution time: {elapsedMS}";
        }

        private async void executeParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ProgressReport;
            //var results = await DemoMethods.RunDownloadparallelAsync();
            //var results = DemoMethods.RunDownloadParallelsync();
            var results = await DemoMethods.RunDownloadParallelsync_2(progress);
            PrintResults(results);

            watch.Stop();
            var elapsedMS = watch.ElapsedMilliseconds;
            resultWindow.Text += $"Total execution time: {elapsedMS}";
        }

        private void cancelOperation_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }

        private void PrintResults(List<WebsiteDataModel> results)
        {
            resultWindow.Text = "";

            foreach (var item in results)
            {
                resultWindow.Text += $"{item.WebsiteUrl} donloaded : {item.WebsiteData.Length} character.\n";
            }
        }

    }
}

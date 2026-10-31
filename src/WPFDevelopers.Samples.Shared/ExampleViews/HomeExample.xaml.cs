using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;
using Tag = WPFDevelopers.Controls.Tag;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class HomeExample : UserControl
    {
        private CancellationTokenSource _statisticsRequest;
        private bool _statisticsLoaded;
        private bool _synchronizingTheme;

        public HomeExample()
        {
            InitializeComponent();
            VersionTag.Content = typeof(Tag).Assembly.GetName().Version.ToString(3);
            Loaded += HomeExample_Loaded;
            Unloaded += HomeExample_Unloaded;
        }

        private void HomeExample_Loaded(object sender, RoutedEventArgs e)
        {
            WPFDevelopers.Resources.ThemeChanged -= Resources_ThemeChanged;
            WPFDevelopers.Resources.ThemeChanged += Resources_ThemeChanged;
            SynchronizeTheme();
            if (!_statisticsLoaded)
                RefreshStatistics();
        }

        private void HomeExample_Unloaded(object sender, RoutedEventArgs e)
        {
            WPFDevelopers.Resources.ThemeChanged -= Resources_ThemeChanged;
            if (_statisticsRequest == null)
                return;

            _statisticsRequest.Cancel();
            _statisticsRequest = null;
            RefreshButton.IsEnabled = true;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshStatistics();
        }

        private void RefreshStatistics()
        {
            if (_statisticsRequest != null)
                return;

            var request = new CancellationTokenSource();
            _statisticsRequest = request;
            RefreshButton.IsEnabled = false;
            StarsTag.Content = DownloadsTag.Content = "加载中";
            StatisticsStatus.Text = "正在读取 GitHub Star 和 NuGet 累计下载量...";
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                var stars = LoadMetric("GitHub", () =>
                    ReadCount(DownloadJson<RepositoryStatistics>(
                        "https://api.github.com/repos/WPFDevelopersOrg/WPFDevelopers", request.Token).Stars));
                request.Token.ThrowIfCancellationRequested();
                var downloads = LoadMetric("NuGet", () =>
                {
                    var result = DownloadJson<PackageSearchResult>(
                        "https://azuresearch-usnc.nuget.org/query?q=packageid:WPFDevelopers&prerelease=true", request.Token);
                    var package = result.Data == null ? null : result.Data.FirstOrDefault(item =>
                        string.Equals(item.Id, "WPFDevelopers", StringComparison.OrdinalIgnoreCase));
                    if (package == null)
                        throw new InvalidDataException("NuGet 未返回 WPFDevelopers 包统计");
                    return ReadCount(package.TotalDownloads);
                });
                e.Result = new[] { stars, downloads };
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                var isCurrent = ReferenceEquals(_statisticsRequest, request);
                request.Dispose();
                worker.Dispose();
                if (!isCurrent)
                    return;

                _statisticsRequest = null;
                RefreshButton.IsEnabled = true;
                if (e.Error != null)
                {
                    StarsTag.Content = DownloadsTag.Content = "加载失败";
                    StatisticsStatus.Text = "统计加载失败：" + e.Error.Message + "。请点击“刷新统计”重试。";
                    return;
                }

                var metrics = (MetricResult[])e.Result;
                StarsTag.Content = metrics[0].Content;
                DownloadsTag.Content = metrics[1].Content;
                var errors = metrics.Where(metric => metric.Error != null).Select(metric => metric.Error).ToArray();
                _statisticsLoaded = errors.Length == 0;
                StatisticsStatus.Text = _statisticsLoaded
                    ? "更新于 " + DateTime.Now.ToString("HH:mm:ss") + " · 下载量为 NuGet 所有版本累计下载量"
                    : string.Join(Environment.NewLine, errors);
            };
            worker.RunWorkerAsync();
        }

        private static MetricResult LoadMetric(string source, Func<long> load)
        {
            try
            {
                return new MetricResult { Content = load().ToString("N0", CultureInfo.InvariantCulture) };
            }
            catch (WebException ex)
            {
                return MetricFailure(source, ex);
            }
            catch (SerializationException ex)
            {
                return MetricFailure(source, ex);
            }
            catch (InvalidDataException ex)
            {
                return MetricFailure(source, ex);
            }
        }

        private static MetricResult MetricFailure(string source, Exception error)
        {
            return new MetricResult
            {
                Content = "加载失败",
                Error = source + " 统计加载失败：" + error.Message + "。请点击“刷新统计”重试。"
            };
        }

        private static T DownloadJson<T>(string address, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var request = (HttpWebRequest)WebRequest.Create(address);
            request.UserAgent = "WPFDevelopers-Demo/1.0";
            request.Accept = "application/json";
            request.Timeout = 15000;
            request.ReadWriteTimeout = 15000;
            using (cancellationToken.Register(request.Abort))
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                var result = new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
                if (result == null)
                    throw new InvalidDataException("统计响应为空");
                return (T)result;
            }
        }

        private static long ReadCount(long? count)
        {
            if (!count.HasValue || count.Value < 0)
                throw new InvalidDataException("统计响应缺少有效的计数");
            return count.Value;
        }

        private void RepositoryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var address = button == null ? null : button.Tag as string;
            Uri uri;
            if (!Uri.TryCreate(address, UriKind.Absolute, out uri) || uri.Scheme != Uri.UriSchemeHttps)
                throw new InvalidOperationException("Repository button has no valid HTTPS address.");

            try
            {
                Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
                LinkError.Visibility = Visibility.Collapsed;
            }
            catch (Win32Exception ex)
            {
                ShowLinkError(ex);
            }
            catch (InvalidOperationException ex)
            {
                ShowLinkError(ex);
            }
        }

        private void ShowLinkError(Exception error)
        {
            LinkError.Text = "无法打开链接：" + error.Message;
            LinkError.Visibility = Visibility.Visible;
        }

        private void Segmented_ItemClick(object sender, RoutedEventArgs e)
        {
            if (_synchronizingTheme) return;
            var segmented = sender as Segmented;
            var theme = segmented.SelectedIndex == 0 ? ThemeType.Light : ThemeType.Dark;
            if (ThemeManager.Instance.Resources.Theme == theme) return;
            App.Theme = theme;
            ThemeManager.Instance.SwitchTheme(PART_Segmented, theme);
            SynchronizeTheme();
        }

        private void Resources_ThemeChanged(ThemeType currentTheme)
        {
            SynchronizeTheme();
        }

        private void SynchronizeTheme()
        {
            var theme = ThemeManager.Instance.Resources.Theme;
            _synchronizingTheme = true;
            try
            {
                PART_Segmented.SelectedIndex = theme == ThemeType.Dark ? 1 : 0;
                SegmentedLight.IsChecked = theme != ThemeType.Dark;
                SegmentedDark.IsChecked = theme == ThemeType.Dark;
            }
            finally
            {
                _synchronizingTheme = false;
            }
        }

        private sealed class MetricResult
        {
            public string Content { get; set; }
            public string Error { get; set; }
        }

        [DataContract]
        private sealed class RepositoryStatistics
        {
            [DataMember(Name = "stargazers_count")]
            public long? Stars { get; set; }
        }

        [DataContract]
        private sealed class PackageSearchResult
        {
            [DataMember(Name = "data")]
            public PackageStatistics[] Data { get; set; }
        }

        [DataContract]
        private sealed class PackageStatistics
        {
            [DataMember(Name = "id")]
            public string Id { get; set; }

            [DataMember(Name = "totalDownloads")]
            public long? TotalDownloads { get; set; }
        }
    }
}

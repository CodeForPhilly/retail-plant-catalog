using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using RobotsParser;
using Shared;

namespace SavvyCrawler
{
    public sealed class Crawler : IDisposable
	{
        /// <summary>Outbound HTTP timeout (TLS + response). Default 5s was too aggressive for slow retail sites.</summary>
        public static TimeSpan DefaultRequestTimeout { get; set; } = TimeSpan.FromSeconds(45);

        /// <summary>How long pooled connections live before reconnecting (refreshes DNS on long-lived clients).</summary>
        public static TimeSpan PooledConnectionLifetime { get; set; } = TimeSpan.FromMinutes(5);

		public List<string> Links { get; set; } = new List<string>();
        public List<string> Visited { get; set; } = new List<string>();
        private string? host;
        private Robots? robots;
        private bool robotsLoaded = false;
        private readonly TermCounter counter;
        private readonly HttpClient client;
        private bool _disposed;

        public Crawler(TermCounter counter, TimeSpan? requestTimeout = null)
        {
            this.counter = counter;
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = PooledConnectionLifetime,
                AutomaticDecompression = DecompressionMethods.None
            };
            client = new HttpClient(handler, disposeHandler: true)
            {
                Timeout = requestTimeout ?? DefaultRequestTimeout
            };
        }

        /// <summary>Updates User-Agent (and related defaults) on the single <see cref="HttpClient"/> for this crawl.</summary>
        public void SetDefaultHeaders(bool legacyDevice = false)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.Remove("Accept-Encoding");
            if (legacyDevice)
                client.DefaultRequestHeaders.UserAgent.ParseAdd("BlackBerry8100/4.2.0 Profile/MIDP-2.0 Configuration/CLDC-1.1 VendorID/155");
            else
                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "none");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            client.Dispose();
            GC.SuppressFinalize(this);
        }

        private static Exception Unwrap(Exception ex)
        {
            while (ex is AggregateException ae && ae.InnerExceptions.Count > 0)
                ex = ae.InnerException ?? ae.InnerExceptions[0];
            return ex;
        }

        /// <summary>Maps HTTP / socket / cancellation failures to <see cref="CrawlFailException"/>.</summary>
        internal static void ThrowCrawlFail(Exception ex)
        {
            ex = Unwrap(ex);
            if (ex is CrawlFailException cf)
                throw cf;
            if (ex is HttpRequestException webex)
            {
                if (webex.HttpRequestError == HttpRequestError.NameResolutionError)
                    throw new CrawlFailException(CrawlStatus.DnsFailure, ex);
                if (ex.InnerException is SocketException se &&
                    (se.SocketErrorCode == SocketError.HostNotFound || se.SocketErrorCode == SocketError.TryAgain))
                    throw new CrawlFailException(CrawlStatus.DnsFailure, ex);
                switch (webex.StatusCode)
                {
                    case HttpStatusCode.TemporaryRedirect:
                    case HttpStatusCode.Redirect:
                    case HttpStatusCode.Moved:
                        throw new CrawlFailException(CrawlStatus.Redirect, ex);
                    case HttpStatusCode.NotFound:
                        throw new CrawlFailException(CrawlStatus.Missing, ex);
                    default:
                        throw new CrawlFailException(CrawlStatus.Missing, ex);
                }
            }
            if (ex is TaskCanceledException)
                throw new CrawlFailException(CrawlStatus.Timeout, ex);
            if (ex is TimeoutException)
                throw new CrawlFailException(CrawlStatus.Timeout, ex);
            if (ex is SocketException sock && sock.SocketErrorCode == SocketError.TimedOut)
                throw new CrawlFailException(CrawlStatus.Timeout, ex);
            throw new CrawlFailException(CrawlStatus.Missing, ex);
        }


        public async Task<Dictionary<string, int>> Start(string absolutePath, int maxPages, bool testOnly = false)
        {
            Links.Clear();
            Visited.Clear();
            Uri startUri;
            try
            {
                robotsLoaded = false;
                startUri = new Uri(absolutePath);
                host = startUri.Host;
            }
            catch (Exception urlEx)
            {
                throw new CrawlFailException(CrawlStatus.UrlParsingError, urlEx);
            }
            // Match listing scheme so HTTPS-only / HSTS hosts are not probed over plain HTTP (often flaky from cloud IPs).
            var robotsScheme = startUri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
                ? Uri.UriSchemeHttps
                : Uri.UriSchemeHttp;
            var robotsUrl = $"{robotsScheme}://{host}/robots.txt";
            robots = new Robots("PAC Agent");
        
            try
            {
                string downloadString = await client.GetStringAsync(robotsUrl);
                if (!string.IsNullOrEmpty(downloadString))
                {
                    await robots.LoadRobotsContent(downloadString);
                    robotsLoaded = true;
                }
            }catch(Exception)
            {
                //ignore:  Can't parse robots, then no restrictions
            }
            SetDefaultHeaders();
            try
            {
                await FetchUrl(absolutePath, testOnly);
            }
            catch (Exception ex)
            {
                ThrowCrawlFail(ex);
            }
            //get host and ignore non hosts
            if (testOnly) return new Dictionary<string, int> { };

            var unvisited = new List<string>();
            do {
                unvisited = Links.Except(Visited).ToList();
                foreach (var unvisitedLink in unvisited)
                {
                    maxPages--;
                    if (maxPages < 0) return counter.Terms;
                    try
                    {
                        await FetchUrl(unvisitedLink, false);
                    }
                    catch (Exception ex)
                    {
                        ThrowCrawlFail(ex);
                    }
                }
            } while (unvisited.Count > 0);
            return counter.Terms;
        }

        private async Task FetchUrl(string absolutePath, bool testOnly)
        {
                if (robotsLoaded)
                {
                    try
                    {
                        var disallowByDefault = robots?.GetDisallowedPaths()?.Any(u => u == "/") ?? false;
                        if (disallowByDefault && !(robots?.IsPathAllowed(absolutePath) ?? true)) return;
                    }
                    catch (Exception)
                    {
                        // robots.txt was fetched and LoadRobotsContent did not throw, but some sites ship non-classic
                        // content (e.g. legal boilerplate only). Querying rules can throw from the parser — crawl without filtering.
                        robotsLoaded = false;
                    }
                }

                var uri = new Uri(absolutePath);
                var parts = uri.PathAndQuery.Split('?');
                string text = "";
                SetDefaultHeaders();
                if (parts[0].EndsWith(".pdf"))
                {
                    //Get pdf into memory
                    var data = await client.GetByteArrayAsync(absolutePath);
                    using var ms = new MemoryStream(data);
                    text = PdfExtensions.GetText(ms);
                } else if (parts[0].EndsWith(".xlsx"))
                {
                    // get xlsx into memory
                    var data = await client.GetByteArrayAsync(absolutePath);
                    using var ms = new MemoryStream(data);
                    text = ExcelHelpers.GetText(ms);
                }
                else
                {
                    text = await ParseHtml(absolutePath);
                }
                Visited.Add(absolutePath);
                if (!testOnly)
                    counter.Examine(text);
        }
        

        public async Task<string> ParseHtml(string absolutePath)
        {
            if (absolutePath.Contains("google.com"))
                SetDefaultHeaders(true);
            var str =  await client.GetStringAsync(new Uri(absolutePath));
            return StripScripts(str);
        }
        public string StripScripts(string html)
        {
            // Need the scripts for hungry plants - skip script stripping
            return html;
        }
    }
}
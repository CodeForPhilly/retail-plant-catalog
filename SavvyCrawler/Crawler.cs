using System.Net;
using System.Text.RegularExpressions;
using RobotsParser;
using Shared;

namespace SavvyCrawler
{
    public class Crawler
	{
		public List<string> Links { get; set; } = new List<string>();
        public List<string> Visited { get; set; } = new List<string>();
        private string? host;
        private Robots? robots;
        private bool robotsLoaded = false;
        private readonly TermCounter counter;
        private HttpClient client;

        public Crawler(TermCounter counter) {
            this.counter = counter;
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            
              //
#pragma warning restore SYSLIB0014 // Type or member is obsolete
         
        }
        public void SetDefaultHeaders(bool legacyDevice = false)
        {
            if (legacyDevice)
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "BlackBerry8100/4.2.0 Profile/MIDP-2.0 Configuration/CLDC-1.1 VendorID/155");
            }
            else
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");

            }
            client.DefaultRequestHeaders.Add("Accept-Encoding", "none");
        }


        public async Task<Dictionary<string, int>> Start(string absolutePath, int maxPages, bool testOnly = false)
        {
            try
            {
                robotsLoaded = false;
                host = new Uri(absolutePath).Host;
            }
            catch (Exception ex)
            {
                throw new CrawlFailException(CrawlStatus.UrlParsingError);
            }
            var robotsUrl = $"http://{host}/robots.txt";
            robots = new Robots("PAC Agent");
        
            try
            {
                string downloadString = await client.GetStringAsync(robotsUrl);
                if (!string.IsNullOrEmpty(downloadString))
                {
                    await robots.LoadRobotsContent(downloadString);
                    robotsLoaded = true;
                }
            }catch(Exception ex)
            {
                //ignore:  Can't parse robots, then no restrictions
            }
            SetDefaultHeaders();
            await FetchUrl(absolutePath, testOnly).ContinueWith(t =>
            {
                if (t.Status == TaskStatus.Canceled) throw new CrawlFailException(CrawlStatus.Timeout);
                if (t.Exception != null)
                {
                    var ex = (t.Exception as AggregateException).InnerException;
                    if (ex is HttpRequestException)
                    {
                        var webex = ex as HttpRequestException;
                        if (webex != null && webex.HttpRequestError == HttpRequestError.NameResolutionError)
                            throw new CrawlFailException(CrawlStatus.DnsFailure);
                        switch (webex.StatusCode)
                        {
                            case HttpStatusCode.TemporaryRedirect:
                            case HttpStatusCode.Redirect:
                            case HttpStatusCode.Moved:
                                throw new CrawlFailException(CrawlStatus.Redirect);
                            case HttpStatusCode.NotFound:
                                throw new CrawlFailException(CrawlStatus.Missing);

                        }
                    }
                    if (ex is TimeoutException)
                    {
                        throw new CrawlFailException(CrawlStatus.Timeout);
                    }
                    

                }
            });
            //get host and ignore non hosts
            if (testOnly) return new Dictionary<string, int> { };

            var unvisited = new List<string>();
            do {
                unvisited = Links.Except(Visited).ToList();
                foreach (var unvisitedLink in unvisited)
                {
                    maxPages--;
                    if (maxPages < 0) return counter.Terms;
                    await FetchUrl(unvisitedLink, false);
                }
            } while (unvisited.Count > 0);
            return counter.Terms;
        }

        private async Task FetchUrl(string absolutePath, bool testOnly)
        {
                if (robotsLoaded)
                {
                    var disallowByDefault = robots?.GetDisallowedPaths()?.Any(u => u == "/") ?? false;
                    if (disallowByDefault && !(robots?.IsPathAllowed(absolutePath) ?? true)) return;
                }

                var uri = new Uri(absolutePath);
                var parts = uri.PathAndQuery.Split('?');
                string text = "";
                bool isHtml = false;
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
                    isHtml = true;
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
            return html; //need the scripts for hungry plants
            var scripts = new Regex(@"<script.*?</script>", RegexOptions.Singleline);
            return scripts.Replace(html, "");
        }
    }
}
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using RobotsParser;

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
        private readonly WebClient client;

        public Crawler(TermCounter counter) {
            this.counter = counter;
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            client = new WebClient();
              //
#pragma warning restore SYSLIB0014 // Type or member is obsolete
         
        }
        public void SetDefaultHeaders(bool legacyDevice = false)
        {
            if (legacyDevice)
            {
                client.Headers.Set("User-Agent", "BlackBerry8100/4.2.0 Profile/MIDP-2.0 Configuration/CLDC-1.1 VendorID/155");
             }
            else
            {
                client.Headers.Set("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");

            }
            client.Headers.Set("Accept-Encoding", "none");
        }


        public async Task<Dictionary<string, int>> Start(string absolutePath, int maxPages)
        {
            try
            {
                robotsLoaded = false;
                host = new Uri(absolutePath).Host;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot parse: " + absolutePath);
                return new Dictionary<string, int>();
            }
            var robotsUrl = $"http://{host}/robots.txt";
            robots = new Robots("PAC Agent");
        
            try
            {
                string downloadString = client.DownloadString(robotsUrl);
                if (!string.IsNullOrEmpty(downloadString))
                {
                    await robots.LoadRobotsContent(downloadString);
                    robotsLoaded = true;
                }
            }catch(Exception ex)
            {
                //ignore
            }
            SetDefaultHeaders();
            await FetchUrl(absolutePath);
            //get host and ignore non hosts
         
            var unvisited = new List<string>();
            do {
                unvisited = Links.Except(Visited).ToList();
                foreach (var unvisitedLink in unvisited)
                {
                    maxPages--;
                    if (maxPages < 0) return counter.Terms;
                    await FetchUrl(unvisitedLink);
                }
            } while (unvisited.Count > 0);
            return counter.Terms;
        }

        private async Task FetchUrl(string absolutePath)
        {
            if (robotsLoaded)
            {
                var disallowByDefault = robots?.GetDisallowedPaths()?.Any(u => u == "/") ?? false;
                if (disallowByDefault && !(robots?.IsPathAllowed(absolutePath) ?? true))  return;
            }
            try
            {
                var uri = new Uri(absolutePath);
                var parts = uri.PathAndQuery.Split('?');
                string text = "";
                bool isHtml = false;
                SetDefaultHeaders();
                if (parts[0].EndsWith(".pdf"))
                {
                    //Get pdf into memory
                   var data = client.DownloadData(absolutePath);
                    using var ms = new MemoryStream(data);
                    text = PdfExtensions.GetText(ms);
                }else if (parts[0].EndsWith(".xlsx"))
                {
                    // get xlsx into memory
                    var data = client.DownloadData(absolutePath);
                    using var ms = new MemoryStream(data);
                    text = ExcelHelpers.GetText(ms);
                }
                else
                {
                    text = await ParseHtml(absolutePath);
                    isHtml = true;
                }
                Visited.Add(absolutePath);
                counter.Examine(text);
                if (!isHtml) return;
                var doc = new HtmlDocument();
                doc.LoadHtml(text);
                var nodes = doc.DocumentNode.SelectNodes("//a[@href]");
                if (nodes != null)
                {
                    foreach (HtmlNode link in nodes)
                    {
                        // Get the value of the HREF attribute
                        string hrefValue = link.GetAttributeValue("href", string.Empty);
                        try
                        {
                            if (string.IsNullOrEmpty(hrefValue)) continue;
                            if (!hrefValue.StartsWith("http") && !hrefValue.StartsWith("tel:") && !hrefValue.StartsWith("mobile:"))
                            {
                                var hyper = new Uri(absolutePath);

                                var hrefHost = hyper.Host;
                                if (!hrefValue.StartsWith('/'))
                                    hrefValue = "/" + hrefValue;
                                hrefValue = $"{uri.Scheme}://{hrefHost}{hrefValue}";
                                Links.Add(hrefValue);
                            }
                            else
                            {
                                var hrefHost = new Uri(hrefValue).Host;
                                if (!string.IsNullOrEmpty(hrefValue) && hrefHost.Contains(host) && !Links.Contains(hrefValue))
                                    Links.Add(hrefValue);
                            }


                        }
                        catch (Exception ex)
                        {
                            return;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                //ignore
                Console.WriteLine(ex.Message + " " + ex.Response?.ResponseUri);
            }catch(Exception ex)
            {
                //ignore
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<string> ParseHtml(string absolutePath)
        {
            if (absolutePath.Contains("google.com"))
                SetDefaultHeaders(true);
            var str =  await client.DownloadStringTaskAsync(new Uri(absolutePath));
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
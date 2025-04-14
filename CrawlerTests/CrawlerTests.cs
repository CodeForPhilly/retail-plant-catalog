using System.Reflection.PortableExecutable;
using SavvyCrawler;
using Shared;
using static System.Net.Mime.MediaTypeNames;

namespace CrawlerTests
{
    public class CrawlerTests
	{
        [Fact]
        public async Task ShouldThrow301()
        {
            try
            {
                var counter = new TermCounter("Japonica", "Camellia Black Magic");
                var crawler = new Crawler(counter);
                var result = await crawler.Start("https://www.hannasgardenshop.com/dept/11/camellias", 1);

            }catch(CrawlFailException ex)
            {
                Assert.Equal(CrawlStatus.Redirect, ex.CrawlStatus);
                return;
            }

            Assert.Fail( "Should've thrown exception");
        }
       
         [Fact]
        public async Task ShouldThrow404()
        {
            try
            {
                var counter = new TermCounter("Japonica", "Camellia Black Magic");
                var crawler = new Crawler(counter);
                var result = await crawler.Start("https://hannasgardenshop.com/inventory/search?404=1&q=dept+1asfd+dfds", 1);

            }
            catch (CrawlFailException ex)
            {
                Assert.Equal(CrawlStatus.Missing, ex.CrawlStatus);
                return;
            }

            Assert.Fail("Should've thrown exception");
        }
        [Fact]
        public async Task ShouldThrowTimeout()
        {
            try
            {
                var counter = new TermCounter("Japonica", "Camellia Black Magic");
                var crawler = new Crawler(counter);
                var result = await crawler.Start("http://10.255.255.1/dept/11/camellias", 1);

            }
            catch (CrawlFailException ex)
            {
                Assert.Equal(CrawlStatus.Timeout, ex.CrawlStatus);
                return;
            }

            Assert.Fail("Should've thrown exception");
        }
        [Fact]
        public async Task ShouldThrowDnsError()
        {
            try
            {
                var counter = new TermCounter("Japonica", "Camellia Black Magic");
                var crawler = new Crawler(counter);
                var result = await crawler.Start("https://www.hannasssssgardenshop.com/dept/11/camellias", 1);

            }
            catch (CrawlFailException ex)
            {
                Assert.Equal(CrawlStatus.DnsFailure, ex.CrawlStatus);
                return;
            }

            Assert.Fail("Should've thrown exception");
        }

		[Fact]
        public async Task ShouldCrawlSiteDirectLink()
        {
			var counter = new TermCounter("Japonica", "Camellia Black Magic");

            var crawler = new Crawler(counter);
			var result = await crawler.Start("https://hannasgardenshop.com/dept/11/camellias", 1);
			Assert.True(result["Camellia Black Magic"] > 0);
		}
        //Ignored because we need scripts for hungry plants
        //[Fact]
        //public async void ShouldEliminateScriptTags()
        //{
        //    var str = "<html><body><script>function alert('');\n{}</script></body></html>";
        //    var counter = new TermCounter("Japonica", "Camellia Black Magic");

        //    var crawler = new Crawler(counter);
        //    var result = crawler.StripScripts(str);
        //    Assert.DoesNotContain("script", result);
        //    Assert.Contains("body", result);
        //}
    }



    public class TermCounterTests
	{
		[Fact]
		public void SimpleFindTerm()
		{
			var counter = new TermCounter("Japonica", "Camellia Black Magic");

			var result = counter.Examine("a very long string with contents Camellia Black Magic somewhere in the middle");

			Assert.Equal(1, result["Camellia Black Magic"]);
        }
        [Fact]
        public void ShouldUtilizeTokenBoundaries()
        {
            var counter = new TermCounter("Japonica", "Camellia Black Magic");

            var result = counter.Examine("a very long string with contents xCamellia Black Magic somewhere in the middle");

            Assert.Equal(0, result["Camellia Black Magic"]);
        }

        [Fact]
        public void ShouldUtilizeTokenBoundariesTakeTwo()
        {
            var counter = new TermCounter("Japonica", "Camellia Black Magic");

            var result = counter.Examine("a very long string with contents Camellia black Magicx somewhere in the middle");

            Assert.Equal(0, result["Camellia Black Magic"]);
        }


        [Fact]
		public void ShouldFindTwoTermsCaseInsensative()
		{
            var counter = new TermCounter("Japonica", "Camellia Black Magic");

            var result = counter.Examine("a very camelLia BlacK magiC string with contents Camellia Black Magic somewhere in the middle");

            Assert.Equal(1, result["Camellia Black Magic"]);
        }
        [Fact]
        public async void CanCrawlMyGardenOfDelights()
        {
            var url = "https://www.mygardenofdelights.com/tropical-plants-cycads";
            var counter = new TermCounter("Pawpaw", "Strawberry", "Umbrella-Tree");
            var crawler = new Crawler(counter);
            var result = await crawler.Start(url, 1);
            Assert.Equal(1, result["Pawpaw"]);
            Assert.Equal(1, result["Strawberry"]);
            Assert.Equal(1, result["Umbrella-Tree"]);
        }

		[Fact]
		public async void CanCrawlGoogleDocs() //robots allowed
		{
            var counter = new TermCounter("Blazing Star");
			var crawler = new Crawler(counter);
			var result = await crawler.Start("https://docs.google.com/document/d/1oXvf0N4k9LXfqYG_s_e9306wCB95DAg740m7cX4Iqc8/edit", 1);
            Assert.Equal(1, result["Blazing Star"]);
        }
        //[Fact] //This site requires javascript eval
        //public async Task CanCrawlWildRidge() //robots allowed
        //{
        //    var counter = new TermCounter("Allium cernuum");
        //    var crawler = new Crawler(counter);
        //    var result = await crawler.Start("https://wildridgeplants.com/shop/", 1);
        //    Assert.Equal(1, result["Allium cernuum"]);
        //}

        [Fact]
        public async Task CanParseGoodHostPlants() //robots allowed
        {
            var counter = new TermCounter("HEAM6");
            var crawler = new Crawler(counter);
            var result = await crawler.Start("https://www.goodhostplants.com/plant-availability/", 1);
            Assert.Equal(1, result["HEAM6"]);
        }

        [Fact]
        public async Task CanFindSouthernCrabApple()
        {
            var counter = new TermCounter("Southern Crabapple");
            var crawler = new Crawler(counter);
            var result = await crawler.Start("https://www.bigmulberrynursery.com/plants/", 1);
            Assert.Equal(1, result["Southern Crabapple"]);
        }

        [Fact]
        public async Task CanParseCalyx() //robots ALLOWED
        {
            var counter = new TermCounter("Conoclinium coelestinum");
            var crawler = new Crawler(counter);
            var result = await crawler.Start("https://calyxnativenursery.com/plants/", 1);
            Assert.Equal(1, result["Conoclinium coelestinum"]);
        }
        //[Fact] //pdf no longer there
        //public async void CanParseNorthBrookNatives() //robots allowed
        //{
        //    var url = "https://static1.squarespace.com/static/602864f89f64ad59109853b1/t/622e2cec1eca2522201f54a7/1647193324231/TFF+2022+Natives+List.pdf";
        //    var counter = new TermCounter("Aquilegia canadensis");
        //    var crawler = new Crawler(counter);
        //    var result = await crawler.Start(url, 1);
        //    Assert.Equal(1, result["Aquilegia canadensis"]);
        //}
        [Fact]
        public async Task CanParseGrowWild() //Robots allowed
        {
            var url = "https://www.growildinc.com/plant-list/";
            var counter = new TermCounter("Acer leucoderme – Chalk Maple");
            var crawler = new Crawler(counter);
            var result = await crawler.Start(url, 1);
            Assert.Equal(1, result["Acer leucoderme – Chalk Maple"]);
        }
        //ROBOTS Disallow
        //[Fact]
        //public async void CanParseFacebook()
        //{
        //    var url = "https://www.facebook.com/recreativenatives/";
        //    var counter = new TermCounter("Acer leucoderme – Chalk Maple");
        //    var crawler = new Crawler(counter);
        //    var result = await crawler.Start(url, 1);
        //    Assert.Equal(1, result["Acer leucoderme – Chalk Maple"]);
        //}

        [Fact]
		public async void CanParsePdf()
		{
			var text = PdfExtensions.GetText(GetStream("catalog2.pdf"));
			Assert.True(!string.IsNullOrEmpty(text));
		}

		//[Fact]
		////Can't do this without a microsoft office license.
  //      public async void CanParseXls()
		//{
		//	var text = ExcelHelpers.GetText(GetStream("catalog.xls"));
  //          Assert.Contains("Hi Martin", text);
  //          Assert.Contains("Hi Susan", text);
  //      }
    
		[Fact]
		public async Task CanParseXlsx()
		{
			var text = ExcelHelpers.GetText(GetStream("catalog.xlsx"));
            Assert.Contains("Hi Martin", text);
            Assert.Contains("Hi Susan", text);
        }
        [Fact]
        public async Task CanParseComplexXlsx()
        {
            var text = ExcelHelpers.GetText(GetStream("catalog_complex.xlsx"));
            Assert.Contains("Monarda fistulosa", text);
            Assert.Contains("Wild Bergamot", text);
        }


        [Fact]
		public async Task CanParseXlsxRemote()
		{
            var counter = new TermCounter("Monarda fistulosa", "Wild Bergamot");
            var crawler = new Crawler(counter);
            var result = await crawler.Start("https://ginosnursery.com/wp-content/uploads/2022/03/RetailAvailability2022-2.xlsx", 1);
            Assert.Equal(1, result["Monarda fistulosa"]);
            Assert.Equal(1, result["Wild Bergamot"]);
        }
		[Fact]
		public async Task CanParsePdfRemote()
		{
            var counter = new TermCounter("Bald Cypress");
            var crawler = new Crawler(counter);
            var result = await crawler.Start("https://www.matlacktreefarm.com/_files/ugd/77bf71_5163e2ecd2ef44d8a6936c4ba5a36c28.pdf", 1);
            Assert.Equal(1, result["Bald Cypress"]);
        }

        /// <summary>
        /// Not needed in lib, just for tests
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static MemoryStream GetStream(string filePath)
        {
            return new MemoryStream(File.ReadAllBytes(filePath));
        }
    }

    /* Problem urls
     * 
     * 
select distinct uri from vendor_urls where VendorId in (
'8c8d9f5d-84df-4c54-aa8a-c5006f29e4b9',
'99e94e85-e2f9-42a4-b588-f3c9f70078ee',
'03e35a47-aa3b-42cc-92cf-2cfe5ec2a735',
'3c1a4b14-1ab3-426e-98d7-dcccaedd4311',
'1e520129-85d3-43c0-ad46-caaffa6622b6',
'e7ca1c6d-ad38-425f-b3b4-43252d761b41',
'61652148-b2da-46f1-bf62-cdf362be3177',
'7bae681a-a5f7-46b7-8340-cb7cd3a1b37b',
'be3f451e-add6-4ee7-a1fd-0f0655328f68',
'0f26f8b8-69f7-499b-975e-387fc7628751',
'0927020b-08aa-4fd3-a819-4e5d03f8d85e',
'ca7e8816-8ef2-47db-b271-c4d7893b429c',
'd29313f2-cef4-413e-bb04-923dc3c4d871',
'ac7a8a07-abe4-4275-83f5-11f604ced592',
'9e1d52c5-469a-4379-beb4-d71e9146db01',
'1cfb477e-75fe-4bf3-a574-9c321907c82e',
'5738514f-15dc-409c-8c92-f1c02f8474bd')
    */
}
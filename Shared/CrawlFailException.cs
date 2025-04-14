using System.Net;

namespace Shared
{
    public class CrawlFailException : Exception
    {
        private readonly CrawlStatus status;
        public CrawlStatus CrawlStatus { get => status; }

        public CrawlFailException(CrawlStatus status)
        {
            this.status = status;
        }
     
    }
    
}
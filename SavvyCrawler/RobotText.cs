using System;
namespace SavvyCrawler
{
	public class RobotText
	{
        private readonly string robotTxt;

        public RobotText(string robotTxt) {
            this.robotTxt = robotTxt;
        }


		public bool ValidateText()
		{
			return false;
		}

		public bool IsCrawlable(string path) {
			return false;
		}
	}
}
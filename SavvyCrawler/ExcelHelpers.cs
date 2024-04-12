using System.Text;
using OfficeOpenXml;

namespace SavvyCrawler
{
    public static class ExcelHelpers
    {

        public static string GetText(MemoryStream memoryStream)
        {
            StringBuilder allText = new StringBuilder();

            try
            {
               
                  ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                // Use EPPlus to open the Excel file
                using (ExcelPackage package = new ExcelPackage(memoryStream))
                {
                    // Iterate through each worksheet
                    foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                    {
                        foreach (var range in worksheet.Cells)  // Access cells directly from ExcelRangeRow
                        {
                            allText.Append(range.Text?.ToString() ?? "");
                            allText.Append(" ");
                        }
                        allText.AppendLine();
                    }
                }

                return allText.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error extracting text from Excel file: " + ex.Message);
                return null;  // Return null on error
            }
        }
    }
}
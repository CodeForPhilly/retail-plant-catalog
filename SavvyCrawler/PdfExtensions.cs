using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace SavvyCrawler
{
    public static class PdfExtensions
    {
        public static string GetText(MemoryStream ms)
        {
            PdfReader reader = new PdfReader(ms);
           
            string text = string.Empty;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += PdfTextExtractor.GetTextFromPage(reader, page);
            }
            reader.Close();
            return text;
        }
    }
}
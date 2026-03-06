using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace SavvyCrawler
{
    public static class PdfExtensions
    {
        public static string GetText(MemoryStream ms)
        {
            var bytes = ms.ToArray();
            using var document = PdfDocument.Open(bytes);
            var parts = new List<string>();
            foreach (var page in document.GetPages())
            {
                var words = page.GetWords(NearestNeighbourWordExtractor.Instance);
                if (words.Any())
                    parts.Add(string.Join(" ", words.Select(w => w.Text)));
                else
                    parts.Add(page.Text);
            }
            var text = string.Join(" ", parts);
            // Normalize whitespace (including Unicode) and hyphens so phrase matching works
            text = Regex.Replace(text, @"\s+", " ");
            text = text.Replace('\u00A0', ' ').Replace('\u2011', '-').Replace('\u2013', '-').Replace('\u2014', '-');
            return text.Trim();
        }
    }
}
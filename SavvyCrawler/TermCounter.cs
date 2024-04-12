using System.Text.RegularExpressions;

public class TermCounter {
    public TermCounter(params string[] terms) {
        foreach (var term in terms)
            Terms[term] = 0;
    }

    public Dictionary<string, int> Terms { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> Examine(string content) {
        content = content.ToLower().Replace("\\n", "\n"); //case insensative.
        foreach (var term in Terms.Keys)
        {
            var pattern = new Regex(@$"(?:^|\s|$|\b){term.Trim().ToLower()}(?:^|\s|$|\b)");
            var numFound = pattern.Split(content).Length - 1;
            if (numFound > 0)
                Terms[term] = numFound > 0 ? 1 : 0;
        }
        return Terms;
    }
}
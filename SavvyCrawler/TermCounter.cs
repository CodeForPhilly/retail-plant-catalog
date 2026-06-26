using System.Text.RegularExpressions;

public class TermCounter {
    private readonly Dictionary<string, Regex> _compiledPatterns;

    public TermCounter(params string[] terms) {
        foreach (var term in terms)
            Terms[term] = 0;
        _compiledPatterns = new Dictionary<string, Regex>(Terms.Count);
        foreach (var term in Terms.Keys)
        {
            var normalized = term.Trim().ToLower();
            var pattern = @"(?:^|\s|$|\b)" + Regex.Escape(normalized) + @"(?:^|\s|$|\b)";
            _compiledPatterns[term] = new Regex(pattern, RegexOptions.Compiled);
        }
    }

    public Dictionary<string, int> Terms { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> Examine(string content) {
        content = content.ToLower().Replace("\\n", "\n"); //case insensative.
        foreach (var term in Terms.Keys)
        {
            var numFound = _compiledPatterns[term].Matches(content).Count;
            Terms[term] = numFound > 0 ? 1 : 0;
        }
        return Terms;
    }
}
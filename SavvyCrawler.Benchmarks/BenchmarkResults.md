# TermCounter.Examine() — Baseline vs Optimized

**Optimized:** Pre-compiled regexes (Dictionary + RegexOptions.Compiled) + Matches() for counting.  
**Benchmark:** One TermCounter per scenario (Setup), one Examine(content) per iteration. Short job.

| Content | Terms | Baseline (ms) | Optimized (ms) | % Improved |
|---------|-------|----------------|----------------|------------|
| 10 KB   | 10    | 1.20           | 0.582          | **51.5%**  |
| 10 KB   | 100   | 12.7           | 6.32           | **50.2%**  |
| 10 KB   | 500   | 63.4           | 30.3           | **52.2%**  |
| 10 KB   | 1,000 | 126.5          | 61.2           | **51.6%**  |
| 50 KB   | 10    | 6.26           | 3.01           | **51.9%**  |
| 50 KB   | 100   | 63.8           | 31.8           | **50.2%**  |
| 50 KB   | 500   | 316            | 154            | **51.3%**  |
| 50 KB   | 1,000 | 635            | 186            | **70.7%**  |
| 100 KB  | 10    | 12.3           | 5.77           | **53.1%**  |
| 100 KB  | 100   | 129            | 63.6           | **50.7%**  |
| 100 KB  | 500   | 645            | 235            | **63.6%**  |
| 100 KB  | 1,000 | 1,300          | 377            | **71.0%**  |

**Summary:** All 12 scenarios improved; no declines. Improvement 50–71% (largest on 500–1000 terms and 50–100 KB content).

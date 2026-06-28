using System;
using System.Collections.Generic;
using System.Linq;


namespace AppCode.Models
{
  public class ThingStats<T>
  {
    public ThingStats(IEnumerable<T> all, IEnumerable<T> relevant, Func<ThingStats<T>, string> summaryFunc = null, Func<ThingStats<T>, string> summaryInfoFunc = null)
    {
      All = all.ToList();
      Relevant = relevant.ToList();
      _summaryFunc = summaryFunc ?? _summaryFunc;
      _summaryInfoFunc = summaryInfoFunc ?? _summaryInfoFunc;
    }

    public List<T> All { get; }

    public List<T> Relevant { get; }

    public string Summary => _summaryFunc?.Invoke(this) ?? "";
    private readonly Func<ThingStats<T>, string> _summaryFunc = (ts) => $"{ts.Relevant.Count}/{ts.All.Count}";

    public string SummaryInfo => _summaryInfoFunc?.Invoke(this) ?? "";
    private readonly Func<ThingStats<T>, string> _summaryInfoFunc = (ts) => $"All: {ts.All.Count}, Relevant: {ts.Relevant.Count}, Summary: {ts.Summary}";
  }

}
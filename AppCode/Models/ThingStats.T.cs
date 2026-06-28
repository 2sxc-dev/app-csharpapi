using System;
using System.Collections.Generic;
using System.Linq;


namespace AppCode.Models
{
  public class ThingStats<T>
  {
    internal ThingStats() { }

    public ThingStats(IEnumerable<T> all, IEnumerable<T> relevant, Func<ThingStats<T>, string> summaryFunc = null, Func<ThingStats<T>, string> summaryInfoFunc = null)
    {
      All = all.ToList();
      Relevant = relevant.ToList();
    }

    public List<T> All { get; internal set; }

    public List<T> Relevant { get; internal set; }

    protected virtual string SummaryMaker => $"{Relevant.Count}/{All.Count}";
    protected virtual string SummaryInfoMaker => string.Join("\n", SummaryInfoLines);
    protected virtual string[] SummaryInfoLines => new string[0];

    public string Summary => SummaryMaker ?? "";

    public string SummaryInfo => string.Join("\n", SummaryInfoLines);

    public static string GetIconOrNumber(int countVisible, int countRelevant)
    {
      return countVisible == 0
        ? Constants.VisEditHiddenOnly
        : countVisible == countRelevant
          ? Constants.VisEditVisible
          : $"{countVisible}";
    }
  }

  public class StatisticsWithVisible<T>: ThingStats<T>
  {
    public int CountVisible { get; internal set; }
    protected override string SummaryMaker => $"{GetIconOrNumber(CountVisible, Relevant.Count)}/{Relevant.Count}/{All.Count}";

    protected override string[] SummaryInfoLines => new []
    {
      $"Statistics",
      "",
      $"{CountVisible:00} Visible in IntelliSense",
      $"{Relevant.Count:00} Relevant",
      $"{All.Count:00} Total"
    };
  }

}
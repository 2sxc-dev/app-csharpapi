using System.Collections.Generic;
using System.Linq;


namespace ThisApp.Code
{
  public class ThingStats<T>
  {
    public ThingStats(IEnumerable<T> all, IEnumerable<T> relevant)
    {
      All = all.ToList();
      Relevant = relevant.ToList();
    }

    public List<T> All { get; }

    public List<T> Relevant { get; }

    public string Summary => $"{Relevant.Count}/{All.Count}"; // {typeof(T).Name}s";
  }

}
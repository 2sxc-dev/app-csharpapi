namespace AppCode.Data
{
  public partial class RuleNamespace
  {
    public RuleNamespace Setup(bool shared = false) {
      SharedRule = shared;
      return this;
    }

    public bool SharedRule { get; private set; }
  }

}

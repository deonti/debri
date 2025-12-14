namespace Debri.Pools
{
  /// <summary>
  /// An interface that allows to prewarm a pool.
  /// </summary>
  public interface IPrewarmable
  {
    /// <summary>Prewarms the pool.</summary>
    void Prewarm(int count);
  }
}
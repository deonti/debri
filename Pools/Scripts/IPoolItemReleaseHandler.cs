namespace Debri.Pools
{
  /// <summary>
  /// Interface for handling pool item release.
  /// </summary>
  /// <remarks>
  /// Implement this interface in your own component to handle pool item release.
  /// </remarks>
  public interface IPoolItemReleaseHandler
  {
    /// <summary>
    /// Called when an item is released to one of the pools from <see cref="GlobalPools"/>.
    /// </summary>
    void OnRelease()
    {
    }
  }
}
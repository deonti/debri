namespace Debri.Pools
{
  /// <summary>
  /// Interface for handling pool item get.
  /// </summary>
  /// <remarks>
  /// Implement this interface in your own component to handle pool item get.
  /// </remarks>
  public interface IPoolItemGetHandler
  {
    /// <summary>
    /// Called when an item is gotten from one of the pools from <see cref="GlobalPools"/>.
    /// </summary>
    void OnGet();
  }
}
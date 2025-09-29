namespace Debri.Common
{
  /// <summary>
  /// Defines the behavior when the Dispose method is called more than once.
  /// </summary>
  public enum RepeatedDisposeMode
  {
    /// <summary>
    /// An exception is thrown on subsequent calls to Dispose to indicate an error.
    /// </summary>
    Throw,

    /// <summary>
    /// Subsequent calls to Dispose are ignored. Nothing happens.
    /// </summary>
    Ignore
  }
}
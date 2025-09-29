using System;
using System.ComponentModel;

namespace Debri.Common
{
  /// <summary>
  /// Simple anonymous <see cref="IDisposable"/> implementation.
  /// </summary>
  public sealed class Disposable : IDisposable
  {
    private Action _disposer;
    private readonly RepeatedDisposeMode _mode;
    private bool _isDisposed;

    /// <summary>
    /// Creates a new instance of <see cref="Disposable"/>.
    /// </summary>
    /// <param name="disposer">The action to execute when <see cref="Dispose"/> is called.</param>
    /// <param name="mode">Defines how repeated calls to <see cref="Dispose"/> should be handled.</param>
    public Disposable(Action disposer, RepeatedDisposeMode mode = RepeatedDisposeMode.Throw)
    {
      _disposer = disposer;
      _mode = mode;
    }

    public void Dispose()
    {
      if (_isDisposed)
        switch (_mode)
        {
          case RepeatedDisposeMode.Ignore:
            break;
          case RepeatedDisposeMode.Throw:
            throw new ObjectDisposedException(nameof(Disposable));
          default:
            throw new InvalidEnumArgumentException(nameof(_mode), (int)_mode, typeof(RepeatedDisposeMode));
        }

      _disposer();
      _disposer = null; // Break possible reference cycle to avoid potential memory leaks.
      _isDisposed = true;
    }
  }
}
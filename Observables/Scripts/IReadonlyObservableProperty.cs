using System;

namespace Debri.Observables
{
  /// <summary>
  /// Read-only observable property.
  /// </summary>
  public interface IReadonlyObservableProperty<out TValue> : IObservable<TValue>
  {
    TValue Value { get; }
  }
}
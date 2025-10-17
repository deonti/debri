using System;
using System.Collections.Generic;

namespace Debri.Observables
{
  /// <summary>
  /// Base class for simple observable implementations.
  /// </summary>
  public abstract class Observable<TValue> : IObservable<TValue>
  {
    protected IReadOnlyList<IObserver<TValue>> Observers => _observers;

    private readonly List<IObserver<TValue>> _observers = new();

    public virtual IDisposable Subscribe(IObserver<TValue> observer) =>
      new Subscription(this, observer);

    private class Subscription : IDisposable
    {
      private readonly Observable<TValue> _observable;
      private readonly IObserver<TValue> _observer;

      public Subscription(Observable<TValue> observable, IObserver<TValue> observer)
      {
        _observable = observable;
        _observer = observer;
        _observable._observers.Add(_observer);
      }

      public void Dispose() =>
        _observable._observers.Remove(_observer);
    }
  }
}
using System;
using System.Collections.Generic;
using Debri.Observables.Internals;

namespace Debri.Observables
{
  /// <summary>
  /// Base class for simple observable implementations.
  /// </summary>
  public abstract class Observable<TValue> : IObservable<TValue>
  {
    [Obsolete("Use Invoke[OnCompleted|OnError|OnNext] methods instead")]
    protected IReadOnlyList<IObserver<TValue>> Observers => _observers;

    private readonly ObserverList<TValue> _observers = new();

    public virtual IDisposable Subscribe(IObserver<TValue> observer) =>
      new Subscription(this, observer);

    protected void InvokeOnCompleted()
    {
      _observers.Lock();
      try
      {
        foreach (IObserver<TValue> observer in _observers)
          observer.OnCompleted();
      }
      finally
      {
        _observers.Unlock();
      }
    }

    protected void InvokeOnError(Exception error)
    {
      _observers.Lock();
      try
      {
        foreach (IObserver<TValue> observer in _observers)
          observer.OnError(error);
      }
      finally
      {
        _observers.Unlock();
      }
    }

    protected void InvokeOnNext(TValue value)
    {
      _observers.Lock();
      try
      {
        foreach (IObserver<TValue> observer in _observers)
          observer.OnNext(value);
      }
      finally
      {
        _observers.Unlock();
      }
    }

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
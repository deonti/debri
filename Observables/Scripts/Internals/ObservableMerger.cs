using System;
using System.Collections.Generic;
using System.Linq;

namespace Debri.Observables.Internals
{
  internal class ObservableMerger<TValue> : IObservable<TValue>
  {
    private readonly IObservable<TValue>[] _sources;

    public ObservableMerger(IEnumerable<IObservable<TValue>> sources) =>
      _sources = sources.ToArray();

    public IDisposable Subscribe(IObserver<TValue> observer) =>
      new Subscription(this, observer);

    private class Subscription : IDisposable
    {
      private readonly Stack<IDisposable> _subscriptions = new();

      public Subscription(ObservableMerger<TValue> merger, IObserver<TValue> observer)
      {
        foreach (IObservable<TValue> observable in merger._sources)
          _subscriptions.Push(observable.Subscribe(observer));
      }

      public void Dispose()
      {
        while (_subscriptions.Count > 0)
          _subscriptions.Pop().Dispose();
      }
    }
  }
}
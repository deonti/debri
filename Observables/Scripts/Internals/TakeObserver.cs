using System;

namespace Debri.Observables.Internals
{
  internal class TakeObserver<TValue> : IObserver<TValue>
  {
    private readonly IObserver<TValue> _observer;
    private int _count;

    public TakeObserver(IObserver<TValue> observer, int count)
    {
      _observer = observer;
      _count = count;
    }

    public void OnNext(TValue value)
    {
      if (_count-- <= 0) return;

      _observer.OnNext(value);
      if (_count != 0) return;

      _observer.OnCompleted();
    }

    public void OnCompleted() =>
      _observer.OnCompleted();

    public void OnError(Exception error) =>
      _observer.OnError(error);
  }
}
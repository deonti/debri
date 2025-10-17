using System;

namespace Debri.Observables.Internals
{
  internal class SkipObserver<TValue> : IObserver<TValue>
  {
    private readonly IObserver<TValue> _observer;
    private int _count;

    public SkipObserver(IObserver<TValue> observer, int count)
    {
      _observer = observer;
      _count = count;
    }

    public void OnNext(TValue value)
    {
      switch (_count)
      {
        case > 0:
          _count--;
          break;
        default:
          _observer.OnNext(value);
          break;
      }
    }

    public void OnCompleted() =>
      _observer.OnCompleted();

    public void OnError(Exception error) =>
      _observer.OnError(error);
  }
}
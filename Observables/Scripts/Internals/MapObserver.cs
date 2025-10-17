using System;

namespace Debri.Observables.Internals
{
  internal class MapObserver<TSource, TValue> : IObserver<TSource>
  {
    private readonly IObserver<TValue> _observer;
    private readonly Func<TSource, TValue> _mapper;

    public MapObserver(IObserver<TValue> observer, Func<TSource, TValue> mapper)
    {
      _observer = observer;
      _mapper = mapper;
    }

    public void OnNext(TSource value) =>
      _observer.OnNext(_mapper(value));

    public void OnCompleted() =>
      _observer.OnCompleted();

    public void OnError(Exception error) =>
      _observer.OnError(error);
  }
}
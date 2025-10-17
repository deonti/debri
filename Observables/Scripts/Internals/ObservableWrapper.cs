using System;

namespace Debri.Observables.Internals
{
  internal class ObservableWrapper<TSource, TValue> : IObservable<TValue>
  {
    public delegate IObserver<TSource> ObserverWrapperFactory(IObserver<TValue> observer);

    private readonly IObservable<TSource> _source;
    private readonly ObserverWrapperFactory _observerFactory;

    public ObservableWrapper(IObservable<TSource> source, ObserverWrapperFactory observerFactory)
    {
      _source = source;
      _observerFactory = observerFactory;
    }

    public IDisposable Subscribe(IObserver<TValue> observer) =>
      _source.Subscribe(_observerFactory(observer));
  }

  internal class ObservableWrapper<TValue> : ObservableWrapper<TValue, TValue>
  {
    public ObservableWrapper(IObservable<TValue> source, ObserverWrapperFactory observerFactory)
      : base(source, observerFactory)
    {
    }
  }
}
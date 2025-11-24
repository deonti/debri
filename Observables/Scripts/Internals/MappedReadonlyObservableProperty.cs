using System;

namespace Debri.Observables.Internals
{
  internal class MappedReadonlyObservableProperty<TSourceValue, TValue> : Observable<TValue>, IReadonlyObservableProperty<TValue>, IDisposable
  {
    private IDisposable _subscription;

    public TValue Value { get; private set; }

    public MappedReadonlyObservableProperty(IReadonlyObservableProperty<TSourceValue> source, Func<TSourceValue, TValue> sourceValueToValue) =>
      _subscription = source.Subscribe(new SourceObserver(this, sourceValueToValue));

    public override IDisposable Subscribe(IObserver<TValue> observer)
    {
      IDisposable subscription = base.Subscribe(observer);
      observer.OnNext(Value);
      return subscription;
    }

    public void Dispose()
    {
      if (_subscription is null) return;

      _subscription.Dispose();
      _subscription = null;
    }

    private class SourceObserver : IObserver<TSourceValue>
    {
      private readonly MappedReadonlyObservableProperty<TSourceValue, TValue> _property;
      private readonly Func<TSourceValue, TValue> _sourceValueToValue;

      public SourceObserver(MappedReadonlyObservableProperty<TSourceValue, TValue> property, Func<TSourceValue, TValue> sourceValueToValue)
      {
        _property = property;
        _sourceValueToValue = sourceValueToValue;
      }

      public void OnCompleted()
      {
        foreach (IObserver<TValue> observer in _property.Observers)
          observer.OnCompleted();
        _property.Dispose();
      }

      public void OnError(Exception error)
      {
        foreach (IObserver<TValue> observer in _property.Observers)
          observer.OnError(error);
      }

      public void OnNext(TSourceValue sourceValue)
      {
        TValue value = _sourceValueToValue(sourceValue);
        if (Equals(_property.Value, value)) return;

        _property.Value = value;
        foreach (IObserver<TValue> observer in _property.Observers)
          observer.OnNext(_property.Value);
      }
    }
  }
}
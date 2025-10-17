using System;
using System.Collections.Generic;
using UnityEngine;

namespace Debri.Observables
{
  /// <summary>
  /// Simple implementation of observable property.
  /// </summary>
  /// <remarks>
  /// This class supports Unity serialization.
  /// </remarks>
  [Serializable]
  public class ObservableProperty<TValue> : Observable<TValue>, IObservableProperty<TValue>, ISerializationCallbackReceiver
  {
    public TValue Value
    {
      get => _currentValue;
      set
      {
        if (!Validate(_currentValue, ref value)) return;

        _currentValue = value;
        foreach (IObserver<TValue> observer in Observers)
          observer.OnNext(value);
      }
    }

    [SerializeField] private TValue _value;

    private TValue _currentValue;

    public ObservableProperty()
    {
    }

    public ObservableProperty(TValue value) =>
      _currentValue = value;

    public override IDisposable Subscribe(IObserver<TValue> observer)
    {
      IDisposable subscription = base.Subscribe(observer);
      observer.OnNext(_currentValue);
      return subscription;
    }

    protected virtual bool Validate(in TValue currentValue, ref TValue newValue) =>
      !EqualityComparer<TValue>.Default.Equals(currentValue, newValue);

    void ISerializationCallbackReceiver.OnBeforeSerialize() =>
      _value = Value;

    void ISerializationCallbackReceiver.OnAfterDeserialize() =>
      Value = _value;
  }
}
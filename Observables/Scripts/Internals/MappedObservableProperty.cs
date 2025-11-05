using System;

namespace Debri.Observables.Internals
{
  internal class MappedObservableProperty<TSourceValue, TValue> : MappedReadonlyObservableProperty<TSourceValue, TValue>, IObservableProperty<TValue>
  {
    private readonly IObservableProperty<TSourceValue> _source;
    private readonly Func<TValue, TSourceValue> _valueToSourceValue;

    public new TValue Value
    {
      get => base.Value;
      set => _source.Value = _valueToSourceValue(value);
    }

    public MappedObservableProperty(
      IObservableProperty<TSourceValue> source,
      Func<TSourceValue, TValue> sourceValueToValue,
      Func<TValue, TSourceValue> valueToSourceValue
    ) : base(source, sourceValueToValue)
    {
      _source = source;
      _valueToSourceValue = valueToSourceValue;
    }
  }
}
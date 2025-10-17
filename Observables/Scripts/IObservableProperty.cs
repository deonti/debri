namespace Debri.Observables
{
  /// <summary>
  /// Observable property.
  /// </summary>
  public interface IObservableProperty<TValue> : IReadonlyObservableProperty<TValue>
  {
    new TValue Value { get; set; }
  }
}
using System;
using System.Collections.Generic;
using Debri.Observables.Internals;

namespace Debri.Observables
{
  public static class ObservableUtils
  {
    /// <summary>
    /// Subscribe to an observable with an anonymous observer.
    /// </summary>
    /// <param name="observable"> Observable to subscribe to.</param>
    /// <param name="onNext"> Handler for the next value.</param>
    /// <param name="onError"> Handler for the error.</param>
    /// <param name="onCompleted"> Handler for the completion.</param>
    /// <returns>
    /// A disposable that unsubscribes from the observable.
    /// </returns>
    public static IDisposable Subscribe<TValue>(this IObservable<TValue> observable, Action<TValue> onNext = null,
      Action<Exception> onError = null, Action onCompleted = null) =>
      observable.Subscribe(new AnonymousObserver<TValue>(onNext, onError, onCompleted));

    /// <summary>
    /// Skip the first n values of an observable.
    /// </summary>
    /// <param name="observable"> Observable to skip values from.</param>
    /// <param name="count"> Number of values to skip.</param>
    /// <returns> An observable that skips the first n values.</returns>
    public static IObservable<TValue> Skip<TValue>(this IObservable<TValue> observable, int count) =>
      new ObservableWrapper<TValue>(observable, observer => new SkipObserver<TValue>(observer, count));

    /// <summary>
    /// Take the first n values of an observable.
    /// </summary>
    /// <param name="observable"> Observable to take values from.</param>
    /// <param name="count"> Number of values to take.</param>
    /// <returns> An observable that takes the first n values.</returns>
    public static IObservable<TValue> Take<TValue>(this IObservable<TValue> observable, int count) =>
      new ObservableWrapper<TValue>(observable, observer => new TakeObserver<TValue>(observer, count));

    /// <summary>
    /// Map the values of an observable.
    /// </summary>
    /// <param name="observable"> Observable to map values from.</param>
    /// <param name="mapper"> Mapper function.</param>
    /// <returns> An observable that maps the values.</returns>
    public static IObservable<TValue> Map<TSource, TValue>(this IObservable<TSource> observable, Func<TSource, TValue> mapper) =>
      new ObservableWrapper<TSource, TValue>(observable, observer => new MapObserver<TSource, TValue>(observer, mapper));

    /// <summary>
    /// Merge multiple observables into one.
    /// </summary>
    /// <param name="observables"> Observables to merge.</param>
    /// <returns> An observable that merges the observables.</returns>
    public static IObservable<TValue> Merge<TValue>(this IEnumerable<IObservable<TValue>> observables) =>
      new ObservableMerger<TValue>(observables);

    /// <summary>
    /// Map readonly observable property to another type.
    /// </summary>
    /// <param name="property"> Source observable property.</param>
    /// <param name="sourceValueToValue"> Mapper function.</param>
    /// <returns> An observable property that maps the values.</returns>
    public static IReadonlyObservableProperty<TValue> Map<TSourceValue, TValue>(
      this IReadonlyObservableProperty<TSourceValue> property,
      Func<TSourceValue, TValue> sourceValueToValue) =>
      new MappedReadonlyObservableProperty<TSourceValue, TValue>(property, sourceValueToValue);

    /// <summary>
    /// Map observable property to another type.</summary>
    /// <param name="property"> Source observable property.</param>
    /// <param name="sourceValueToValue"> Mapper function.</param>
    /// <param name="valueToSourceValue"> Mapper function.</param>
    /// <returns> An observable property that maps the values.</returns>
    public static IObservableProperty<TValue> Map<TSourceValue, TValue>(
      this IObservableProperty<TSourceValue> property,
      Func<TSourceValue, TValue> sourceValueToValue, Func<TValue, TSourceValue> valueToSourceValue) =>
      new MappedObservableProperty<TSourceValue, TValue>(property, sourceValueToValue, valueToSourceValue);
  }
}
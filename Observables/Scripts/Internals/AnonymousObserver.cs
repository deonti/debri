using System;

namespace Debri.Observables.Internals
{
  internal class AnonymousObserver<TValue> : IObserver<TValue>
  {
    private readonly Action<TValue> _onNext;
    private readonly Action<Exception> _onError;
    private readonly Action _onCompleted;

    public AnonymousObserver(Action<TValue> onNext = null, Action<Exception> onError = null,
      Action onCompleted = null)
    {
      _onNext = onNext;
      _onError = onError;
      _onCompleted = onCompleted;
    }

    public void OnNext(TValue value) =>
      _onNext(value);

    public void OnError(Exception error) =>
      _onError?.Invoke(error);

    public void OnCompleted() =>
      _onCompleted?.Invoke();
  }
}
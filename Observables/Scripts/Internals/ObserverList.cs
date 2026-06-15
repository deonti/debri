using System;
using System.Collections;
using System.Collections.Generic;

namespace Debri.Observables.Internals
{
  internal class ObserverList<TValue> : IReadOnlyList<IObserver<TValue>>
  {
    private readonly List<IObserver<TValue>> _observers = new();
    private readonly Queue<ObserversAction> _unlockActions = new();
    private int _lockCounter;

    public int Count =>
      _observers.Count;

    public IObserver<TValue> this[int index] =>
      _observers[index];

    public IEnumerator<IObserver<TValue>> GetEnumerator() =>
      _observers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
      _observers.GetEnumerator();

    public void Lock() =>
      _lockCounter++;

    public void Unlock()
    {
      _lockCounter--;
      if (_lockCounter > 0) return;

      while (_unlockActions.TryDequeue(out ObserversAction observersAction))
        observersAction.Invoke();
    }

    public void Add(IObserver<TValue> observer)
    {
      if (_lockCounter > 0)
      {
        _unlockActions.Enqueue(new ObserversAddAction(_observers, observer));
        return;
      }

      _observers.Add(observer);
    }

    public void Remove(IObserver<TValue> observer)
    {
      if (_lockCounter > 0)
      {
        _unlockActions.Enqueue(new ObserversRemoveAction(_observers, observer));
        return;
      }

      _observers.Remove(observer);
    }

    private abstract class ObserversAction
    {
      protected readonly List<IObserver<TValue>> Observers;
      protected readonly IObserver<TValue> Observer;

      protected ObserversAction(List<IObserver<TValue>> observers, IObserver<TValue> observer)
      {
        Observers = observers;
        Observer = observer;
      }

      public abstract void Invoke();
    }

    private class ObserversAddAction : ObserversAction
    {
      public ObserversAddAction(List<IObserver<TValue>> observers, IObserver<TValue> observer)
        : base(observers, observer)
      {
      }

      public override void Invoke() =>
        Observers.Add(Observer);
    }

    private class ObserversRemoveAction : ObserversAction
    {
      public ObserversRemoveAction(List<IObserver<TValue>> observers, IObserver<TValue> observer)
        : base(observers, observer)
      {
      }

      public override void Invoke() =>
        Observers.Remove(Observer);
    }
  }
}
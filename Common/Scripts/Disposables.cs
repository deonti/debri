using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Debri.Common
{
  /// <summary>
  /// A collection of disposable objects that can be disposed together.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Elements are disposed in the reverse order they were added.
  /// </para>
  /// <para>
  /// When elements are disposed by calling <see cref="DisposeAll"/>, they are removed from the collection.
  /// </para>
  /// </remarks>
  /// <example>
  /// <para>
  /// Manage a set of subscriptions.
  /// <code>
  /// class HealthWidget : MonoBehaviour
  /// {
  ///   [SerializeField] private Health _health;
  ///   [SerializeField] private Button _healButton;
  ///   // ...
  ///   
  ///   private Disposables _subscriptions = new Disposables();
  ///   
  ///   void OnEnable()
  ///   {
  ///     // Add regular subscription
  ///     _health.OnChanged += UpdateWidget;
  ///     _subscriptions.Add(() => _health.OnChanged -= UpdateWidget);
  ///     
  ///     // Add conditional subscription
  ///     if (_healButton)
  ///     {
  ///       _healButton.onClick.AddListener(Heal);
  ///       _subscriptions.Add(_healButton.onClick.RemoveListener(Heal));
  ///     }
  ///     
  ///     // ...
  ///   }
  ///   
  ///   void OnDisable()
  ///   {
  ///     // Unsubscribe all added subscriptions
  ///     _subscriptions.Dispose();
  ///   }
  ///   
  ///   // ...
  /// </code>
  /// </para>
  /// </example>
  public sealed class Disposables
  {
    private readonly Stack<Action> _items = new();

    /// <summary>
    /// Releases all disposable resources managed by this instance.
    /// </summary>
    /// <remarks>
    /// The collection is completely cleared during this call.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DisposeAll()
    {
      while (_items.TryPop(out Action action))
        action();
    }

    /// <summary>
    /// Adds a dispose action to the collection.
    /// </summary>
    /// <param name="disposer">The action to execute when disposing.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(Action disposer) =>
      _items.Push(disposer);

    /// <summary>
    /// Adds a disposable object to the collection.
    /// </summary>
    /// <param name="disposable">The disposable object to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(IDisposable disposable) =>
      Add(disposable.Dispose);
  }
}
using System;
using System.Collections.Generic;
using Debri.Common;
using Debri.PlayerLoop.Internal;
using UnityEngine;
using UnityPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoopSystem;
using UnityPlayerLoop = UnityEngine.LowLevel.PlayerLoop;

namespace Debri.PlayerLoop
{
  public static class PlayerLoopManager
  {
    internal static IEnumerable<PlayerLoopSystem> Systems => _systems;

    private static readonly List<PlayerLoopSystem> _systems = new();
    private static bool _isSubsystemListPrepared;

    /// <summary> Adds a subsystem to a parent system </summary>
    /// <param name="handlerInvoker"> Callback that will be invoked for calling the handler </param>
    /// <typeparam name="TParentSystem"> Type of the parent system </typeparam>
    /// <typeparam name="TSubsystem"> Type identifier of added Subsystem </typeparam>
    /// <typeparam name="THandler"> Interface type that contains subsystem handler method </typeparam>
    /// <example>
    /// <code>
    ///   public readonly struct MyPreUpdateSubsystem
    ///   {
    ///     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    ///     private static void Initialize()
    ///     {
    ///       PlayerLoopManager
    ///         .AddSubsystem&lt;UnityEngine.PlayerLoop.PreUpdate, MyPreUpdateSubsystem, IMyPreUpdateHandler&gt;(
    ///           handlerInvoker: static handler => handler.MyPreUpdate());
    ///     }
    ///   }
    /// 
    ///   public interface IMyPreUpdateHandler
    ///   {
    ///     void MyPreUpdate();
    ///   }
    /// </code>
    /// </example>
    /// <seealso cref="RegisterHandlers"/>
    public static void AddSubsystem<TParentSystem, TSubsystem, THandler>(Invoker<THandler> handlerInvoker)
    {
      if (_isSubsystemListPrepared)
      {
        Debug.LogError("Too late to add subsytems, use [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)].");
        return;
      }

      _systems.Add(new PlayerLoopSystem<THandler, TParentSystem, TSubsystem>(handlerInvoker));
    }

    /// <summary> Register handlers implemented by owner in all compatible subsystems </summary>
    /// <param name="handlersOwner"> Object that implements subsystem handlers </param>
    /// <returns> Registration token </returns>
    /// <example>
    /// <code>
    ///   public class MyBehaviour : MonoBehaviour, IMyPreUpdateHandler
    ///   {
    ///     private IDisposable _registration;
    /// 
    ///     private void OnEnable()
    ///     {
    ///       _registration = PlayerLoopManager.RegisterHandlers(this);
    ///     }
    /// 
    ///     private void OnDisable()
    ///     {
    ///       _registration.Dispose();
    ///     }
    /// 
    ///     public void MyPreUpdate()
    ///     {
    ///       Debug.Log("MyPreUpdate was called");
    ///     }
    ///   }
    /// </code>
    /// </example>
    /// <seealso cref="AddSubsystem{TParentSystem,TSubsystem,THandler}"/>
    public static IDisposable RegisterHandlers(object handlersOwner)
    {
      if (!_isSubsystemListPrepared)
      {
        Debug.LogError("Too early for register handlers, list of subsystems has not been prepared.");
        return Disposable.Dummy;
      }

      return HandlersRegistration.Pool.Get(handlersOwner);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Initialize()
    {
      _isSubsystemListPrepared = true;
      UnityPlayerLoopSystem playerLoop = UnityPlayerLoop.GetCurrentPlayerLoop();
      {
        foreach (PlayerLoopSystem system in _systems)
          system.AddTo(ref playerLoop);
      }
      UnityPlayerLoop.SetPlayerLoop(playerLoop);
    }

#if UNITY_EDITOR
    [Common.Editor.FinalizeOnPlayModeExitMethod]
    private static void FinalizeOnPlayModeExit()
    {
      _isSubsystemListPrepared = false;
      UnityPlayerLoopSystem playerLoop = UnityPlayerLoop.GetCurrentPlayerLoop();
      {
        foreach (PlayerLoopSystem system in _systems)
        {
          system.SafeDispose();
          system.RemoveFrom(ref playerLoop);
        }
      }
      UnityPlayerLoop.SetPlayerLoop(playerLoop);

      _systems.Clear();
    }

    private static void SafeDispose(this IDisposable disposable)
    {
      try
      {
        disposable.Dispose();
      }
      catch (Exception e)
      {
        Debug.LogError(e);
      }
    }
#endif
  }
}
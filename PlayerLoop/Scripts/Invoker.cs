namespace Debri.PlayerLoop
{
  /// <summary> Delegate for invoking a subsystems handlers </summary>
  /// <typeparam name="THandler"> Interface type that contains subsystem handler method </typeparam>
  public delegate void Invoker<in THandler>(THandler handler);
}
namespace Mergeburgers.Events
{
  /// <summary>
  /// Срабатывает когда MergeResolver применил хотя бы одно слияние (2+ плитки → 1).
  /// Не для merge типа "просто движение" (1 плитка переехала).
  /// </summary>
  public sealed class MergeOccurredSignal
  {
  }
}
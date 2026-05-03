using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mergeburgers.Tools
{
  /// <summary>
  /// Делает Unity AsyncOperation awaitable.
  /// После добавления этого файла можно писать:
  ///   await SceneManager.LoadSceneAsync("MainMenu");
  ///   await Resources.LoadAsync<Sprite>("path");
  ///   await someAssetBundle.LoadAssetAsync<...>();
  ///
  /// Без этого extension'а компилятор не знает, как awaitнуть AsyncOperation
  /// (Unity-класс из дотаска эпохи).
  /// </summary>
  public static class AsyncOperationExtensions
  {
    public static AsyncOperationAwaiter GetAwaiter(this AsyncOperation op)
      => new AsyncOperationAwaiter(op);
  }

  public readonly struct AsyncOperationAwaiter : INotifyCompletion
  {
    private readonly AsyncOperation _op;

    public AsyncOperationAwaiter(AsyncOperation op) => _op = op;

    public bool IsCompleted => _op.isDone;

    public void GetResult()
    {
      /* AsyncOperation не возвращает значение */
    }

    public void OnCompleted(System.Action continuation)
      => _op.completed += _ => continuation();
  }
}
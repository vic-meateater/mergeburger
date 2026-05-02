#if UNITY_EDITOR
using System.Threading.Tasks;
using UnityEngine;

namespace Mergeburgers.Yandex.Editor
{
  public class EditorLeaderboardService : ILeaderboardService
  {
    public async Task SubmitScoreAsync(string leaderboardId, int score)
    {
      await Task.Delay(50);
      Debug.Log($"[EditorLeaderboard] Score submitted: {leaderboardId}={score}");
    }

    public async Task<int> GetPlayerScoreAsync(string leaderboardId)
    {
      await Task.Delay(50);
      Debug.Log($"[EditorLeaderboard] Score requested: {leaderboardId}");
      return 0;
    }
  }
}
#endif
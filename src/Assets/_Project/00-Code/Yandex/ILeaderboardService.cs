using System.Threading.Tasks;

namespace Mergeburgers.Yandex
{
  public interface ILeaderboardService
  {
    Task SubmitScoreAsync(string leaderboardId, int score);
    Task<int> GetPlayerScoreAsync(string leaderboardId);
  }
}
using System.Threading.Tasks;

namespace Mergeburgers.Yandex
{
  public interface ICloudSaveService
  {
    Task<string> LoadAsync();
    Task SaveAsync(string json);
  }
}
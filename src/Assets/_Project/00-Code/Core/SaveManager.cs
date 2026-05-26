using System.Threading.Tasks;
using Mergeburgers.Yandex;
using UnityEngine;

namespace Mergeburgers.Core
{
  /// <summary>
  /// Управляет загрузкой/сохранением состояния игры через ICloudSaveService.
  /// Знает о версионировании и миграции форматов.
  /// Plain C# класс, инжектится через конструктор (не MonoBehaviour — ему
  /// не нужны Update/Awake, он переживает между сценами как Zenject Singleton).
  /// </summary>
  public sealed class SaveManager
  {
    public const int CurrentVersion = 1;

    private readonly ICloudSaveService _cloudSave;

    public GameSave Current { get; private set; }

    public SaveManager(ICloudSaveService cloudSave)
    {
      _cloudSave = cloudSave;
    }

    public async Task LoadAsync()
    {
      var json = await _cloudSave.LoadAsync();

      if (string.IsNullOrEmpty(json))
      {
        Current = GameSave.CreateDefault();
        return;
      }

      try
      {
        var loaded = JsonUtility.FromJson<GameSave>(json);

        if (loaded == null || loaded.version < CurrentVersion)
        {
          Current = Migrate(loaded);
        }
        else
        {
          Current = loaded;
        }
      }
      catch (System.Exception e)
      {
        Current = GameSave.CreateDefault();
      }
    }

    public async Task SaveAsync()
    {
      if (Current == null)
      {
        return;
      }

      Current.version = CurrentVersion;
      var json = JsonUtility.ToJson(Current);
      await _cloudSave.SaveAsync(json);
    }

    /// <summary>
    /// Миграция старых сейвов в актуальный формат.
    /// Сейчас тривиальная — на v1 ничего конвертировать не от чего.
    /// На v2 здесь будет логика типа "если version==1 → переименовать поле X".
    /// </summary>
    private GameSave Migrate(GameSave old)
    {
      // На текущем этапе любая старая версия = новый default.
      // Когда будет реальная v2 — добавим логику типа:
      // if (old?.version == 1) { /* конвертация */ }
      return GameSave.CreateDefault();
    }
  }
}
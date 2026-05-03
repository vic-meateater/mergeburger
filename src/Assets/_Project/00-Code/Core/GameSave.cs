using System;
using System.Collections.Generic;

namespace Mergeburgers.Core
{
  /// <summary>
  /// Сериализуемое состояние игры. Формат — JSON через JsonUtility.
  /// При изменении структуры — обязательно бампать VERSION и добавлять
  /// миграцию в SaveManager.LoadAsync().
  ///
  /// Все поля public — JsonUtility не видит private даже с [SerializeField].
  /// Это плата за встроенный сериализатор Unity. Newtonsoft.Json умнее,
  /// но тащить его на WebGL ради красоты — не стоит.
  /// </summary>
  [Serializable]
  public class GameSave
  {
    public int version;
    public int coins;
    public int energy;
    public long energyLastFullTime; // unix seconds, для расчёта восстановления
    public long lastSessionEndTime; // unix seconds, для расчёта idle-дохода
    public List<string> unlockedRecipes;
    public List<string> ownedUpgrades;
    public int highScore;
    public int sessionsCount;
    public bool tutorialPassed;

    public static GameSave CreateDefault()
    {
      return new GameSave
      {
        version = SaveManager.CurrentVersion,
        coins = 0,
        energy = 5,
        energyLastFullTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        lastSessionEndTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        unlockedRecipes = new List<string> {"bun"}, // стартовый ингредиент
        ownedUpgrades = new List<string>(),
        highScore = 0,
        sessionsCount = 0,
        tutorialPassed = false
      };
    }
  }
}
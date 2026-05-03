# День 5 — SaveManager и JSON-сериализация

**Цель дня:** GameSave data-класс готов, SaveManager умеет загружать/сохранять через ICloudSaveService, версионирование заложено, в Bootstrap идёт реальная загрузка сейва. После перезапуска сцены значения сохраняются.

**Время:** ~3 часа.

---

## Блок 1 — GameSave data-класс (30 мин)

- [x] Создан `Assets/_Project/00-Code/Core/GameSave.cs` (код из чата)
- [x] Класс помечен `[System.Serializable]`
- [x] Все поля `public` (без `[SerializeField]` private)
- [x] Реализован static-метод `CreateDefault()`
- [x] Console чистая, нет ошибок компиляции

> **Зачем все поля public:** JsonUtility (встроенный сериализатор Unity) видит ТОЛЬКО public-поля. Это его ограничение, плата за то, что он работает на WebGL без аллокаций и весит 0KB (часть Unity). Альтернатива — Newtonsoft.Json (умнее, но +200KB к билду).

---

## Блок 2 — SaveManager (40 мин)

- [x] Создан `Assets/_Project/00-Code/Core/SaveManager.cs` (код из чата)
- [x] Constructor принимает `ICloudSaveService cloudSave`
- [x] Свойство `Current` имеет `private set`
- [x] Метод `LoadAsync()`:
  - [x] Если JSON пуст → создаёт CreateDefault
  - [x] Если парсинг упал → создаёт CreateDefault, логирует ошибку
  - [x] Если version < CurrentVersion → вызывает Migrate
- [x] Метод `SaveAsync()`:
  - [x] Проверяет, что Current != null
  - [x] Обновляет Current.version перед сохранением
  - [x] Сериализует через JsonUtility.ToJson
- [x] Console чистая

---

## Блок 3 — Биндинг в GameInstaller (15 мин)

- [x] В `GameInstaller.InstallBindings()` добавлено:
  ```csharp
  Container.Bind<SaveManager>().AsSingle();
  ```
- [x] Биндинг расположен ПОСЛЕ Yandex services (которые инжектятся в SaveManager)
- [x] Console чистая

---

## Блок 4 — Интеграция в GameBootstrap (30 мин)

- [x] В Construct добавлен параметр `SaveManager saveManager`
- [x] Параметр `ICloudSaveService` УБРАН — теперь работаем через SaveManager
- [x] В `Start()`:
  - [x] Вызывается `await _saveManager.LoadAsync()`
  - [x] Логируется состояние Current (coins, energy)
  - [x] LoadAsync вызывается ДО Fire сигнала
- [x] Console чистая, при Play нет ошибок DI

---

## Блок 5 — Smoke test (30 мин)

### Тест 1: дефолтный сейв
- [x] Удалить PlayerPrefs (Edit → Clear All PlayerPrefs)
- [x] Запустить Bootstrap
- [x] В консоли:
  ```
  [EditorCloudSave] Loaded: <empty>
  [SaveManager] No save found, creating default
  [Bootstrap] Save ready: coins=0, energy=5
  ```

### Тест 2: персистентность через Inspector
- [x] Добавить в SaveManager временный метод для теста:
  ```csharp
  [System.Diagnostics.Conditional("UNITY_EDITOR")]
  public async void TestAddCoins(int amount)
  {
      Current.coins += amount;
      await SaveAsync();
  }
  ```
  > Альтернатива: добавь `[ContextMenu("Test: +100 coins")]` на компонент GameBootstrap, который дёргает `_saveManager.Current.coins += 100; _ = _saveManager.SaveAsync();`. Через правый клик в инспекторе на компоненте можно вызвать.
- [x] Запустить, остановить, заработать через ContextMenu +100 монет
- [x] Перезапустить Bootstrap
- [x] В консоли:
  ```
  [SaveManager] Save loaded: v1, coins=100
  [Bootstrap] Save ready: coins=100, energy=5
  ```

### Тест 3: повреждённый сейв
- [x] В EditorCloudSaveService временно вернуть мусорный JSON: `return "{ broken json }";`
- [x] Запустить Bootstrap
- [x] В консоли:
  ```
  [SaveManager] Failed to parse save, creating default: ...
  [Bootstrap] Save ready: coins=0, energy=5
  ```
- [x] **Откатить изменение в EditorCloudSaveService** обратно

---

## Блок 6 — Commit (15 мин)

- [ ] `git status`
- [ ] `git add Assets/_Project/00-Code`
- [ ] `git commit -m "feat: save manager with versioning and json serialization"`
- [ ] `git push origin feature`
- [ ] Закрыть Unity

---

## Чекпоинт дня

Готово, если:
- [ ] При старте Bootstrap идёт LoadAsync, видны логи
- [ ] Тест 2 пройден: монеты сохраняются между запусками
- [ ] Тест 3 пройден: повреждённый сейв не валит игру, создаётся дефолт
- [ ] Console чистая в нормальном случае
- [ ] Коммит запушен

## Если застрял

**`JsonUtility.ToJson` возвращает "{}" вместо данных** — поля не public, либо нет `[Serializable]` на классе.

**`ZenjectException: Cannot inject SaveManager`** — забыл `Container.Bind<SaveManager>().AsSingle();` в installer.

**`NullReferenceException` при Save** — Current не инициализирован, забыл вызвать LoadAsync до SaveAsync.

**Сейв "сохраняется", но при следующем запуске = 0** — EditorCloudSaveService использует PlayerPrefs, а ты случайно сбросил их между запусками. Это норма, не баг.

**Unix time возвращает странные числа** — DateTimeOffset.UtcNow.ToUnixTimeSeconds() возвращает long. Если используешь int — будет переполнение. У нас long, должно работать.

## Не делай сегодня

- Не интегрируй Yandex SDK реально — это День 6
- Не делай UI с отображением монет — экраны на День 17
- Не пиши автосохранение по таймеру — событийное автосохранение добавим на Дне 19 (после game over)
- Не оптимизируй сериализацию — JsonUtility достаточно быстр

---

**Старт дня:** _____
**Конец дня:** _____
**Реальное время:** _____
**Что не успел:** _____

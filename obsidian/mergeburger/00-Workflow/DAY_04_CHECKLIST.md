# День 4 — SignalBus и нервная система событий

**Цель дня:** Zenject SignalBus подключён, четыре сигнала определены, DebugEventLogger подписан и логирует. В консоли видна полная цепочка: Bootstrap.Fire → Logger.OnReceived → Debug.Log.

**Время:** ~2.5 часа.

---

## Блок 1 — Сигналы (30 мин)

Создать четыре файла в `Assets/_Project/00-Code/Events/`:

- [x] `GameOverSignal.cs` (код из чата)
- [x] `BurgerCreatedSignal.cs` (код из чата)
- [x] `EnergySpentSignal.cs` (код из чата)
- [x] `LevelMilestoneSignal.cs` (код из чата)

Проверка:
- [x] Все классы в namespace `Mergeburgers.Events`
- [x] Все классы помечены `sealed`
- [x] Все поля — `{ get; }` без сеттера (immutable)
- [x] Console чистая

---

## Блок 2 — DebugEventLogger (30 мин)

- [x] Создан `Assets/_Project/00-Code/Core/DebugEventLogger.cs` (код из чата)
- [x] Класс реализует `IInitializable` и `System.IDisposable`
- [x] В `Initialize()` — четыре `_signalBus.Subscribe<T>(handler)`
- [x] В `Dispose()` — четыре `_signalBus.Unsubscribe<T>(handler)`
- [ ] Каждый handler пишет в Debug.Log с префиксом `[Signals]`

---

## Блок 3 — GameInstaller обновление (20 мин)

- [x] Открыть `Assets/_Project/00-Code/Core/GameInstaller.cs`
- [x] В `InstallBindings()` добавлено в нужном порядке:
  - [x] `SignalBusInstaller.Install(Container);` — должен быть ПЕРВЫМ
  - [x] `Container.DeclareSignal<...>();` для каждого из четырёх сигналов
  - [x] (Yandex биндинги остаются как были)
  - [x] `Container.BindInterfacesAndSelfTo<DebugEventLogger>().AsSingle().NonLazy();`
- [x] Console чистая, нет ошибок компиляции

---

## Блок 4 — GameBootstrap обновление (20 мин)

- [x] Открыть `GameBootstrap.cs`
- [x] В `Construct(...)` добавлен третий параметр `SignalBus signalBus`
- [x] Поле `_signalBus` присваивается
- [x] В конце `Start()` (перед LoadSceneAsync) добавлено:
  ```csharp
  _signalBus.Fire(new LevelMilestoneSignal(0));
  ```

---

## Блок 5 — Smoke test (20 мин)

- [x] Открыта сцена `Bootstrap`
- [x] Нажать Play
- [x] В Console видны логи в таком порядке:
  ```
  [EditorCloudSave] Loaded: <empty>
  [Bootstrap] Save loaded: <no save>
  [Bootstrap] Ad service ready: True
  [Signals] Milestone reached: #0
  ```
- [x] Грузится MainMenu, кнопка «Играть» работает
- [x] Цикл Bootstrap → MainMenu → Game → MainMenu работает без ошибок
- [x] Console пустая — без `ZenjectException` или `SignalNotDeclaredException`

---

## Блок 6 — Commit (15 мин)

- [x] `git status` — видишь изменения
- [x] `git add Assets/_Project/00-Code`
- [x] `git commit -m "feat: signal bus with four core signals and debug logger"`
- [x] `git push origin feature`
- [x] Закрыть Unity

---

## Чекпоинт дня

Готово, если:
- [x] При старте Bootstrap-сцены в консоли видна строка `[Signals] Milestone reached: #0`
- [x] DebugEventLogger создан Zenject'ом автоматически (благодаря NonLazy)
- [x] Подписки и отписки симметричны (одинаковый набор методов в Initialize / Dispose)
- [x] Коммит запушен

## Если застрял

**`ZenjectException: Signal 'LevelMilestoneSignal' has not been declared`** — забыл `Container.DeclareSignal<LevelMilestoneSignal>()` в installer.

**`SignalBus could not be resolved`** — забыл `SignalBusInstaller.Install(Container)` в начале InstallBindings.

**DebugEventLogger не создаётся** (не пишет логи) — забыл `NonLazy()` в биндинге, либо забыл `BindInterfacesAndSelfTo` (`BindInterfacesTo` без `Self` тоже сработает в нашем случае, но `BindInterfacesAndSelfTo` универсальнее).

**`Cannot inject SignalBus into GameBootstrap`** — на сцене Bootstrap нет SceneContext, либо ProjectContext не наследуется. Проверь Hierarchy.

**Сигнал летит, но handler не вызывается** — проверь, что в Initialize действительно вызывается Subscribe, и что метод-handler соответствует сигнатуре `void OnX(SignalType s)`.

## Не делай сегодня

- Не создавай новые сигналы (вроде `OnButtonClicked`) — добавим по мере появления реального gameplay
- Не превращай SignalBus в команды (например, не пиши `_signalBus.Fire(new ShowPopupSignal())` — для команд используй прямые вызовы интерфейсов)
- Не пиши SaveManager — это День 5
- Не интегрируй Yandex SDK — это День 6

---

**Старт дня:** _____
**Конец дня:** _____
**Реальное время:** _____
**Что не успел:** _____

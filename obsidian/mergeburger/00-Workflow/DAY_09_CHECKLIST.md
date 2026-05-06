# День 9 — SwipeInput через New Input System

**Цель дня:** игрок свайпает мышью или пальцем на экране → в Console появляется `[Input] Swipe Up/Down/Left/Right`. Сигнал `SwipeDetectedSignal` публикуется через SignalBus.

**Время:** ~3 часа.

---

## Блок 1 — Проверка EventSystem (15 мин)

- [x] Открой сцену Game
- [x] В Hierarchy найди объект EventSystem
- [x] В инспекторе посмотри, какой компонент Input Module стоит:
  - Если `Standalone Input Module` (legacy) → удали его, добавь `Input System UI Input Module` (через Add Component → search "input system")
  - Если `Input System UI Input Module` → ОК, переходи дальше
- [x] Если EventSystem нет вообще — создай: Hierarchy → Create → UI → Event System
- [x] Сохрани сцену
- [x] Повтори то же самое для сцены MainMenu

---

## Блок 2 — SwipeDetectedSignal (15 мин)

- [x] Создан `Assets/_Project/00-Code/Events/SwipeDetectedSignal.cs` (код из чата)
- [x] Содержит enum SwipeDirection (Up/Down/Left/Right)
- [x] Содержит sealed class SwipeDetectedSignal с полем Direction
- [x] В `GameInstaller.Signals()` добавлен `Container.DeclareSignal<SwipeDetectedSignal>();`
- [x] Console чистая

---

## Блок 3 — SwipeInput.cs (60 мин)

- [x] Создан `Assets/_Project/00-Code/Gameplay/SwipeInput.cs` (код из чата)
- [x] Использует `using UnityEngine.InputSystem;`
- [x] Метод Construct получает SignalBus
- [x] Логика в Update: tracking start → tracking end → direction calculation → fire signal

### Объект на сцене Game
- [x] В Hierarchy создай пустой GameObject `[SwipeInput]` (квадратные скобки — конвенция системных объектов)
- [x] Повесь на него компонент SwipeInput
- [x] В инспекторе SwipeInput:
  - Min Swipe Distance: 50 (тестовое значение, подкрутим если нужно)
  - Max Swipe Duration: 1.0

---

## Блок 4 — DebugSwipeListener (20 мин)

- [x] Создан `Assets/_Project/00-Code/Gameplay/DebugSwipeListener.cs` (код из чата)
- [x] Реализует IInitializable + IDisposable
- [x] Subscribe в Initialize, Unsubscribe в Dispose

### Биндинг
- [x] В `GameSceneInstaller`:
  - [x] Добавлено поле `[SerializeField] private SwipeInput _swipeInput;`
  - [x] В InstallBindings: `Container.Bind<SwipeInput>().FromInstance(_swipeInput).AsSingle();`
  - [x] В InstallBindings: `Container.BindInterfacesAndSelfTo<DebugSwipeListener>().AsSingle().NonLazy();`
- [x] На SceneContext в Hierarchy найди GameSceneInstaller → перетащи объект `[SwipeInput]` в поле Swipe Input

---

## Блок 5 — Smoke test (20 мин)

- [x] Открой сцену Bootstrap
- [x] Play
- [x] Bootstrap → MainMenu → клик «Играть» → Game сцена
- [x] В Game-сцене с зажатой ЛКМ свайпни мышью:
  - [x] Вверх → лог `[Input] Swipe Up`
  - [x] Вниз → лог `[Input] Swipe Down`
  - [x] Влево → лог `[Input] Swipe Left`
  - [x] Вправо → лог `[Input] Swipe Right`
- [x] Короткий тап (без движения) → НЕТ лога (отбрасывается как слишком короткий)
- [x] Очень медленное движение (>1 сек) → НЕТ лога (отбрасывается как drag)
- [x] Console чистая

---

## Блок 6 — Commit (15 мин)

- [x] git add Assets/_Project
- [x] git commit -m "feat: swipe input via new input system with signal bus"
- [x] git push origin feature

---

## Чекпоинт дня

Готово, если:
- [x] 4 направления свайпа определяются корректно
- [x] Тапы и долгие удержания не триггерят свайп
- [x] Console чистая
- [x] Коммит запушен

## Если застрял

**`Pointer.current` is null** — на сцене нет EventSystem, или стоит StandaloneInputModule вместо Input System UI Input Module.

**`Pointer` не найден через using** — забыл `using UnityEngine.InputSystem;`. Если IDE не предлагает — это значит New Input System пакет не установлен. Project Settings → Player → Active Input Handling должен быть `Both` или `Input System Package (New)`. Если стоит `Input Manager (Old)` — переключай, Unity потребует перезапуск.

**Свайп срабатывает на каждое движение мыши** — забыл проверку `_isTracking` или порог `_minSwipeDistance` слишком маленький. Поставь 100 для теста.

**Сигнал не доходит до DebugSwipeListener** — забыл `NonLazy()` в биндинге, либо забыл `BindInterfacesAndSelfTo` (просто `Bind<DebugSwipeListener>` не подключит интерфейс IInitializable).

**На сцене кнопка «← В меню» не работает после смены EventSystem** — Input System UI Input Module по умолчанию должен работать с UI кнопками. Если не работает — проверь, что Action References (Move/Submit/Cancel и т.д.) заполнены автоматически (Unity делает это при добавлении модуля). Если нет — нажми кнопку справа от поля → Default.

## Не делай сегодня

- Не реализуй движение плиток в направлении свайпа — это День 10
- Не делай визуальные эффекты свайпа (trail, частицы) — на потом
- Не пиши собственный InputAction asset — для одного свайпа overkill
- Не пытайся обработать multi-touch (свайп двумя пальцами и т.п.) — у нас одиночный свайп

---

**Старт дня:** _____
**Конец дня:** _____
**Реальное время:** _____

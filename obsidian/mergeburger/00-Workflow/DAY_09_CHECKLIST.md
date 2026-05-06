# День 9 — SwipeInput через New Input System

**Цель дня:** игрок свайпает мышью или пальцем на экране → в Console появляется `[Input] Swipe Up/Down/Left/Right`. Сигнал `SwipeDetectedSignal` публикуется через SignalBus.

**Время:** ~3 часа.

---

## Блок 1 — Проверка EventSystem (15 мин)

- [ ] Открой сцену Game
- [ ] В Hierarchy найди объект EventSystem
- [ ] В инспекторе посмотри, какой компонент Input Module стоит:
  - Если `Standalone Input Module` (legacy) → удали его, добавь `Input System UI Input Module` (через Add Component → search "input system")
  - Если `Input System UI Input Module` → ОК, переходи дальше
- [ ] Если EventSystem нет вообще — создай: Hierarchy → Create → UI → Event System
- [ ] Сохрани сцену
- [ ] Повтори то же самое для сцены MainMenu

---

## Блок 2 — SwipeDetectedSignal (15 мин)

- [ ] Создан `Assets/_Project/00-Code/Events/SwipeDetectedSignal.cs` (код из чата)
- [ ] Содержит enum SwipeDirection (Up/Down/Left/Right)
- [ ] Содержит sealed class SwipeDetectedSignal с полем Direction
- [ ] В `GameInstaller.Signals()` добавлен `Container.DeclareSignal<SwipeDetectedSignal>();`
- [ ] Console чистая

---

## Блок 3 — SwipeInput.cs (60 мин)

- [ ] Создан `Assets/_Project/00-Code/Gameplay/SwipeInput.cs` (код из чата)
- [ ] Использует `using UnityEngine.InputSystem;`
- [ ] Метод Construct получает SignalBus
- [ ] Логика в Update: tracking start → tracking end → direction calculation → fire signal

### Объект на сцене Game
- [ ] В Hierarchy создай пустой GameObject `[SwipeInput]` (квадратные скобки — конвенция системных объектов)
- [ ] Повесь на него компонент SwipeInput
- [ ] В инспекторе SwipeInput:
  - Min Swipe Distance: 50 (тестовое значение, подкрутим если нужно)
  - Max Swipe Duration: 1.0

---

## Блок 4 — DebugSwipeListener (20 мин)

- [ ] Создан `Assets/_Project/00-Code/Gameplay/DebugSwipeListener.cs` (код из чата)
- [ ] Реализует IInitializable + IDisposable
- [ ] Subscribe в Initialize, Unsubscribe в Dispose

### Биндинг
- [ ] В `GameSceneInstaller`:
  - [ ] Добавлено поле `[SerializeField] private SwipeInput _swipeInput;`
  - [ ] В InstallBindings: `Container.Bind<SwipeInput>().FromInstance(_swipeInput).AsSingle();`
  - [ ] В InstallBindings: `Container.BindInterfacesAndSelfTo<DebugSwipeListener>().AsSingle().NonLazy();`
- [ ] На SceneContext в Hierarchy найди GameSceneInstaller → перетащи объект `[SwipeInput]` в поле Swipe Input

---

## Блок 5 — Smoke test (20 мин)

- [ ] Открой сцену Bootstrap
- [ ] Play
- [ ] Bootstrap → MainMenu → клик «Играть» → Game сцена
- [ ] В Game-сцене с зажатой ЛКМ свайпни мышью:
  - [ ] Вверх → лог `[Input] Swipe Up`
  - [ ] Вниз → лог `[Input] Swipe Down`
  - [ ] Влево → лог `[Input] Swipe Left`
  - [ ] Вправо → лог `[Input] Swipe Right`
- [ ] Короткий тап (без движения) → НЕТ лога (отбрасывается как слишком короткий)
- [ ] Очень медленное движение (>1 сек) → НЕТ лога (отбрасывается как drag)
- [ ] Console чистая

---

## Блок 6 — Commit (15 мин)

- [ ] git add Assets/_Project
- [ ] git commit -m "feat: swipe input via new input system with signal bus"
- [ ] git push origin feature

---

## Чекпоинт дня

Готово, если:
- [ ] 4 направления свайпа определяются корректно
- [ ] Тапы и долгие удержания не триггерят свайп
- [ ] Console чистая
- [ ] Коммит запушен

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

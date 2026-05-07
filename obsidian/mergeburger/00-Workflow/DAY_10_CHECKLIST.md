# День 10 — Базовая 2048-механика движения

**Цель дня:** свайп → плитки двигаются в направлении свайпа, упираясь в стену; одинаковые соседи схлопываются в следующий ингредиент по цепочке (Bun+Bun → Patty, и т.д.). Без анимаций, без рецептов с разными ингредиентами.

**Время:** ~3 часа.

---

## Блок 1 — MergeOperation (20 мин)

- [x] Создан `Assets/_Project/00-Code/Gameplay/MergeOperation.cs` (код из чата)
- [x] enum MergeOperationType (Move, Merge)
- [x] readonly struct MergeOperation с полями From, To, ResultType
- [x] Console чистая

---

## Блок 2 — EvolutionChain (40 мин)

- [x] Создан `Assets/_Project/00-Code/Data/EvolutionChain.cs` (код из чата)
- [x] CreateAssetMenu атрибут добавлен
- [x] Метод Evolve возвращает следующий тип или None
- [x] Console чистая

### Создание ассета
- [x] `Assets/_Project/01-Data/Config/` → правый клик → Create → Mergeburgers → Evolution Chain
- [x] Назови `EvolutionChain`
- [x] В инспекторе проверь, что список `_chain` имеет 6 элементов: Bun, Patty, Cheese, Lettuce, Tomato, Sauce. Если пуст — заполни вручную.

---

## Блок 3 — MergeResolver (60 мин)

- [ ] Создан `Assets/_Project/00-Code/Gameplay/MergeResolver.cs` (код из чата)
- [ ] **Конструктор помечен `[Inject]`** ← обязательно (урок Дня 9)
- [ ] Класс sealed
- [ ] Public метод Resolve возвращает ResolveResult с NewState, Operations, AnyChange
- [ ] Приватный ProcessLine обрабатывает одну строку или столбец
- [ ] Console чистая

---

## Блок 4 — Board.HandleSwipe (40 мин)

- [ ] Заменить содержимое `Board.cs` на новую версию (код из чата)
- [ ] Добавлено поле `IngredientType[,] _state` параллельно с `Tile[,] _grid`
- [ ] В Construct инжектится `MergeResolver`
- [ ] В SpawnGrid сохраняется тип каждой плитки в `_state`
- [ ] Метод HandleSwipe вызывает MergeResolver.Resolve, обновляет состояние, перерисовывает
- [ ] Метод RedrawGrid обновляет UI на основе нового _state
- [ ] Console чистая

---

## Блок 5 — BoardController (20 мин)

- [ ] Создан `Assets/_Project/00-Code/Gameplay/BoardController.cs` (код из чата)
- [ ] Конструктор помечен `[Inject]`
- [ ] Реализует IInitializable + IDisposable
- [ ] В Initialize подписывается на SwipeDetectedSignal
- [ ] В Dispose отписывается

### Удаление DebugSwipeListener
- [ ] Удалён файл `DebugSwipeListener.cs`
- [ ] Из GameSceneInstaller убран биндинг `BindInterfacesAndSelfTo<DebugSwipeListener>()`

### Биндинги в GameSceneInstaller
- [ ] Добавлено поле `[SerializeField] private EvolutionChain _evolutionChain;`
- [ ] Добавлен биндинг `Container.Bind<EvolutionChain>().FromInstance(_evolutionChain).AsSingle();`
- [ ] Добавлен биндинг `Container.Bind<MergeResolver>().AsSingle();`
- [ ] Добавлен биндинг `Container.BindInterfacesAndSelfTo<BoardController>().AsSingle().NonLazy();`
- [ ] В Inspector на SceneContext в поле GameSceneInstaller перетащи EvolutionChain ассет

---

## Блок 6 — Smoke test (20 мин)

- [ ] Открой Bootstrap
- [ ] Play
- [ ] Bootstrap → MainMenu → клик «Играть» → Game
- [ ] Свайп вправо: все плитки сдвигаются вправо, упираясь в правую стену; одинаковые соседи (например 2 буквы B стоявшие рядом) схлопываются в P (Patty, следующий по цепочке). Слева остаются пустые клетки (тёмные)
- [ ] Свайп влево: то же самое, но к левой стенке
- [ ] Свайп вверх: к верхней стенке
- [ ] Свайп вниз: к нижней стенке
- [ ] Если на доске **нет** одинаковых пар и плитки уже у стены — свайп ничего не меняет, в Console `[Board] Swipe X — no change`
- [ ] При движении: `[Board] Swipe X — N ops`
- [ ] Console чистая

### Если повезёт собрать цепочку
- [ ] B+B = P, потом P+P = C, и т.д. до Sauce. Sauce+Sauce ничего не делает (max в цепочке)

### Особенный случай: тройной merge
- [ ] B B B в ряд → свайп → P B (две слились, третья отдельно)
- [ ] B B B B в ряд → свайп → P P (две пары)

---

## Блок 7 — Commit (15 мин)

- [ ] git add Assets/_Project
- [ ] git commit -m "feat: basic 2048 movement and evolution merge"
- [ ] git push origin feature

---

## Чекпоинт дня

Готово, если:
- [ ] Все 4 направления свайпа корректно двигают плитки
- [ ] Одинаковые пары схлопываются в следующий по цепочке
- [ ] Тройные/четверные группы обрабатываются корректно
- [ ] Console чистая
- [ ] Коммит запушен

## Если застрял

**Плитки не двигаются** — BoardController не подписан на сигнал. Проверь NonLazy в биндинге, проверь [Inject] на конструкторе.

**Плитки исчезают, но новых нет** — RedrawGrid не вызывается, или _state не обновляется. Поставь Debug.Log в HandleSwipe, посмотри что result.AnyChange = true.

**Странное движение в одном направлении (например, право работает, лево нет)** — баг в ProcessLine, скорее всего в условии `toEnd` или `Reverse()`. Покажешь код, разберём.

**3+ одинаковых в ряд схлопываются неправильно** — алгоритм 2048 при свайпе вправо по B B B → должен дать (None, B, P) или (None, P, B) в зависимости от того, с какого конца начинаешь. По стандарту 2048 — с конца движения, то есть с правой стороны. У меня в коде `if (toEnd) line.Reverse()` — это даёт правильный порядок.

**NullReferenceException где-то в Resolver** — _evolutionChain == null. Проверь [Inject] на конструкторе и биндинг EvolutionChain в installer.

## Не делай сегодня

- Не делай анимации DOTween — это День 14
- Не делай спавн новой плитки после хода — это День 11
- Не делай рецепты с разными ингредиентами (булка+котлета+булка=гамбургер) — это День 12
- Не пиши юнит-тесты на MergeResolver — на потом, если найдём время
- Не оптимизируй алгоритм — он O(n²), для 5×5 = 25 операций, мгновенно

---

**Старт дня:** _____
**Конец дня:** _____
**Реальное время:** _____

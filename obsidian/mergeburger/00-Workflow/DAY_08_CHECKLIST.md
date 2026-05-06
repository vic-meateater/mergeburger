# День 8 — Доска и плитки

**Цель дня:** На сцене Game отображается грид 5×5 разноцветных плиток-плейсхолдеров с буквами. Никакой логики, только визуал.

**Время:** ~3 часа.

---

## Блок 1 — Типы ингредиентов и данные (30 мин)

- [x] Создан `Assets/_Project/00-Code/Gameplay/IngredientType.cs` (enum, код из чата)
- [x] Создан `Assets/_Project/00-Code/Data/IngredientData.cs` (ScriptableObject, код из чата)
- [x] Создан `Assets/_Project/00-Code/Data/IngredientDatabase.cs` (ScriptableObject, код из чата)
- [x] Console чистая

### Создание ассетов
- [ ] В Project: `Assets/_Project/01-Data/Ingredients/` → правый клик → Create → Mergeburgers → Ingredient. Сделай 6 штук:
  - [x] `Ingredient_Bun` — Type: Bun, Color: золотисто-жёлтый (`#F4C430`), Letter: `B`
  - [x] `Ingredient_Patty` — Type: Patty, Color: тёмно-коричневый (`#6B3410`), Letter: `P`
  - [x] `Ingredient_Cheese` — Type: Cheese, Color: ярко-жёлтый (`#FFE135`), Letter: `C`
  - [x] `Ingredient_Lettuce` — Type: Lettuce, Color: салатовый (`#7FBF3F`), Letter: `L`
  - [x] `Ingredient_Tomato` — Type: Tomato, Color: красный (`#E63946`), Letter: `T`
  - [x] `Ingredient_Sauce` — Type: Sauce, Color: оранжевый (`#FF9F1C`), Letter: `S`
- [x] В Project: `Assets/_Project/01-Data/` → правый клик → Create → Mergeburgers → Ingredient Database. Назови `IngredientDatabase`.
- [x] Открой `IngredientDatabase`. В поле Ingredients перетащи все 6 созданных IngredientData ассетов.

---

## Блок 2 — Tile префаб (40 мин)

- [x] Создан `Assets/_Project/00-Code/Gameplay/Tile.cs` (код из чата)

### Префаб
- [x] В Hierarchy: Create → UI → Image. Назови `Tile`.
- [x] На объекте Tile:
  - Width: 160, Height: 160
  - Source Image: пусто (по умолчанию белый квадрат — нам подходит)
  - Color: серый (видно в редакторе)
- [x] Дочерний объект: правый клик на Tile → UI → Text - TextMeshPro. Назови `Letter`.
  - Anchor preset: stretch + fill (Alt+Shift+стрелочки)
  - Left/Top/Right/Bottom: 0
  - Font Size: 60
  - Alignment: Center + Middle
  - Color: чёрный
  - Text: `?` (плейсхолдер, перезапишется в коде)
- [x] На корневом Tile повесь компонент `Tile.cs`
- [x] В инспекторе Tile.cs:
  - Background ← перетащи сам Image-компонент Tile
  - Letter ← перетащи TextMeshProUGUI компонент дочернего объекта
- [x] Перетащи Tile из Hierarchy в `Assets/_Project/04-Prefabs/`. Появится синяя иконка префаба
- [x] Удали Tile из сцены (он остался префабом в папке)

---

## Блок 3 — Board (40 мин)

- [ ] Создан `Assets/_Project/00-Code/Gameplay/Board.cs` (код из чата)

### Объекты на сцене Game
- [x] Открой сцену `Game`
- [x] В существующем Canvas создай дочерний пустой объект `BoardRoot` (Create Empty Child от Canvas)
  - Anchor preset: middle-center
  - Width: 900, Height: 900
  - Position: 0, 0, 0
- [x] Внутри BoardRoot создай дочерний пустой `BoardContainer` (RectTransform):
  - Anchor preset: middle-center
  - Width: 900, Height: 900
  - Position: 0, 0, 0
- [x] На BoardRoot повесь компонент Board.cs
- [x] В инспекторе Board:
  - Container ← перетащи BoardContainer
  - Tile Prefab ← перетащи префаб Tile из 04-Prefabs
  - Width: 5
  - Height: 5
  - Tile Size: 160
  - Spacing: 8

---

## Блок 4 — GameSceneInstaller (20 мин)

- [x] Создан `Assets/_Project/00-Code/Core/GameSceneInstaller.cs` (код из чата)
- [x] На сцене Game в Hierarchy найди объект SceneContext
- [x] На SceneContext добавь компонент GameSceneInstaller через Add Component
- [x] В SceneContext в поле Mono Installers: размер списка → 1, элемент → перетащи компонент GameSceneInstaller (тот же объект)
- [x] В GameSceneInstaller инспектор:
  - Board ← перетащи BoardRoot объект (с компонентом Board)
  - Ingredient Database ← перетащи IngredientDatabase ассет из 01-Data

---

## Блок 5 — Smoke test (20 мин)

- [x] Открой сцену Bootstrap
- [x] Play
- [x] Проходишь Bootstrap → MainMenu → нажимаешь «Играть»
- [x] На сцене Game видишь грид 5×5 разноцветных квадратов с буквами B/P/C/L/T/S в случайном порядке
- [x] В Console: `[Board] Spawned 5x5 grid`
- [x] Кнопка «← В меню» возвращает в MainMenu
- [x] Console чистая

---

## Блок 6 — Commit (15 мин)

- [x] git add Assets/_Project
- [x] git commit -m "feat: board grid 5x5 with placeholder ingredient tiles"
- [x] git push origin feature

---

## Чекпоинт дня

Готово, если:
- [x] Видишь грид 5×5 на Game сцене
- [x] Каждый запуск — разные ингредиенты в случайных местах
- [x] Console чистая, нет ошибок DI
- [x] Коммит запушен

## Если застрял

**Плитки накладываются друг на друга** — _container не назначен в Board. Проверь, что в Board.Container стоит BoardContainer.

**Плитки за пределами экрана** — Canvas Scaler не настроен, Reference Resolution не 1080×1920, или BoardRoot anchor не middle-center.

**`NullReferenceException` в Board.SpawnGrid** — IngredientDatabase не привязан в GameSceneInstaller, либо в самом Database список Ingredients пуст.

**Tile.cs не видит компонентов** — Background и Letter не перетянуты в инспекторе на префабе.

**ZenjectException Cannot inject Board** — забыл повесить GameSceneInstaller на SceneContext, или забыл добавить его в Mono Installers список SceneContext.

## Не делай сегодня

- Не реализуй свайп — это День 9
- Не реализуй merge — это День 10-13
- Не делай красивый арт — плейсхолдеры с буквами достаточно
- Не реализуй спавн новой плитки после хода — некуда спавнить, ходов нет

---

**Старт дня:** _____
**Конец дня:** _____
**Реальное время:** _____

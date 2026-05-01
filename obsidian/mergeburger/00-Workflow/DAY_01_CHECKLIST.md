# День 1 — Подготовка проекта

**Цель дня:** Unity-проект открывается без ошибок, структура папок создана, базовые пакеты импортированы, git инициализирован, первый коммит сделан.

**Время:** ~3 часа. Если идёт быстрее — отлично, ранний финиш.
**Если идёт медленнее** — не паникуй, перенеси Блок 6 на завтра. День 1 единственный, где это допустимо.

---

## Блок 1 — Unity Hub и создание проекта (30 мин)

- [x] Открыт Unity Hub
- [x] Установлена версия Unity 2022.3.x LTS (последняя стабильная минорная)
  - Modules при установке: WebGL Build Support, Documentation
- [x] Создан проект через Unity Hub:
  - Template: **2D (URP)** ← важно, не 3D
  - Project name: `BurgerMaster`
  - Path: НЕ внутри OneDrive / Yandex.Disk / Dropbox
- [ ] Проект открылся, нет ошибок в Console

---

## Блок 2 — Project Settings (20 мин)

Open `Edit → Project Settings`.

- [x] Player → Resolution and Presentation:
  - Default Canvas Width: `1080`
  - Default Canvas Height: `1920`
  - Run In Background: `false` (WebGL не любит фоновое обновление)
- [x] Player → Other Settings:
  - Color Space: `Linear`
  - Auto Graphics API: оставляем true
- [x] Quality:
  - Удалить все профили кроме одного
  - Переименовать оставшийся в `WebGL_Mid`
  - VSync Count: `Don't Sync`
  - Anti Aliasing: `Disabled` (WebGL — экономим)
- [x] Graphics:
  - Scriptable Render Pipeline Settings: убедиться, что назначен URP Asset с 2D Renderer
  - Если стоит 3D — создать новый: `Right click in Project → Create → Rendering → URP Asset (with 2D Renderer)` и назначить
- [x] Time → Fixed Timestep: `0.02` (50 FPS физики, для casual хватит)

---

## Блок 3 — Структура папок (30 мин)

Создать вручную через `Right click → Create → Folder` в Project window:

- [ ] `Assets/_Project/` (главная папка с подчёркиванием — будет первой в списке)
- [ ] `Assets/_Project/Scripts/`
- [ ] `Assets/_Project/Scripts/Core/`
- [ ] `Assets/_Project/Scripts/Yandex/`
- [ ] `Assets/_Project/Scripts/Yandex/Editor/`
- [ ] `Assets/_Project/Scripts/Gameplay/`
- [ ] `Assets/_Project/Scripts/Meta/`
- [ ] `Assets/_Project/Scripts/UI/`
- [ ] `Assets/_Project/Scripts/Data/`
- [ ] `Assets/_Project/Scripts/Events/`
- [ ] `Assets/_Project/Data/`
- [ ] `Assets/_Project/Data/Recipes/`
- [ ] `Assets/_Project/Data/Ingredients/`
- [ ] `Assets/_Project/Data/Upgrades/`
- [ ] `Assets/_Project/Data/Config/`
- [ ] `Assets/_Project/Art/`
- [ ] `Assets/_Project/Art/Sprites/`
- [ ] `Assets/_Project/Art/UI/`
- [ ] `Assets/_Project/Art/Atlases/`
- [ ] `Assets/_Project/Audio/`
- [ ] `Assets/_Project/Prefabs/`
- [ ] `Assets/_Project/Scenes/`
- [ ] Создать тестовую сцену `Assets/_Project/Scenes/_Scratch.unity` — для проб, удалим в конце недели

---

## Блок 4 — Импорт пакетов (40 мин)

### DOTween Free
- [x] Open `Window → Asset Store → search "DOTween HOTween v2"` или Package Manager → My Assets
- [x] Import all
- [x] Open `Tools → Demigiant → DOTween Utility Panel`
- [x] Click `Setup DOTween...` → `Apply`
- [x] Без этого шага DOTween работает в обрезанном виде — НЕ ЗАБУДЬ

### Extenject (актуальный форк Zenject)
- [x] Asset Store → search `Extenject Dependency Injection IOC` (by Mathijs Bernson) → Import
  - Альтернативно через GitHub: https://github.com/Mathijs-Bakker/Extenject (PackageManager → Add from Git URL)
- [x] При импорте — снять галочки с `OptionalExtras/SampleGame1`, `SampleGame2` (нам не нужны примеры в проекте)
- [x] После импорта проверить: `GameObject → Zenject` — пункты меню есть

### Yandex SDK plugin (max-games)
- [x] Скачать с https://max-games.ru/plugin-yg/ (последняя версия)
- [x] Импортировать .unitypackage в проект
- [x] Не трогать его настройки сегодня — займёмся на Дне 6

### Проверка консоли
- [x] Console очищена, нет ошибок (warnings допустимы)
- [x] Если есть `compilation errors` — пиши в чат, разбираем

---

## Блок 5 — Git (30 мин)

В терминале (или через Git GUI клиента — твой выбор):

- [x] `git init` в корне проекта `BurgerMaster/`
- [x] Создать `.gitignore` в корне со стандартным Unity-шаблоном:
  - Скачать с https://github.com/github/gitignore/blob/main/Unity.gitignore
  - Положить в корень как `.gitignore`
- [x] `git add .gitignore`
- [x] `git add .`
- [x] `git commit -m "chore: project scaffold"`
- [x] **Опционально, но настоятельно:** создать приватный репозиторий на GitHub `burger-master`, добавить remote, push:
  - `git remote add origin <url>`
  - `git branch -M main`
  - `git push -u origin main`

Проверка `.gitignore`:
- [x] `Library/`, `Temp/`, `Logs/`, `obj/`, `Build/`, `Builds/`, `.vs/`, `.idea/` — игнорятся
- [x] `Assets/` и `ProjectSettings/` — закоммитчены

---

## Блок 6 — Артефакты дня (30 мин)

- [x] Файл `backlog.md` положен в корень проекта (не в `Assets/`!)
- [x] Файл `DAY_01_CHECKLIST.md` положен в корень
- [x] Все галочки в этом чеклисте проставлены по факту
- [x] Если что-то не сделано — записать в `backlog.md` в секцию "Технический долг"
- [x] Коммит: `git add backlog.md DAY_01_CHECKLIST.md && git commit -m "docs: day 1 artifacts"`
- [x] Push (если есть remote)
- [x] Закрыть Unity

---

## Чекпоинт дня

Готово, если:
- [x] Unity открывается без ошибок
- [x] Структура `_Project` создана полностью
- [x] DOTween, Extenject, max-games плагин импортированы
- [x] Git инициализирован, есть как минимум 2 коммита
- [x] `backlog.md` существует и пуст (готов к заполнению)

## Если застрял

Что бы ни случилось — НЕ начинай вечером пилить логику игры. Это нарушение плана.
- Ошибки компиляции после импорта пакетов → пиши в чат, я разбираю
- Конфликт версий пакетов → пиши, разбираем
- Не получается с DOTween Setup → пропусти, на Дне 14 перенастроим
- Не получается с Git → разбираемся завтра, не блокирует

## Не делай сегодня

- Не пиши скрипты — даже "просто чтобы попробовать"
- Не настраивай Animator — у нас не будет персонажа
- Не открывай туториалы по 2048 — алгоритм мы разберём вместе на Дне 10
- Не показывай идею знакомым — рано

---

**Старт дня:** _(время начала)_
**Конец дня:** _(время окончания)_
**Реальное время:** _(сколько по факту)_
**Что не успел:** _(переносим на завтра, если есть)_

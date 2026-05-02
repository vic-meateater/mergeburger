# День 2 — Переходы между сценами

**Цель дня:** клик «Играть» → загрузка Game; клик «Назад» → возврат в MainMenu. Bootstrap автоматически грузит MainMenu. Никакой игровой логики, только smoke test инфраструктуры.

**Время:** ~2.5 часа.

---

## Блок 1 — Build Settings (15 мин)

- [x] `File → Build Settings` открыто
- [x] В Scenes In Build добавлены и упорядочены:
  - [x] `0` — `Assets/_Project/05-Scenes/Bootstrap.unity`
  - [x] `1` — `Assets/_Project/05-Scenes/MainMenu.unity`
  - [x] `2` — `Assets/_Project/05-Scenes/Game.unity`
- [x] `_Scratch` НЕ добавлен
- [x] Platform: WebGL не переключаем сегодня (оставляем PC для скорости итераций; WebGL переключим на Дне 7)

---

## Блок 2 — GameBootstrap на Bootstrap-сцене (30 мин)

- [x] Открыта сцена `Bootstrap`
- [x] Сцена пустая (только Main Camera от Unity)
- [x] Создан GameObject `[Bootstrap]` (квадратные скобки в имени — это конвенция для системных объектов, чтоб бросались в глаза в Hierarchy)
- [x] Создан скрипт `Assets/_Project/00-Code/Core/GameBootstrap.cs` (код из чата)
- [x] Скрипт повешен на `[Bootstrap]`
- [x] В инспекторе у `[Bootstrap]` поле `Next Scene` = `MainMenu`
- [x] Сцена сохранена

---

## Блок 3 — MainMenu UI и контроллер (40 мин)

- [x] Открыта сцена `MainMenu`
- [x] Создан Canvas:
  - Render Mode: `Screen Space - Overlay`
  - Pixel Perfect: `false`
- [x] На Canvas → компонент Canvas Scaler:
  - UI Scale Mode: `Scale With Screen Size`
  - Reference Resolution: `1080 × 1920`
  - Screen Match Mode: `Match Width Or Height`
  - Match: `0.5`
- [x] EventSystem создан (Unity спросит автоматически)
- [x] На Canvas создана UI Button (UI → Button - TextMeshPro):
  - Text: `Играть`
  - Якоря: центр (Anchor Presets → middle-center)
  - Размер: ~400×200
- [x] Создан скрипт `Assets/_Project/00-Code/UI/MainMenuController.cs` (код из чата)
- [x] На Canvas создан дочерний пустой GameObject `[MainMenu]`
- [x] На `[MainMenu]` повешен `MainMenuController`
- [x] В инспекторе `[MainMenu]` поле `Play Button` ← перетащил кнопку
- [x] Сцена сохранена

---

## Блок 4 — Game UI и контроллер (30 мин)

- [x] Открыта сцена `Game`
- [x] Создан Canvas с теми же настройками что в MainMenu (Reference Resolution 1080×1920, Match 0.5)
- [x] EventSystem создан
- [x] На Canvas создана UI Button:
  - Text: `← В меню`
  - Якоря: top-left
  - Position: ~(50, -50) от верхнего левого угла
  - Размер: ~250×100
- [x] Создан скрипт `Assets/_Project/00-Code/UI/GameSceneController.cs` (код из чата)
- [x] На Canvas создан дочерний пустой GameObject `[GameScene]`
- [x] На `[GameScene]` повешен `GameSceneController`
- [x] В инспекторе поле `Back Button` ← перетащил кнопку
- [x] Сцена сохранена

---

## Блок 5 — Smoke test (20 мин)

- [x] Открыта сцена `Bootstrap` (важно: запускать всегда отсюда!)
- [x] Нажать Play
- [x] Тест-кейсы:
  - [x] Bootstrap → автоматически грузится MainMenu (через ~0.1 сек)
  - [x] В MainMenu видна кнопка «Играть» по центру
  - [x] Клик «Играть» → грузится Game
  - [x] В Game видна кнопка «← В меню» в верхнем левом углу
  - [x] Клик «← В меню» → грузится MainMenu
  - [x] Цикл MainMenu ↔ Game работает 3+ раза подряд без ошибок
- [x] Console чистая: нет красных ошибок (warnings допустимы, разберёмся позже)

---

## Блок 6 — Commit (15 мин)

- [x] `git status` — видишь изменения
- [x] `git add Assets/_Project/00-Code Assets/_Project/05-Scenes ProjectSettings/EditorBuildSettings.asset`
- [x] `git commit -m "feat: scene routing skeleton (bootstrap → menu → game)"`
- [x] `git push origin feature`
- [x] Закрыть Unity

---

## Чекпоинт дня

Готово, если:
- [x] Цикл переходов Bootstrap → MainMenu ↔ Game работает без сбоев
- [x] Console пустая
- [x] Коммит запушен в `feature` ветку
- [x] Нет соблазна "ну давай ещё немного покодить"

## Если застрял

- Кнопка не реагирует на клик → проверь, есть ли EventSystem в сцене (UI Button требует EventSystem; обычно Unity создаёт автоматически)
- LoadSceneAsync падает с ошибкой → сцена не добавлена в Build Settings
- В инспекторе скрипта поле кнопки = None → перетащи кнопку из Hierarchy в это поле
- TMP_Essentials попросит импорт → жми Import (это шрифты для TextMeshPro)

## Не делай сегодня

- Не настраивай Zenject, не пиши installers — это День 3
- Не добавляй сейв-логику — это День 5
- Не пытайся сделать "красивые кнопки" с эффектами — арт на Дне 22
- Не создавай дополнительные сцены вроде Loading Screen — добавим, если понадобится

---

**Старт дня:** _____
**Конец дня:** _____
**Реальное время:** 2 часа
**Что не успел:** _____

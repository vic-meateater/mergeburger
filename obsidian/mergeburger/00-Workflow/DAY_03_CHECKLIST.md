# День 3 — Zenject DI и Editor-фейки сервисов

**Цель дня:** Zenject подключён, ProjectContext и SceneContext настроены, четыре интерфейса сервисов определены, Editor-фейки реализованы. В Bootstrap через `[Inject]` получаешь `IAdService` и `ICloudSaveService`, в консоли видишь логи имитированных вызовов.

**Время:** ~3 часа.

---

## Блок 0 — Хвосты Дня 1 (5 мин)

Если не дочистил Quality Settings вчера:

- [x] Project Settings → Quality
- [x] Realtime Reflection Probes: false
- [x] Realtime GI CPU Usage: Low
- [x] Particle Raycast Budget: 64
- [x] LOD Bias: 1
- [x] Skin Weights: 2 Bones
- [x] (если есть несколько профилей) удалить все, оставить один, переименовать в `WebGL_Mid`

---

## Блок 1 — ProjectContext (30 мин)

- [x] В Unity меню: `Edit → Project Settings → Zenject` (или `GameObject → Zenject → Project Context` — зависит от версии Extenject)
- [x] Создан prefab `Assets/Resources/ProjectContext.prefab` (Extenject ставит автоматически)
- [x] Открыт ProjectContext.prefab в инспекторе
- [x] Проверить: на нём есть компонент `ProjectContext` (от Zenject)

> **Важно:** ProjectContext.prefab должен лежать ИМЕННО в `Assets/Resources/`, не в `_Project/`. Это требование Extenject. Не переноси его, иначе DI не загрузится.

---

## Блок 2 — Интерфейсы сервисов (40 мин)

Создать четыре файла в `Assets/_Project/00-Code/Yandex/`:

- [x] `IAdService.cs` (код из чата)
- [x] `ICloudSaveService.cs` (код из чата)
- [x] `ILeaderboardService.cs` (код из чата)
- [x] `IIAPService.cs` (код из чата)

Проверка:
- [x] Console чистая, нет ошибок компиляции
- [x] Все четыре интерфейса в namespace `BurgerMaster.Yandex`

---

## Блок 3 — Editor-фейки (50 мин)

Создать четыре файла в `Assets/_Project/00-Code/Yandex/Editor/`:

- [x] `EditorAdService.cs` (код из чата)
- [x] `EditorCloudSaveService.cs` (код из чата)
- [x] `EditorLeaderboardService.cs` (код из чата)
- [x] `EditorIAPService.cs` (код из чата)

Проверка:
- [x] Все классы обёрнуты в `#if UNITY_EDITOR ... #endif`
- [x] Console чистая
- [x] (опционально) переключение Build Target на WebGL временно: `File → Build Settings → WebGL → Switch Platform`. После переключения проверить, что Editor-классы не компилируются (Console чистая, имплементаций нет — это норма, временно. Потом верни обратно на Standalone, чтобы быстрее итерировать.)

---

## Блок 4 — GameInstaller (30 мин)

- [x] Создан `Assets/_Project/00-Code/Core/GameInstaller.cs` (код из чата)
- [x] Открыт ProjectContext.prefab в инспекторе
- [x] У ProjectContext в поле `Installers` нажать `+`, перетащить `GameInstaller.cs` в слот ИЛИ:
  - повесить компонент `GameInstaller` на сам ProjectContext через Add Component
  - в поле `Mono Installers` ProjectContext'а перетащить ссылку на этот компонент

> **Если GameInstaller не виден в списке компонентов** при Add Component — проверь, что класс наследуется от `MonoInstaller`, а не от `Installer` (есть две разные базы в Zenject). У нас `MonoInstaller` — он вешается на GameObject.

---

## Блок 5 — SceneContext на Bootstrap-сцене (20 мин)

- [x] Открыта сцена `Bootstrap`
- [x] В Hierarchy: правый клик → `Zenject → Scene Context` (или `GameObject → Zenject → Scene Context`)
- [x] В сцене появился объект `SceneContext`
- [x] Сохранить сцену
- [x] Повторить для сцен `MainMenu` и `Game` (без SceneContext инжект в скрипты этих сцен не сработает)

---

## Блок 6 — Обновление GameBootstrap (15 мин)

- [x] Открыть `GameBootstrap.cs`
- [x] Заменить содержимое на новую версию из чата (с `[Inject]` и async Start)
- [x] Сохранить, дождаться компиляции
- [x] Проверить: на объекте `[Bootstrap]` в сцене всё ещё назначен скрипт, нет красных ошибок в инспекторе

---

## Блок 7 — Smoke test DI (15 мин)

- [x] Открыть сцену `Bootstrap`
- [x] Нажать Play
- [x] В Console должны появиться логи в таком порядке:
  ```
  [Bootstrap] Initializing...
  [EditorCloudSave] Loaded: <empty>
  [Bootstrap] Save loaded: <no save>
  [Bootstrap] Ad service ready: True
  ```
- [x] Через ~0.6 сек грузится MainMenu
- [x] Кнопка «Играть» по-прежнему работает (DI не сломал старый функционал)
- [x] Кнопка «← В меню» в Game по-прежнему работает

---

## Блок 8 — Commit (10 мин)

- [x] `git status`
- [x] `git add Assets/_Project Assets/Resources`
- [x] `git commit -m "feat: zenject DI scaffold with editor service stubs"`
- [x] `git push origin feature`
- [x] Закрыть Unity

---

## Чекпоинт дня

Готово, если:
- [x] При старте сцены Bootstrap в Console видны 4 строки логов из EditorAdService/EditorCloudSaveService
- [x] Цикл переходов сцен работает как раньше
- [x] Console пустая (нет ошибок DI: `ZenjectException`, `Unable to resolve...`)
- [x] Коммит запушен

## Если застрял

**`ZenjectException: Unable to resolve type 'IAdService'`** — installer не подключён к ProjectContext, либо ProjectContext.prefab лежит не в `Assets/Resources/`.

**`[Inject]` не срабатывает** (поля null) — на сцене нет SceneContext, либо SceneContext без наследования от ProjectContext (по умолчанию должно работать автоматически).

**Класс GameInstaller не виден в Add Component** — наследуется от `Installer` вместо `MonoInstaller`. Должен быть `MonoInstaller`.

**В консоли `Method not found` при вызове async** — забыл `using System.Threading.Tasks;`. Проверь импорты.

**Хочется добавить пятый сервис** — нет. День 3 — это четыре сервиса. Пятый запиши в backlog, добавим если понадобится.

## Не делай сегодня

- Не пиши реальные WebGL-имплементации (это День 6, после изучения max-games плагина)
- Не делай SignalBus / EventBus (это День 4)
- Не пиши SaveManager класс (это День 5)
- Не пытайся сделать prefab для UI кнопок Reused — пока всё на сцене

---

**Старт дня:** _____
**Конец дня:** _____
**Реальное время:** 1,5 часа
**Что не успел:** _____

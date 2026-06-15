# ТЗ для агента: фикс аудио под требования модерации Яндекс.Игр (Mergeburgers)

## Контекст
Игра Mergeburgers (Unity 2022.3, WebGL, Zenject, PluginYG2) отклонена модерацией Яндекса по 3 аудио-замечаниям. Нужно остановить весь звук, когда игра теряет видимость, и когда показывается полноэкранная реклама.

Замечания модерации:
- п.1.3 — при сворачивании окна звук/музыка продолжает играть (надо глушить)
- п.1.3 — при переходе на другую вкладку звук продолжает играть (надо глушить)
- п.4.7 — при показе interstitial/rewarded звук не ставится на паузу (надо глушить)

## Ключевой факт об архитектуре (НЕ менять)
Аудио в WebGL реализовано через Web Audio API в .jslib-плагинах, которые УЖЕ существуют в `Assets/Plugins/WebGL/`: `WebAudioMusic.jslib`, `WebAudioSound.jslib`, `Vibration.jslib`. **Музыка и SFX используют ОДИН общий AudioContext** — `window._wam.ctx` (SFX-плагин берёт контекст у музыкального: `window._wam ? window._wam.ctx : new AudioContext()`).

Следствие: `window._wam.ctx.suspend()` глушит ВЕСЬ звук разом (музыка + SFX + звук рекламы). `resume()` возвращает. Обёртки `WebAudioMusic_Pause` (делает `ctx.suspend()`) и `WebAudioMusic_Resume` (делает `ctx.resume()`) уже есть в WebAudioMusic.jslib и работают. В C# они доступны как `WebAudioMusic.Pause()` / `WebAudioMusic.Resume()`.

В Editor (не WebGL) звук идёт через два обычных AudioSource (`_musicSource`, `_sfxSource`) на DontDestroyOnLoad-объекте.

## Конвенции проекта (соблюдать)
- Namespaces `Mergeburgers.Audio` и т.д., папки `Assets/_Project/00-Code/...`, плагины в `Assets/Plugins/WebGL/`
- `sealed` классы; DI через Zenject; plain-классы — явный `[Inject]` конструктор
- WebGL-специфичный код под `#if UNITY_WEBGL && !UNITY_EDITOR`, иначе Editor-ветка
- Подписки на внешние события отписывать в Dispose/OnDestroy
- Не добавлять плагины, не менять механику/баланс/UI — только аудио-пауза

## Задача 1. JS-мост для visibilitychange (закрывает 2 из 3 замечаний)

Создать `Assets/Plugins/WebGL/VisibilityBridge.jslib`:
```javascript
mergeInto(LibraryManager.library, {
    RegisterVisibilityBridge: function() {
        document.addEventListener('visibilitychange', function() {
            if (window._wam && window._wam.ctx) {
                if (document.hidden) {
                    if (window._wam.ctx.state === 'running') window._wam.ctx.suspend();
                } else {
                    if (window._wam.ctx.state === 'suspended') window._wam.ctx.resume();
                }
            }
        });
    }
});
```
Логика: при скрытии вкладки/сворачивании окна браузер шлёт `visibilitychange` с `document.hidden===true` → суспендим общий AudioContext → молчит всё. При возврате — резюмим. Работает напрямую с контекстом, не зависит от Unity-событий (они в браузере нестабильны — именно поэтому модерация поймала звук).

Создать C#-обёртку `Assets/_Project/00-Code/Audio/VisibilityBridge.cs`, namespace `Mergeburgers.Audio`:
```csharp
using System.Runtime.InteropServices;

namespace Mergeburgers.Audio
{
    public static class VisibilityBridge
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void RegisterVisibilityBridge();
        public static void Register() => RegisterVisibilityBridge();
#else
        public static void Register() { }
#endif
    }
}
```

## Задача 2. Методы PauseAll/ResumeAll в AudioService

В существующем `AudioService.cs` (namespace `Mergeburgers.Audio`, plain-класс с конструктором `AudioService(AudioConfig config)`):

(а) В конструкторе, в ветке `#if UNITY_WEBGL && !UNITY_EDITOR`, ПОСЛЕ `LoadAllSfx();` добавить:
```csharp
VisibilityBridge.Register();
```

(б) Добавить два публичных метода:
```csharp
public void PauseAll()
{
#if UNITY_WEBGL && !UNITY_EDITOR
    WebAudioMusic.Pause();   // suspend общего AudioContext → молчит музыка + SFX + реклама
#else
    if (_musicSource != null) _musicSource.Pause();
    if (_sfxSource != null) _sfxSource.mute = true;
#endif
}

public void ResumeAll()
{
#if UNITY_WEBGL && !UNITY_EDITOR
    WebAudioMusic.Resume();
#else
    if (_musicSource != null) _musicSource.UnPause();
    if (_sfxSource != null) _sfxSource.mute = false;
#endif
}
```
Не ломать существующие методы (PlayMenuMusic, PlaySwipe, PlaySfx, MuteSfx, SetMusicVolume и пр.) — только добавить новое.

## Задача 3. Пауза звука на рекламе через YG2 (закрывает 3-е замечание)

Создать отдельный plain-класс `Assets/_Project/00-Code/Audio/AdAudioMuter.cs` (изолирует подписку, не трогает AdService), namespace `Mergeburgers.Audio`:
```csharp
using Zenject;

namespace Mergeburgers.Audio
{
    public sealed class AdAudioMuter : IInitializable, System.IDisposable
    {
        private readonly AudioService _audio;

        [Inject]
        public AdAudioMuter(AudioService audio) => _audio = audio;

        public void Initialize()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            YG2.onOpenInterstitialAdv  += _audio.PauseAll;
            YG2.onCloseInterstitialAdv += _audio.ResumeAll;
            YG2.onErrorInterstitialAdv += _audio.ResumeAll;
            YG2.onOpenRewardedAdv      += _audio.PauseAll;
            YG2.onCloseRewardedAdv     += _audio.ResumeAll;
            YG2.onErrorRewardedAdv     += _audio.ResumeAll;
#endif
        }

        public void Dispose()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            YG2.onOpenInterstitialAdv  -= _audio.PauseAll;
            YG2.onCloseInterstitialAdv -= _audio.ResumeAll;
            YG2.onErrorInterstitialAdv -= _audio.ResumeAll;
            YG2.onOpenRewardedAdv      -= _audio.PauseAll;
            YG2.onCloseRewardedAdv     -= _audio.ResumeAll;
            YG2.onErrorRewardedAdv     -= _audio.ResumeAll;
#endif
        }
    }
}
```

ВАЖНО про имена событий: сверить с реальной версией PluginYG2 в проекте (файл вида `Assets/PluginYG2/.../YG2.cs`, искать `public static Action onOpen` / `onClose` / `onError`). Если имена/сигнатуры отличаются (напр. событие отдаёт параметр) — адаптировать подписку под фактическое API, но СМЫСЛ сохранить: открытие любой полноэкранной рекламы → PauseAll, закрытие ИЛИ ошибка → ResumeAll. Обязательно подписаться и на close, и на error — иначе при сбое загрузки рекламы звук останется выключенным навсегда.

Зарегистрировать в GameInstaller (ProjectContext, где биндится AudioService):
```csharp
Container.BindInterfacesAndSelfTo<AdAudioMuter>().AsSingle().NonLazy();
```
AudioService уже должен быть забинден как AsSingle в этом же инсталлере — проверить, что AdAudioMuter получит ту же инстанцию.

## Подстраховка (опционально, если просто)
Если в проекте есть AppLifecycleHandler (MonoBehaviour на ProjectContext, ловит OnApplicationFocus/Pause для сейва) — можно продублировать паузу и там (hasFocus=false → PauseAll, true → ResumeAll) как второй слой. Но visibilitychange (Задача 1) — основной и достаточный механизм; не полагаться на Unity-события как на единственный.

## Definition of Done
1. Компилируется под Editor и под WebGL Build Target без ошибок.
2. Новые файлы: VisibilityBridge.jslib, VisibilityBridge.cs, AdAudioMuter.cs. Изменён: AudioService.cs (Register в конструкторе + PauseAll/ResumeAll). Изменён: GameInstaller (биндинг AdAudioMuter).
3. Имена YG2-событий сверены с фактическим PluginYG2 в проекте.
4. Никакая существующая логика звука/механики/UI не сломана.

## Чеклист ручного теста (выполняет заказчик после билда)
WebGL-билд на локальном http-сервере:
1. Музыка играет → переключиться на другую вкладку → тишина → вернуться → звук играет.
2. Музыка играет → свернуть окно браузера → тишина → развернуть → звук играет.
3. Собрать 7 бургеров → interstitial → во время рекламы тишина → после рекламы звук вернулся.
4. Game Over → rewarded «+5 плиток» → во время рекламы тишина → звук вернулся (в т.ч. если реклама закрыта досрочно или не загрузилась).

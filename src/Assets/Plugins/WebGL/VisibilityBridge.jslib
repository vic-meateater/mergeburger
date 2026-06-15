mergeInto(LibraryManager.library, {
    // Единый "шлюз" над общим AudioContext (window._wam.ctx).
    // Контекст суспендится, пока активна ХОТЯ БЫ одна причина паузы
    // ('ad', 'visibility', ...), и резюмится только когда сняты ВСЕ.
    // Это исключает гонку: разворот окна во время рекламы снимает причину
    // 'visibility', но 'ad' остаётся → звук молчит, пока реклама не закрыта.
    RegisterVisibilityBridge: function() {
        var ensure = function() {
            if (!window._audioGate) {
                window._audioGate = {
                    reasons: {},
                    update: function() {
                        if (!window._wam || !window._wam.ctx) return;
                        var ctx = window._wam.ctx;
                        var paused = false;
                        for (var k in this.reasons) { if (this.reasons[k]) { paused = true; break; } }
                        if (paused) {
                            if (ctx.state === 'running') ctx.suspend();
                        } else {
                            if (ctx.state === 'suspended') ctx.resume();
                        }
                    },
                    pause: function(reason) { this.reasons[reason] = true; this.update(); },
                    resume: function(reason) { this.reasons[reason] = false; this.update(); }
                };
            }
            return window._audioGate;
        };
        window._ensureAudioGate = ensure;

        var gate = ensure();
        document.addEventListener('visibilitychange', function() {
            if (document.hidden) gate.pause('visibility');
            else gate.resume('visibility');
        });
    },

    AudioGate_Pause: function(reasonPtr) {
        if (window._ensureAudioGate) window._ensureAudioGate().pause(UTF8ToString(reasonPtr));
    },

    AudioGate_Resume: function(reasonPtr) {
        if (window._ensureAudioGate) window._ensureAudioGate().resume(UTF8ToString(reasonPtr));
    }
});

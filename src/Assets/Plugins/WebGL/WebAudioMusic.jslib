mergeInto(LibraryManager.library, {

    WebAudioMusic_Init: function() {
        if (!window._wam) {
            var ctx = new (window.AudioContext || window.webkitAudioContext)();
            window._wam = { ctx: ctx, gain: ctx.createGain(), src: null, buffer: null, playing: false, pendingPlay: false };
            window._wam.gain.connect(ctx.destination);

            // iOS Safari fix
            document.addEventListener('touchstart', function() {
                if (window._wam && window._wam.ctx.state === 'suspended')
                    window._wam.ctx.resume();
            }, { once: true });
            document.addEventListener('mousedown', function() {
                if (window._wam && window._wam.ctx.state === 'suspended')
                    window._wam.ctx.resume();
            }, { once: true });
        }
    },

    WebAudioMusic_Load: function(urlPtr) {
        var url = UTF8ToString(urlPtr);
        window._wam.buffer = null;
        fetch(url)
            .then(function(r) { return r.arrayBuffer(); })
            .then(function(buf) { return window._wam.ctx.decodeAudioData(buf); })
            .then(function(decoded) {
                var wam = window._wam;
                wam.buffer = decoded;
                if (wam.pendingPlay) {
                    wam.pendingPlay = false;
                    if (wam.src) { try { wam.src.stop(); } catch(e){} }
                    wam.src = wam.ctx.createBufferSource();
                    wam.src.buffer = wam.buffer;
                    wam.src.loop = !!wam._pendingLoop;
                    wam.src.connect(wam.gain);
                    wam.src.start(0);
                    wam.playing = true;
                    if (wam.ctx.state === 'suspended') wam.ctx.resume();
                }
            })
            .catch(function(e) { console.error('[WAM] Load error:', e); });
    },

    WebAudioMusic_Play: function(loop) {
        var wam = window._wam;
        if (!wam) return;
        if (!wam.buffer) {
            wam.pendingPlay = true;
            wam._pendingLoop = !!loop;
            return;
        }
        if (wam.src) { try { wam.src.stop(); } catch(e){} }
        wam.src = wam.ctx.createBufferSource();
        wam.src.buffer = wam.buffer;
        wam.src.loop = !!loop;
        wam.src.connect(wam.gain);
        wam.src.start(0);
        wam.playing = true;
        if (wam.ctx.state === 'suspended') wam.ctx.resume();
    },

    WebAudioMusic_Stop: function() {
        var wam = window._wam;
        if (!wam) return;
        wam.pendingPlay = false;
        if (wam.src) { try { wam.src.stop(); } catch(e){} }
        wam.playing = false;
    },

    WebAudioMusic_SetVolume: function(vol) {
        if (window._wam) window._wam.gain.gain.value = vol;
    },

    WebAudioMusic_Pause: function() {
        if (window._wam && window._wam.ctx.state === 'running')
            window._wam.ctx.suspend();
    },

    WebAudioMusic_Resume: function() {
        if (window._wam && window._wam.ctx.state === 'suspended')
            window._wam.ctx.resume();
    }
});
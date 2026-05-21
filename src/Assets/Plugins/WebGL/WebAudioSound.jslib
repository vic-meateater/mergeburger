mergeInto(LibraryManager.library, {

    WebAudioSound_Init: function() {
        if (!window._wasfx) {
            var ctx = window._wam ? window._wam.ctx : new (window.AudioContext || window.webkitAudioContext)();
            window._wasfx = { ctx: ctx, buffers: {} };
        }
    },

    WebAudioSound_Load: function(keyPtr, urlPtr) {
        var key = UTF8ToString(keyPtr);
        var url = UTF8ToString(urlPtr);
        var sfx = window._wasfx;
        fetch(url)
            .then(function(r) { return r.arrayBuffer(); })
            .then(function(buf) { return sfx.ctx.decodeAudioData(buf); })
            .then(function(decoded) { sfx.buffers[key] = decoded; })
            .catch(function(e) { console.error('[WASFX] Load error:', key, e); });
    },

    WebAudioSound_Play: function(keyPtr, volume) {
        var key = UTF8ToString(keyPtr);
        var sfx = window._wasfx;
        if (!sfx || !sfx.buffers[key]) return;
        var src = sfx.ctx.createBufferSource();
        var gain = sfx.ctx.createGain();
        gain.gain.value = volume;
        src.buffer = sfx.buffers[key];
        src.connect(gain);
        gain.connect(sfx.ctx.destination);
        src.start(0);
        if (sfx.ctx.state === 'suspended') sfx.ctx.resume();
    }
});
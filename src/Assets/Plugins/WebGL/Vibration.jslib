mergeInto(LibraryManager.library, {
    VibrateJS: function(milliseconds) {
        if (navigator.vibrate) {
            navigator.vibrate(milliseconds);
        }
    }
});
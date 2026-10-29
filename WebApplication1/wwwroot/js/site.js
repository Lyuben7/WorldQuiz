// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Simple settings panel toggle and small interactions
document.addEventListener('DOMContentLoaded', function () {
    var settingsToggle = document.getElementById('settingsToggle');
    var settingsPanel = document.getElementById('settingsPanel');
    if (settingsToggle && settingsPanel) {
        settingsToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            settingsPanel.classList.toggle('open');
            var open = settingsPanel.classList.contains('open');
            settingsPanel.setAttribute('aria-hidden', (!open).toString());
        });

        document.addEventListener('click', function (e) {
            if (!settingsPanel.contains(e.target) && !settingsToggle.contains(e.target)) {
                settingsPanel.classList.remove('open');
                settingsPanel.setAttribute('aria-hidden', 'true');
            }
        });
    }
});

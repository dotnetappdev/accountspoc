// Theme Manager - Handles light/dark mode switching
(function () {
    'use strict';

    // Initialize theme on page load
    function initializeTheme() {
        // Check localStorage for saved theme preference
        let theme = localStorage.getItem('theme');
        
        // If no saved preference, check system preference
        if (!theme) {
            const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
            theme = prefersDark ? 'dark' : 'light';
        }
        
        // Apply theme
        document.documentElement.setAttribute('data-theme', theme);
    }

    // Run on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeTheme);
    } else {
        initializeTheme();
    }

    // Listen for system theme changes
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
        // Only apply if user hasn't set a manual preference
        if (!localStorage.getItem('theme')) {
            const theme = e.matches ? 'dark' : 'light';
            document.documentElement.setAttribute('data-theme', theme);
        }
    });
})();

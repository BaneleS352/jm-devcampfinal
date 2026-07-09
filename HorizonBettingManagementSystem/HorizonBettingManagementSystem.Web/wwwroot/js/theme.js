(function () {
    const storageKey = "bet-horizon-theme";

    function getPreferredTheme() {
        const stored = localStorage.getItem(storageKey);
        if (stored) return stored;

        if (window.matchMedia("(prefers-color-scheme: dark)").matches) {
            return "dark";
        }
        return "light";
    }

    function applyTheme(theme) {
        document.documentElement.setAttribute("data-theme", theme);
    }

    function saveTheme(theme) {
        localStorage.setItem(storageKey, theme);
    }

    function initTheme() {
        const theme = getPreferredTheme();
        applyTheme(theme);

        const selector = document.getElementById("themeSelector");
        if (selector) {
            selector.value = theme;

            selector.addEventListener("change", function () {
                const selected = this.value;
                if (selected === "system") {
                    localStorage.removeItem(storageKey);
                    applyTheme(getPreferredTheme());
                } else {
                    saveTheme(selected);
                    applyTheme(selected);
                }
            });
        }
    }

    document.addEventListener("DOMContentLoaded", initTheme);
})();
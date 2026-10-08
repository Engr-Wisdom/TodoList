(function () {

    function applyTheme(theme) {

        if (theme === "dark") {
            document.documentElement.classList.add("dark-theme");
        }
        else if (theme === "light") {
            document.documentElement.classList.remove("dark-theme");
        }
        else {
            const prefersDark = window.matchMedia(
                "(prefers-color-scheme: dark)"
            ).matches;

            document.documentElement.classList.toggle(
                "dark-theme",
                prefersDark
            );
        }
    }

    const savedTheme = localStorage.getItem("taskora-theme");

    if (savedTheme) {
        applyTheme(savedTheme);
    } else {
        applyTheme("system");
    }

})();
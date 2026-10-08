document.addEventListener("DOMContentLoaded", function () {

    /*
     * =========================
     * Theme Settings
     * =========================
     */

    const themeSelect = document.querySelector("#themeSelect");

    if (themeSelect) {

        const savedTheme =
            localStorage.getItem("taskora-theme") || "system";

        themeSelect.value = savedTheme;

        themeSelect.addEventListener("change", function () {

            const theme = this.value;

            localStorage.setItem("taskora-theme", theme);

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
        });
    }


    /*
     * =========================
     * Notification Settings
     * =========================
     */

    const toggles = document.querySelectorAll(".toggle input");

    toggles.forEach(function (toggle, index) {

        const key = "taskora-toggle-" + index;
        const savedValue = localStorage.getItem(key);

        if (savedValue !== null) {
            toggle.checked = savedValue === "true";
        }

        toggle.addEventListener("change", function () {

            localStorage.setItem(
                key,
                this.checked
            );

        });
    });


    /*
     * =========================
     * Task Preferences
     * =========================
     */

    const prioritySelect =
        document.querySelector("#defaultPriority");

    if (prioritySelect) {

        const savedPriority =
            localStorage.getItem("taskora-default-priority");

        if (savedPriority !== null) {
            prioritySelect.value = savedPriority;
        }

        prioritySelect.addEventListener("change", function () {

            localStorage.setItem(
                "taskora-default-priority",
                this.value
            );

        });
    }


    /*
     * =========================
     * Show Completed Tasks
     * =========================
     */

    const completedToggle =
        document.querySelector("#showCompleted");

    if (completedToggle) {

        const savedValue =
            localStorage.getItem("taskora-show-completed");

        if (savedValue !== null) {
            completedToggle.checked =
                savedValue === "true";
        }

        completedToggle.addEventListener("change", function () {

            localStorage.setItem(
                "taskora-show-completed",
                this.checked
            );

        });
    }

});
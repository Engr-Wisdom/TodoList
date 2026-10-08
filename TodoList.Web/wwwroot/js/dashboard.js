document.addEventListener("DOMContentLoaded", function () {

    const filterButtons =
        document.querySelectorAll(".task-filters .filter-button");

    const taskCards =
        document.querySelectorAll(".task-list .task-card");


    filterButtons.forEach(function (button) {

        button.addEventListener("click", function () {

            // Remove active from all buttons
            filterButtons.forEach(function (btn) {
                btn.classList.remove("active");
            });

            // Make clicked button active
            this.classList.add("active");

            const selectedCategory =
                this.textContent.trim().toLowerCase();


            taskCards.forEach(function (card) {

                const categoryBadge =
                    card.querySelector(".category-badge");

                if (!categoryBadge) {
                    return;
                }

                const taskCategory =
                    categoryBadge.textContent.trim().toLowerCase();


                if (
                    selectedCategory === "all" ||
                    taskCategory === selectedCategory ||
                    (selectedCategory === "home" &&
                     taskCategory === "home / chores") ||
                    (selectedCategory === "church" &&
                     taskCategory === "church calling")
                ) {
                    card.style.display = "";
                }
                else {
                    card.style.display = "none";
                }

            });

        });

    });

});
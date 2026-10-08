document.addEventListener("DOMContentLoaded", function () {

    const modal = document.getElementById("deleteTaskModal");
    const deleteForm = document.getElementById("deleteTaskForm");
    const deleteTaskTitle = document.getElementById("deleteTaskTitle");

    const cancelDelete = document.getElementById("cancelDelete");
    const cancelDeleteTop = document.getElementById("cancelDeleteTop");
    const overlay = modal.querySelector(".delete-modal-overlay");

    const deleteButtons =
        document.querySelectorAll(".delete-task-button");


    function openDeleteModal(taskId, taskTitle) {

        deleteTaskTitle.textContent = taskTitle;

        deleteForm.action = `/tasks/delete/${taskId}`;

        modal.classList.add("show");

        modal.setAttribute("aria-hidden", "false");
    }


    function closeDeleteModal() {

        modal.classList.remove("show");

        modal.setAttribute("aria-hidden", "true");
    }


    deleteButtons.forEach(function (button) {

        button.addEventListener("click", function () {

            const taskId = this.dataset.taskId;
            const taskTitle = this.dataset.taskTitle;

            openDeleteModal(taskId, taskTitle);

        });

    });


    cancelDelete.addEventListener(
        "click",
        closeDeleteModal
    );


    cancelDeleteTop.addEventListener(
        "click",
        closeDeleteModal
    );


    overlay.addEventListener(
        "click",
        closeDeleteModal
    );


    document.addEventListener(
        "keydown",
        function (event) {

            if (event.key === "Escape") {
                closeDeleteModal();
            }

        }
    );

});
function openDeleteModal() {
    document.getElementById("deleteModal")
        .classList.add("show");

    document.body.classList.add("modal-open");
}

function closeDeleteModal() {
    document.getElementById("deleteModal")
        .classList.remove("show");

    document.body.classList.remove("modal-open");
}

const deleteModal = document.getElementById("deleteModal");

if (deleteModal) {
    deleteModal.addEventListener("click", function (event) {
        if (event.target === this) {
            closeDeleteModal();
        }
    });
}

document.addEventListener("keydown", function (event) {
    if (event.key === "Escape") {
        closeDeleteModal();
    }
});
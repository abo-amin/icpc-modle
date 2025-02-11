
document.addEventListener("DOMContentLoaded", function () {
    var form = document.getElementById("exam-form");
    if (form) {
        form.addEventListener("submit", function () {
            form.querySelector("button").disabled = true;
        });
    }
});

(function () {
    var form = document.getElementById("registerForm");
    form.addEventListener("submit", function (event) {
        var password = document.getElementById("password");
        var rePassword = document.getElementById("rePassword");
        var field = rePassword.closest(".field");
        var error = field.querySelector(".field-error");
        field.classList.remove("has-error");
        if (password.value && rePassword.value && password.value !== rePassword.value) {
            field.classList.add("has-error");
            error.textContent = "Password dan Re-Password tidak sama.";
            event.preventDefault();
        }
    });
})();

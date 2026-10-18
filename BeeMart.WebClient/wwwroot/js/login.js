(function () {
    var form = document.getElementById("loginForm");
    form.addEventListener("submit", function (event) {
        var username = document.getElementById("username");
        var password = document.getElementById("password");
        var valid = true;
        [username, password].forEach(function (input) {
            var field = input.closest(".field");
            var error = field.querySelector(".field-error");
            field.classList.remove("has-error");
            if (!input.value.trim()) {
                field.classList.add("has-error");
                error.textContent = "Field ini wajib diisi.";
                valid = false;
            }
        });
        if (!valid) {
            event.preventDefault();
        }
    });
})();

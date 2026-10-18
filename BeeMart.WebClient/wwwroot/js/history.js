(function () {
    var historyNote = document.getElementById("historyNote");
    var dateFilter = document.getElementById("dateFilter");
    var searchBtn = document.getElementById("searchBtn");
    var historyBody = document.getElementById("historyBody");
    var pagination = document.getElementById("pagination");

    var currentPage = 1;

    function readCookie(name) {
        var parts = document.cookie.split("; ");
        for (var i = 0; i < parts.length; i += 1) {
            var pair = parts[i].split("=");
            if (pair[0] === name) {
                return decodeURIComponent(pair[1] || "");
            }
        }
        return "";
    }

    var role = readCookie("beemart_role") || "Cashier";
    var username = readCookie("beemart_username");
    if (role === "Admin") {
        historyNote.textContent = "Pencarian Data Transaksi (Menampilkan Semua Kasir)";
    } else {
        historyNote.textContent = "Pencarian Data Transaksi (Hanya Kasir: " + username + ")";
    }

    function showToast(message, type) {
        var stack = document.getElementById("toastStack");
        var toast = document.createElement("div");
        toast.className = "toast " + (type === "error" ? "error" : "success");
        toast.textContent = message;
        stack.appendChild(toast);
        window.setTimeout(function () { toast.remove(); }, 3200);
    }

    function renderEmpty(message) {
        historyBody.textContent = "";
        var row = document.createElement("tr");
        row.className = "empty-row";
        var cell = document.createElement("td");
        cell.colSpan = 6;
        cell.textContent = message;
        row.appendChild(cell);
        historyBody.appendChild(row);
        pagination.textContent = "";
    }

    function formatDateTime(value) {
        var date = new Date(value);
        var pad = function (n) { return n < 10 ? "0" + n : String(n); };
        return date.getFullYear() + "-" + pad(date.getMonth() + 1) + "-" + pad(date.getDate()) + " " + pad(date.getHours()) + ":" + pad(date.getMinutes());
    }

    function renderRows(items) {
        historyBody.textContent = "";
        items.forEach(function (row) {
            var tr = document.createElement("tr");

            var invoiceCell = document.createElement("td");
            invoiceCell.textContent = row.invoiceNo;
            tr.appendChild(invoiceCell);

            var dateCell = document.createElement("td");
            dateCell.textContent = formatDateTime(row.transactionDate);
            tr.appendChild(dateCell);

            var cashierCell = document.createElement("td");
            cashierCell.textContent = row.cashierName;
            tr.appendChild(cashierCell);

            var itemCell = document.createElement("td");
            itemCell.textContent = String(row.itemCount);
            tr.appendChild(itemCell);

            var paymentCell = document.createElement("td");
            paymentCell.textContent = row.paymentMethod;
            tr.appendChild(paymentCell);

            var totalCell = document.createElement("td");
            totalCell.textContent = BeeMartFormat.rupiah(row.total);
            tr.appendChild(totalCell);

            historyBody.appendChild(tr);
        });
    }

    function renderPagination(totalPages) {
        pagination.textContent = "";
        if (totalPages <= 1) {
            return;
        }
        for (var page = 1; page <= totalPages; page += 1) {
            var button = document.createElement("button");
            button.type = "button";
            button.className = "page-btn" + (page === currentPage ? " is-active" : "");
            button.textContent = String(page);
            button.addEventListener(
                "click",
                (function (targetPage) {
                    return function () {
                        currentPage = targetPage;
                        loadPage();
                    };
                })(page)
            );
            pagination.appendChild(button);
        }
    }

    function loadPage() {
        fetch("/api/proxy/history?date=" + encodeURIComponent(dateFilter.value) + "&page=" + currentPage)
            .then(async function (response) {
                var payload = await response.json().catch(function () { return null; });
                if (!response.ok) {
                    throw new Error(payload && payload.message ? payload.message : "Terjadi kesalahan pada server.");
                }
                return payload;
            })
            .then(function (result) {
                if (result.items.length === 0) {
                    renderEmpty("Tidak ada transaksi pada tanggal tersebut.");
                    return;
                }
                renderRows(result.items);
                renderPagination(result.totalPages);
            })
            .catch(function (err) {
                renderEmpty("Pilih tanggal lalu klik Cari Data.");
                showToast(err.message, "error");
            });
    }

    searchBtn.addEventListener("click", function () {
        currentPage = 1;
        loadPage();
    });

    var today = new Date();
    var iso = today.getFullYear() + "-" + String(today.getMonth() + 1).padStart(2, "0") + "-" + String(today.getDate()).padStart(2, "0");
    dateFilter.value = iso;
})();

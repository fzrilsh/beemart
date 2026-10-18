(function () {
    var invoiceBanner = document.getElementById("invoiceBanner");
    var searchInput = document.getElementById("searchInput");
    var searchHint = document.getElementById("searchHint");
    var searchResultBody = document.getElementById("searchResultBody");
    var pickPanel = document.getElementById("pickPanel");
    var pickTitle = document.getElementById("pickTitle");
    var pickPrice = document.getElementById("pickPrice");
    var pickStock = document.getElementById("pickStock");
    var pickQty = document.getElementById("pickQty");
    var qtyError = document.getElementById("qtyError");
    var addToCartBtn = document.getElementById("addToCartBtn");
    var cartBody = document.getElementById("cartBody");
    var cartTotal = document.getElementById("cartTotal");
    var paymentMethod = document.getElementById("paymentMethod");
    var submitBtn = document.getElementById("submitBtn");

    var selectedProduct = null;
    var cartItems = [];
    var cartView = { items: [], total: 0 };
    var currentInvoice = "";

    function showToast(message, type) {
        var stack = document.getElementById("toastStack");
        var toast = document.createElement("div");
        toast.className = "toast " + (type === "error" ? "error" : "success");
        toast.textContent = message;
        stack.appendChild(toast);
        window.setTimeout(function () { toast.remove(); }, 3200);
    }

    async function callApi(url, options) {
        var response = await fetch(url, options);
        var payload = await response.json().catch(function () { return null; });
        if (!response.ok) {
            throw new Error(payload && payload.message ? payload.message : "Terjadi kesalahan pada server.");
        }
        return payload;
    }

    function emptyRow(cellCount, message) {
        var row = document.createElement("tr");
        row.className = "empty-row";
        var cell = document.createElement("td");
        cell.colSpan = cellCount;
        cell.textContent = message;
        row.appendChild(cell);
        return row;
    }

    function renderInvoice() {
        invoiceBanner.textContent = currentInvoice ? "Invoice No: " + currentInvoice : "Invoice No: memuat...";
    }

    function renderCart() {
        cartBody.textContent = "";
        if (cartView.items.length === 0) {
            cartBody.appendChild(emptyRow(6, "Belum ada barang di keranjang."));
        } else {
            cartView.items.forEach(function (item) {
                var row = document.createElement("tr");

                var nameCell = document.createElement("td");
                nameCell.textContent = item.name;
                row.appendChild(nameCell);

                var priceCell = document.createElement("td");
                priceCell.textContent = BeeMartFormat.rupiah(item.price);
                row.appendChild(priceCell);

                var discountCell = document.createElement("td");
                discountCell.textContent = BeeMartFormat.percent(item.discount);
                row.appendChild(discountCell);

                var qtyCell = document.createElement("td");
                qtyCell.textContent = String(item.qty);
                row.appendChild(qtyCell);

                var subtotalCell = document.createElement("td");
                subtotalCell.textContent = BeeMartFormat.rupiah(item.subTotal);
                row.appendChild(subtotalCell);

                var actionCell = document.createElement("td");
                var removeBtn = document.createElement("button");
                removeBtn.type = "button";
                removeBtn.className = "btn btn-danger";
                removeBtn.textContent = "Hapus";
                removeBtn.addEventListener("click", function () {
                    cartItems = cartItems.filter(function (entry) {
                        return entry.idProduct !== item.idProduct;
                    });
                    refreshCart();
                    showToast(item.name + " dihapus dari keranjang.", "success");
                });
                actionCell.appendChild(removeBtn);
                row.appendChild(actionCell);

                cartBody.appendChild(row);
            });
        }
        cartTotal.textContent = "Total: " + BeeMartFormat.rupiah(cartView.total);
    }

    function refreshCart() {
        if (cartItems.length === 0) {
            cartView = { items: [], total: 0 };
            renderCart();
            return;
        }
        callApi("/api/proxy/transactions/calculate", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ items: cartItems })
        })
            .then(function (result) {
                cartView = result;
                renderCart();
            })
            .catch(function (err) {
                showToast(err.message, "error");
            });
    }

    function hidePickPanel() {
        selectedProduct = null;
        pickPanel.classList.add("hidden");
        qtyError.classList.remove("is-visible");
        qtyError.textContent = "";
    }

    function renderSearchResults(results, keyword) {
        searchResultBody.textContent = "";
        if (results.length === 0) {
            searchResultBody.appendChild(emptyRow(6, 'Tidak ada barang yang cocok dengan "' + keyword + '".'));
            return;
        }

        results.forEach(function (product) {
            var row = document.createElement("tr");

            var nameCell = document.createElement("td");
            nameCell.textContent = product.name;
            row.appendChild(nameCell);

            var weightCell = document.createElement("td");
            weightCell.textContent = BeeMartFormat.weight(product.weightGram);
            row.appendChild(weightCell);

            var stockCell = document.createElement("td");
            if (product.stock === 0) {
                stockCell.textContent = "0";
                stockCell.className = "stock-out";
            } else {
                stockCell.textContent = String(product.stock);
            }
            row.appendChild(stockCell);

            var priceCell = document.createElement("td");
            priceCell.textContent = BeeMartFormat.rupiah(product.price);
            row.appendChild(priceCell);

            var discountCell = document.createElement("td");
            discountCell.textContent = BeeMartFormat.percent(product.discount);
            row.appendChild(discountCell);

            var actionCell = document.createElement("td");
            var pickBtn = document.createElement("button");
            pickBtn.type = "button";
            pickBtn.className = "btn btn-small";
            pickBtn.textContent = "Pilih";
            pickBtn.addEventListener("click", function () {
                selectProduct(product);
            });
            actionCell.appendChild(pickBtn);
            row.appendChild(actionCell);

            searchResultBody.appendChild(row);
        });
    }

    function selectProduct(product) {
        selectedProduct = product;
        pickPanel.classList.remove("hidden");
        pickTitle.textContent = "Barang Terpilih: " + product.name;
        pickPrice.textContent = BeeMartFormat.rupiah(product.price);
        pickStock.textContent = String(product.stock);
        pickQty.value = "1";
        qtyError.classList.remove("is-visible");
        qtyError.textContent = "";
        pickQty.focus();
    }

    function search(keyword) {
        var trimmed = keyword.trim();
        hidePickPanel();
        if (!trimmed) {
            searchHint.textContent = "Ketik nama barang lalu tekan Enter untuk mencari.";
            searchResultBody.textContent = "";
            searchResultBody.appendChild(emptyRow(6, "Ketik nama barang lalu tekan Enter untuk mencari."));
            return;
        }

        callApi("/api/proxy/products/search?keyword=" + encodeURIComponent(trimmed))
            .then(function (results) {
                searchHint.textContent = "Menampilkan " + results.length + ' hasil untuk "' + trimmed + '".';
                renderSearchResults(results, trimmed);
            })
            .catch(function (err) {
                showToast(err.message, "error");
            });
    }

    function addToCart() {
        if (!selectedProduct) {
            return;
        }

        var qty = parseInt(pickQty.value, 10);
        if (isNaN(qty) || qty < 1) {
            qtyError.textContent = "Qty minimal 1.";
            qtyError.classList.add("is-visible");
            return;
        }

        var nextItems = cartItems.slice();
        var found = false;
        for (var i = 0; i < nextItems.length; i += 1) {
            if (nextItems[i].idProduct === selectedProduct.idProduct) {
                nextItems[i] = { idProduct: nextItems[i].idProduct, qty: nextItems[i].qty + qty };
                found = true;
                break;
            }
        }
        if (!found) {
            nextItems.push({ idProduct: selectedProduct.idProduct, qty: qty });
        }

        callApi("/api/proxy/transactions/calculate", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ items: nextItems })
        })
            .then(function (result) {
                cartItems = nextItems;
                cartView = result;
                renderCart();
                qtyError.classList.remove("is-visible");
                qtyError.textContent = "";
                showToast(selectedProduct.name + " ditambahkan ke keranjang.", "success");
            })
            .catch(function (err) {
                qtyError.textContent = err.message;
                qtyError.classList.add("is-visible");
            });
    }

    function loadInvoice() {
        renderInvoice();
        callApi("/api/proxy/invoices/lock", { method: "POST" })
            .then(function (lock) {
                currentInvoice = lock.invoiceNo;
                renderInvoice();
            })
            .catch(function (err) {
                showToast(err.message, "error");
            });
    }

    function loadPaymentMethods() {
        callApi("/api/proxy/payment-methods")
            .then(function (methods) {
                paymentMethod.textContent = "";
                methods.forEach(function (method) {
                    var option = document.createElement("option");
                    option.value = String(method.idPaymentMethod);
                    option.textContent = method.methodName;
                    paymentMethod.appendChild(option);
                });
            })
            .catch(function (err) {
                showToast(err.message, "error");
            });
    }

    function resetTransaction() {
        cartItems = [];
        cartView = { items: [], total: 0 };
        renderCart();
        searchInput.value = "";
        searchHint.textContent = "";
        searchResultBody.textContent = "";
        searchResultBody.appendChild(emptyRow(6, "Ketik nama barang lalu tekan Enter untuk mencari."));
        hidePickPanel();
        paymentMethod.selectedIndex = 0;
        loadInvoice();
    }

    function submitTransaction() {
        if (cartItems.length === 0) {
            showToast("Keranjang masih kosong. Tambahkan barang terlebih dahulu.", "error");
            return;
        }

        submitBtn.disabled = true;
        callApi("/api/proxy/transactions", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                invoiceNo: currentInvoice,
                idPaymentMethod: Number(paymentMethod.value),
                items: cartItems
            })
        })
            .then(function (result) {
                showToast("Transaksi " + result.invoiceNo + " berhasil disimpan.", "success");
                resetTransaction();
            })
            .catch(function (err) {
                showToast(err.message, "error");
                refreshCart();
            })
            .finally(function () {
                submitBtn.disabled = false;
            });
    }

    searchInput.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            search(searchInput.value);
        }
    });

    pickQty.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            addToCart();
        }
    });

    pickQty.addEventListener("input", function () {
        qtyError.classList.remove("is-visible");
    });

    addToCartBtn.addEventListener("click", addToCart);
    submitBtn.addEventListener("click", submitTransaction);

    renderCart();
    loadPaymentMethods();
    loadInvoice();
})();

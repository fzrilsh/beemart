# BeeMart

## Fitur Tambahan: Invoice Lock

Soal menyebutkan WebClient tidak boleh mengakses database dan seluruh data harus diperoleh melalui API. Karena itu pembentukan nomor invoice dilakukan di Backend API, bukan di frontend.

Agar nomor invoice tidak bentrok saat halaman dibuka di beberapa tab atau ada request bersamaan, API mengunci nomor invoice terlebih dahulu di tabel tambahan `TrInvoiceLock`.

Cara kerja singkat:

1. Saat halaman New Transaction dibuka, WebClient meminta nomor invoice ke API.
2. API membuat nomor invoice, menyimpannya ke `TrInvoiceLock`, lalu mengembalikannya.
3. Satu pengguna hanya memiliki satu lock aktif per hari. Membuka halaman lagi pada hari yang sama mengembalikan nomor yang sama.
4. Saat transaksi disubmit, API memvalidasi lock, menyimpan transaksi, mengurangi stok, lalu menghapus lock.
5. Lock yang tanggalnya sudah lewat dihapus saat pengguna meminta invoice berikutnya.

Lima tabel dari ERD tidak berubah: `MsUser`, `MsProduct`, `LtPaymentMethod`, `TrTransaction`, dan `TrTransactionDetail`. `TrInvoiceLock` hanya tabel tambahan untuk fitur ini.

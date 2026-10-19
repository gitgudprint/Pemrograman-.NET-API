# IF133 - Tugas 1: Backend Skeleton & Data Persistence

## Identitas
- Nama: Muhammad Elbra H
- NIM: 1003250036
- Kelas: `<isi kelas>`

## Studi Kasus
**Produk / Inventory.** Aplikasi menyimpan data produk sebuah toko (nama, deskripsi, harga, stok) dan menyediakan operasi CRUD melalui REST API.

Entity utama: **`Product`**

| Field | Tipe | Wajib | Aturan validasi |
|---|---|---|---|
| `Id` | `int` | Ya | Primary key, dibuat otomatis oleh database (AUTOINCREMENT) |
| `Name` | `string` | Ya | Tidak boleh kosong, maksimal 100 karakter |
| `Description` | `string?` | Tidak | Maksimal 500 karakter |
| `Price` | `decimal` | Ya | `>= 0` |
| `Stock` | `int` | Ya | `>= 0` |

## Teknologi
- .NET SDK 10.0.402
- ASP.NET Core Web API (berbasis controller)
- Entity Framework Core 10.0.13
- Database provider: **SQLite** (`Microsoft.EntityFrameworkCore.Sqlite`)
- Swagger UI: `Swashbuckle.AspNetCore` 10.3.0

## Struktur Project

```
IF133_Tugas1_1003250036_Muhammad Elbra H/
├── IF133.Tugas1.slnx              # Solution (format .slnx bawaan .NET 10)
├── IF133.Tugas1.Api/              # Project Web API
│   ├── Controllers/
│   │   └── ProductsController.cs  # Endpoint CRUD /api/products
│   ├── Data/
│   │   └── AppDbContext.cs        # DbContext + DbSet<Product> + konfigurasi kolom
│   ├── Entities/
│   │   └── Product.cs             # Entity + aturan validasi (Data Annotations)
│   ├── Migrations/                # File migration hasil "dotnet ef migrations add"
│   ├── Properties/launchSettings.json
│   ├── Program.cs                 # Registrasi DbContext (DI), controller, Swagger
│   ├── appsettings.json           # Connection string database
│   ├── app.db                     # Database SQLite
│   └── IF133.Tugas1.Api.csproj
├── docs/
│   └── screenshots/               # Bukti gambar
└── README.md
```

![Struktur project di VS Code](docs/screenshots/01%20struktur%20project.png)

**Hubungan antar komponen:**
`Product` (entity) didaftarkan sebagai `DbSet<Product>` di `AppDbContext`. Dari model tersebut, **migration** menghasilkan perintah pembuatan tabel `Products` di **database** SQLite. `AppDbContext` disuntikkan ke `ProductsController` melalui dependency injection. Setiap **endpoint** di controller dipetakan ke satu **HTTP method** (GET/POST/PUT/DELETE) dan mengembalikan **response** dengan status code yang sesuai.

## Entity dan DbContext

`Entities/Product.cs` mendefinisikan field dan validasi. Karena controller memakai atribut `[ApiController]`, request yang melanggar validasi otomatis ditolak dengan **400 Bad Request** sebelum masuk ke method controller.

`Data/AppDbContext.cs` berisi `DbSet<Product> Products` serta konfigurasi kolom: `Name` wajib dan maksimal 100 karakter, `Description` maksimal 500 karakter, dan `Price` dengan presisi `(18,2)`.

![Entity Product](docs/screenshots/02%20entity.png)
![AppDbContext](docs/screenshots/03%20dbcontext.png)

## Konfigurasi Database

Connection string disimpan di `appsettings.json`, tidak di controller:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=app.db"
}
```

DbContext didaftarkan di `Program.cs` melalui dependency injection:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```

![appsettings.json](docs/screenshots/04%20appsettings.png)
![Program.cs](docs/screenshots/05%20program.png)

## Migration

```powershell
dotnet ef migrations add InitialCreate --project IF133.Tugas1.Api
dotnet ef database update --project IF133.Tugas1.Api
```

Hasilnya, migration `20261018144550_InitialCreate` berhasil dibuat dan diterapkan, lalu tabel `Products` terbentuk:

```sql
CREATE TABLE "Products" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Products" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Price" TEXT NOT NULL,
    "Stock" INTEGER NOT NULL
);
```

![dotnet ef migrations add](docs/screenshots/06%20migration%20add.png)
![dotnet ef database update](docs/screenshots/07%20database%20update.png)
![File migration](docs/screenshots/08%20migration%20file.png)

Struktur tabel `Products` dilihat di VS Code (extension SQLite Viewer):

![Struktur tabel Products](docs/screenshots/09%20struktur%20tabel.png)

## Cara Menjalankan

Prasyarat: .NET SDK 10.

```powershell
# 1. Restore dependency
dotnet restore

# 2. Install tool EF Core (sekali saja)
dotnet tool install --global dotnet-ef

# 3. Update database (membuat app.db dari migration)
dotnet ef database update --project IF133.Tugas1.Api

# 4. Jalankan project
dotnet run --project IF133.Tugas1.Api --launch-profile http
```

Setelah itu buka Swagger UI di **http://localhost:5251/swagger**.

> File `app.db` sudah disertakan, sehingga langkah 3 boleh dilewati. Tabel `Products` di dalamnya kosong karena data uji dihapus pada skenario DELETE.

![dotnet run](docs/screenshots/10%20dotnet%20run.png)

Skeleton awal (sebelum database ditambahkan) juga sudah dibuktikan dapat dijalankan, dengan endpoint bawaan `/weatherforecast` yang mengembalikan data:

![Skeleton berjalan sebelum database ditambahkan](docs/screenshots/11%20skeleton.png)

## Endpoint

| Operasi | Method | Endpoint | Body | Success | Error |
|---|---|---|---|---|---|
| Read all | GET | `/api/products` | - | 200 OK | - |
| Read one | GET | `/api/products/{id}` | - | 200 OK | 404 Not Found |
| Create | POST | `/api/products` | JSON | 201 Created (+ header `Location`) | 400 Bad Request |
| Update | PUT | `/api/products/{id}` | JSON | 200 OK (data terbaru) | 400 / 404 |
| Delete | DELETE | `/api/products/{id}` | - | 204 No Content | 404 Not Found |

Contoh body POST/PUT:

```json
{
  "name": "Keyboard Mechanical",
  "description": "Keyboard 75%",
  "price": 350000,
  "stock": 5
}
```

![Daftar endpoint di Swagger](docs/screenshots/12%20swagger.png)
![ProductsController (1)](docs/screenshots/13%20controller%201.png)
![ProductsController (2)](docs/screenshots/14%20controller%202.png)

## Bukti Pengujian

Ketujuh skenario pada bagian F diuji melalui **Swagger UI** (Try it out → Execute), secara berurutan pada database yang baru dibuat.

| No | Skenario | Method & Endpoint | Input | Expected | Hasil | Response |
|---|---|---|---|---|---|---|
| 1 | GET seluruh data | `GET /api/products` | - | 200 + array/list | ✅ 200 | `[]` (database masih kosong) |
| 2 | POST data valid | `POST /api/products` | Keyboard Mechanical, 350000, stok 5 | 201 + data baru | ✅ 201 | `{"id":1,"name":"Keyboard Mechanical",...}` |
| 3 | GET ID valid | `GET /api/products/1` | - | 200 + satu object | ✅ 200 | `{"id":1,"name":"Keyboard Mechanical",...}` |
| 4 | PUT data valid | `PUT /api/products/1` | name → "Keyboard Mechanical Pro", price 425000, stok 8 | 200 atau 204 | ✅ 200 | data produk setelah diubah |
| 5 | DELETE data valid | `DELETE /api/products/1` | - | 204 | ✅ 204 | (tanpa body) |
| 6 | GET ID tidak ada | `GET /api/products/999` | - | 404 | ✅ 404 | `{"message":"Product dengan id 999 tidak ditemukan."}` |
| 7 | POST data tidak valid | `POST /api/products` | name `""`, price -1000, stock -5 | 400 | ✅ 400 | errors: Name wajib diisi, Price tidak boleh negatif, Stock tidak boleh negatif |

Hasil: **7 dari 7 skenario sesuai expected result.**

### Screenshot Swagger

**1. GET seluruh data → 200 OK**
![Test 1](docs/screenshots/15%20get200.png)

**2. POST data valid → 201 Created**
![Test 2](docs/screenshots/16%20post201.png)

**3. GET ID valid → 200 OK**
![Test 3](docs/screenshots/17%20getid200.png)

**4. PUT data valid → 200 OK**
![Test 4](docs/screenshots/18%20put200.png)

**5. DELETE data valid → 204 No Content**
![Test 5](docs/screenshots/19%20delete204.png)

**6. GET ID tidak ada → 404 Not Found**
![Test 6](docs/screenshots/20%20get404.png)

**7. POST data tidak valid → 400 Bad Request**
![Test 7](docs/screenshots/21%20post400.png)

## Kendala dan Solusi

| No | Kendala | Penyebab | Solusi |
|---|---|---|---|
| 1 | Perintah `dotnet` gagal dengan pesan *"No .NET SDKs were found"* | Di komputer hanya ada .NET host/runtime, belum ada SDK | Memasang .NET SDK 10 dari dotnet.microsoft.com, lalu memverifikasi dengan `dotnet --list-sdks` |
| 2 | Di terminal baru, `dotnet ef migrations add` gagal: *"Could not execute because the specified command or file was not found ... dotnet-ef does not exist"* | Terminal dibuka dengan PATH lama (sebelum `dotnet tool install --global dotnet-ef`), sehingga folder `%USERPROFILE%\.dotnet\tools` belum terbaca | Menutup lalu membuka ulang terminal (atau VS Code) agar PATH terbaru dimuat, kemudian menjalankan ulang command migration |
| 3 | Swagger UI tidak tersedia setelah project dibuat | Mulai .NET 9, template `webapi` hanya menyediakan dokumen OpenAPI (`/openapi/v1.json`) tanpa Swagger UI | Menambahkan paket `Swashbuckle.AspNetCore`, lalu memanggil `AddSwaggerGen()`, `UseSwagger()`, dan `UseSwaggerUI()` di `Program.cs` |
| 4 | Pada POST dengan `name` kosong muncul dua pesan error sekaligus: "Name wajib diisi." dan "Name maksimal 100 karakter." | Atribut `[StringLength(100, MinimumLength = 1)]` juga ikut gagal untuk string kosong, sehingga muncul pesan yang tidak relevan | `MinimumLength` dihapus karena string kosong sudah ditangani oleh `[Required]`. Pengujian diulang dan sekarang hanya muncul "Name wajib diisi." |
| 5 | Kolom `Price` bertipe `TEXT` di database | SQLite tidak memiliki tipe `decimal`, sehingga EF Core menyimpannya sebagai TEXT agar presisinya tidak hilang | Diterima sebagai perilaku normal provider SQLite. Presisi diatur dengan `HasPrecision(18, 2)`, dan nilai tetap dibaca kembali sebagai `decimal` dengan benar |
| 6 | Muncul file tambahan `app.db-wal` dan `app.db-shm` | EF Core 10 mengaktifkan mode WAL di SQLite, sehingga perubahan terbaru bisa masih tersimpan di file `-wal` | Sebelum dikumpulkan dilakukan checkpoint (`PRAGMA wal_checkpoint(TRUNCATE)`) agar semua data masuk ke `app.db`, lalu kedua file tambahan dihapus |
| 7 | File solution berekstensi `.slnx`, bukan `.sln` | .NET 10 memakai format solution XML yang baru secara default | Tetap digunakan karena bisa dibuka di Visual Studio 2022 (17.13+) dan VS Code C# Dev Kit, serta didukung oleh `dotnet build` |

## Kesimpulan

- Project ASP.NET Core Web API berhasil dibuat dan dijalankan dengan struktur folder yang terpisah sesuai tanggung jawabnya (`Controllers`, `Data`, `Entities`, `Migrations`).
- Persistence data dengan EF Core dan SQLite berjalan. Migration `InitialCreate` berhasil membuat tabel `Products`, dan data yang dikirim lewat API tersimpan ke database.
- Kelima endpoint CRUD (GET all, GET by id, POST, PUT, DELETE) berfungsi dengan status code yang sesuai: 200, 201, 204, 400, dan 404.
- Ketujuh skenario uji di Swagger UI berhasil, termasuk 2 skenario gagal (404 untuk ID yang tidak ada, 400 untuk input tidak valid).

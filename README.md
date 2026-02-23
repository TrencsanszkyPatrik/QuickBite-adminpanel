# 🎨 QuickBite Admin Panel (WPF)

Modern desktop admin alkalmazás a QuickBite ételfutár platform kezeléséhez.

## 📋 Funkciók

### ✅ Bejelentkezés
- **Super Admin** bejelentkezés email + jelszóval
- JWT token alapú autentikáció
- Modern, user-friendly UI
- Full screen mód

### ✅ Super Admin Dashboard
- **Statisztikák:**
  - Összes étterem
  - Összes admin
  - Összes rendelés  
  - Teljes bevétel (Ft)
  
- **Admin kezelés:**
  - Adminok listázása
  - Új admin hozzáadása (Restaurant Admin vagy Super Admin)
  - Admin törlése
  - Részletes adminadatok

## 🚀 Indítás

### Előfeltételek
- .NET 10.0 SDK vagy .NET 8.0 SDK
- Futó QuickBite Backend API (`https://localhost:7289`)

### Backend API indítása
```powershell
cd G:\Vizsgaremek\QuickBite-Backend\quickbiteback
dotnet run
```

### WPF App indítása
```powershell
cd G:\Vizsgaremek\QuickBite-AdminPanel\Quickbite-AdminPanel
dotnet run
```

Vagy Visual Studio-ban:
1. Nyisd meg a `Quickbite-AdminPanel.sln`-t
2. F5 (Start Debugging)

## 🔑 Teszt Super Admin Fiókok

Az SQL migration után megadott adminok:

**1. Patrik (Super Admin)**
- Email: `padmin@gmail.com`
- Jelszó: _(eredeti jelszó - lásd migration script)_

**2. Martin (Super Admin)**
- Email: `madmin@gmail.com`
- Jelszó: _(eredeti jelszó - lásd migration script)_

> ⚠️ **Fontos:** Csak `super_admin` szerepkörű felhasználók jelentkezhetnek be!

## 🏗️ Projekt Struktúra

```
Quickbite-AdminPanel/
├── Models/
│   └── ApiModels.cs           # API request/response modellek
├── Services/
│   └── ApiService.cs          # HTTP API kommunikáció
├── Views/
│   ├── LoginWindow.xaml       # Bejelentkezési ablak
│   ├── LoginWindow.xaml.cs
│   ├── SuperAdminWindow.xaml  # Fő admin dashboard
│   ├── SuperAdminWindow.xaml.cs
│   ├── AddAdminDialog.xaml    # Új admin hozzáadása dialog
│   └── AddAdminDialog.xaml.cs
├── App.xaml                   # Alkalmazás beállítások
└── App.xaml.cs
```

## 🎨 Design Rendszer

### Színek
- **Primary:** `#6B4CE6` (lila)
- **Secondary:** `#8C7AE6` (világos lila)
- **Background:** `#F5F7FA` (világos szürke)
- **Card Background:** `White`
- **Text:** `#2D3748` (sötét szürke)
- **Success:** `#48BB78` (zöld)
- **Danger:** `#F56565` (piros)

### UI Komponensek
- Modern card design árnyékolással
- Kerekített sarkok (6-16px)
- Egységes tipográfia
- Responsive grid layout

## 🔌 API Végpontok

### Bejelentkezés
```
POST https://localhost:7289/api/admin/auth/login
Body: { "email": "...", "password": "..." }
```

### Dashboard Statisztikák
```
GET https://localhost:7289/api/super-admin/dashboard
Authorization: Bearer {token}
```

### Adminok Listázása
```
GET https://localhost:7289/api/super-admin/admins
Authorization: Bearer {token}
```

### Új Admin Létrehozása
```
POST https://localhost:7289/api/super-admin/admins
Authorization: Bearer {token}
Body: { "name": "...", "email": "...", "password": "...", "role": "..." }
```

### Admin Törlése
```
DELETE https://localhost:7289/api/super-admin/admins/{id}
Authorization: Bearer {token}
```

## 🛠️ Használt Technológiák

- **WPF** (Windows Presentation Foundation)
- **.NET 10.0** (vagy .NET 8.0)
- **Newtonsoft.Json** - JSON serialization
- **HttpClient** - HTTP API kommunikáció

## 📦 NuGet Csomagok

```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

## 🔧 Beállítások

### API URL módosítása

Ha a backend más porton fut, módosítsd az `ApiService.cs` fájlban:

```csharp
public ApiService()
{
    _httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7289/")  // <-- Itt
    };
}
```

## 📝 Funkciók Részletesen

### 1. Login Screen
- Email és jelszó mezők
- Validáció
- Enter key támogatás
- Hibakezelés
- Loading állapot

### 2. Super Admin Panel
- **Dashboard kártyák:**
  - Real-time statisztikák
  - Színes ikonok
  - Formázott számok (Ft)

- **Admin táblázat:**
  - Sortable columns
  - Törlés gomb minden sornál
  - Modern DataGrid design

- **Új Admin Dialog:**
  - Név, email, jelszó mezők
  - Role választó (ComboBox)
  - Validáció
  - Sikeres létrehozás után automatikus frissítés

### 3. Biztonság
- JWT token tárolása memóriában
- Automatikus Authorization header hozzáadása
- Role-based access (csak super_admin)
- Kijelentkezés funkció

## 🐛 Hibakeresés

### "Nem sikerült csatlakozni a szerverhez"
- Ellenőrizd, hogy a backend fut-e
- Nézd meg, hogy a helyes port van-e beállítva

### "Hibás email vagy jelszó"
- Ellenőrizd az SQL migration futtatását
- Nézd meg a `users` táblában a `role` mezőt

### Build hibák
```powershell
dotnet clean
dotnet restore
dotnet build
```

## 📊 Jövőbeli Fejlesztések

- [ ] Restaurant admin panel
- [ ] Étterem kezelés
- [ ] Étlap CRUD műveletek
- [ ] Kép feltöltés
- [ ] Rendelések kezelése
- [ ] Real-time értesítések (SignalR)
- [ ] Export funkciók (PDF, Excel)
- [ ] Részletes statisztikák grafikonokkal

---

**Készítette:** QuickBite Team  
**Létrehozva:** 2026-02-23  
**Verzió:** 1.0.0

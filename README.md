# QuickBite AdminPanel

Ez a projekt a **QuickBite vizsgaremek** admin felülete (WPF). A célja, hogy az adminisztrátorok és az éttermi adminok egy biztonságos, asztali kezelőfelületen keresztül tudják menedzselni az éttermeket, admin jogosultságokat és a rendszerhez kapcsolódó adminisztratív műveleteket.

## Adminpanel mappaszerkezet (részletes)

```text
QuickBite-adminpanel/
├── Quickbite-AdminPanel.sln             # .NET solution fájl
└── Quickbite-AdminPanel/                # WPF alkalmazás projekt
    ├── MainWindow.xaml                  # Példa főablak nézet
    ├── MainWindow.xaml.cs               # Példa főablak code-behind
    ├── Converters/                      # UI konverterek
    │   ├── BoolToColorConverter.cs      # Példa: logikai érték -> szín
    │   └── ...
    ├── Models/                          # API-hoz és UI-hoz kapcsolódó modellek
    │   ├── ApiModels.cs                 # Példa modellgyűjtemény
    │   └── ...
    ├── Services/                        # Külső kommunikáció / API hívások
    │   ├── ApiService.cs                # Backend hívások
    │   └── ...
    ├── Views/                           # Ablakok, dialógusok, admin nézetek
    │   ├── LoginWindow.xaml             # Bejelentkezési nézet
    │   ├── SuperAdminWindow.xaml        # Super admin nézet
    │   └── ...                          # További admin és étteremkezelő nézetek
```

## Gyors indítás

```bash
dotnet restore
dotnet run --project ./Quickbite-AdminPanel/Quickbite-AdminPanel.csproj
```

Alternatív lehetőségként a solution megnyitható Visual Studio-ban is a `Quickbite-AdminPanel.sln` fájllal, és onnan indítható.

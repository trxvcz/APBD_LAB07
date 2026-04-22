# Clinic Appointment Manager (ADO.NET)

System backendowy typu REST API zbudowany w technologii ASP.NET Core, służący do zarządzania wizytami w przychodni medycznej. Projekt realizuje pełną komunikację z bazą danych SQL Server za pomocą **ADO.NET**, bez użycia systemów ORM (takich jak Entity Framework).

## 🎯 Cel projektu
Głównym celem zadania jest demonstracja umiejętności niskopoziomowej pracy z bazą danych w ekosystemie .NET:
* Ręczne zarządzanie połączeniami (`SqlConnection`).
* Wykorzystanie `SqlCommand` i `SqlDataReader` do operacji CRUD.
* Ochrona przed **SQL Injection** poprzez pełną parametryzację zapytań.
* Mapowanie wyników z bazy danych na obiekty **DTO**.
* Obsługa reguł biznesowych i zwracanie odpowiednich statusów HTTP.

## 🛠 Technologie
* **Język:** C#
* **Framework:** ASP.NET Core Web API
* **Baza danych:** SQL Server
* **Biblioteka dostępu do danych:** `Microsoft.Data.SqlClient`

## 🗄️ Model Bazy Danych
System opiera się na czterech relacyjnych tabelach:
1.  **Specializations** – Słownik specjalizacji lekarskich.
2.  **Doctors** – Dane lekarzy wraz z przypisaniem do specjalizacji.
3.  **Patients** – Dane pacjentów przychodni.
4.  **Appointments** – Rejestr wizyt łączący pacjentów i lekarzy.

## 🚀 Dokumentacja API (Endpointy)

| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| `GET` | `/api/appointments` | Pobiera listę wizyt. Obsługuje filtry query: `status` oraz `patientLastName`. |
| `GET` | `/api/appointments/{id}` | Pobiera szczegółowe dane konkretnej wizyty. |
| `POST` | `/api/appointments` | Rejestruje nową wizytę (status domyślny: `Scheduled`). |
| `PUT` | `/api/appointments/{id}` | Aktualizuje dane istniejącej wizyty. |
| `DELETE` | `/api/appointments/{id}` | Usuwa wizytę z bazy danych. |

## ⚙️ Logika i Reguły Biznesowe
* **Walidacja terminów:** Nowa wizyta nie może być zaplanowana w przeszłości.
* **Konflikty:** System uniemożliwia zapisanie lekarza na dwie różne wizyty w tym samym terminie (**409 Conflict**).
* **Statusy:** Nie można usunąć ani zmienić daty wizyty, która ma już status `Completed`.
* **Integralność:** Przed zapisem system sprawdza, czy dany pacjent i lekarz istnieją oraz czy są aktywni.
* **Bezpieczeństwo:** Zabronione jest używanie interpolacji stringów w zapytaniach SQL.

## 🏗️ Instrukcja Uruchomienia

1.  **Przygotowanie bazy:**
    Uruchom skrypt `01_create_and_seed_clinic.sql` na swoim serwerze SQL, aby utworzyć strukturę tabel i dane testowe.

2.  **Konfiguracja połączenia:**
    W pliku `appsettings.json` skonfiguruj `ConnectionString` do Twojej bazy:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=localhost,1433;Database=ClinicAdoNet;User Id=sa;Password=TwojeHaslo;TrustServerCertificate=True"
      }
    }
    ```

3.  **Instalacja paczek:**
    W katalogu projektu wykonaj polecenie:
    ```bash
    dotnet add package Microsoft.Data.SqlClient
    ```

4.  **Uruchomienie:**
    ```bash
    dotnet run
    ```

## 📝 Wymagane statusy HTTP
* `200 OK` – Poprawny odczyt/aktualizacja.
* `201 Created` – Pomyślne utworzenie wizyty.
* `204 No Content` – Pomyślne usunięcie.
* `400 Bad Request` – Błędne dane wejściowe.
* `404 Not Found` – Zasób nie istnieje.
* `409 Conflict` – Naruszenie reguł biznesowych (np. zajęty termin).

---
*Projekt wykonany w ramach ćwiczeń z przedmiotu APBD.*

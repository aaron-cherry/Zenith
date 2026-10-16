# Zenith Workout App (v1.0.0)

Zenith is a high-performance, full-stack fitness and workout tracking application designed to handle standard resistance training, custom modalities, and progressive overload tracking. 

Built with a decoupled client-server architecture, Zenith pairs a cross-platform **.NET MAUI** mobile client with an **ASP.NET Core Web API** hosted in Docker on **Render**, backed by a serverless **PostgreSQL** instance on **Neon** leveraging native binary JSON (`jsonb`).

---

## 1. System Architecture

```
┌───────────────────────────────────────┐
│        Client: .NET MAUI App          │
│               (Android)               │
│   - MVVM & Modular CustomComponents   │
│   - Debounced Autocomplete Typeahead  │
│   - Dynamic Set Rows & Blur-Save      │
└──────────────────┬────────────────────┘
                   │ HTTPS (TLS encrypted)
                   ▼
┌───────────────────────────────────────┐
│       Backend: ASP.NET Core 8 API     │
│       (Docker Container on Render)    │
│   - RESTful Resource Controllers      │
│   - Entity Framework Core 8           │
│   - Npgsql DataSource with JSONB      │
└──────────────────┬────────────────────┘
                   │ TCP 5432 (SSL/TLS)
                   ▼
┌───────────────────────────────────────┐
│    Database: PostgreSQL 16 (Neon)     │
│   - Relational Core (Workouts/Ex)     │
│   - Unstructured JSONB (Set Metrics)  │
│   - Normalized Cascade Constraints    │
└───────────────────────────────────────┘
```

---

## 2. Key Features in v1.0.0

* **Workout & Routine Management:** Create, view, navigate, and delete custom workout routines with cascading relational cleanup.
* **Global Exercise Catalog & Autocomplete:** When adding exercises to any workout, an inline typeahead dropdown queries all catalogued exercises across the database, preventing redundant duplicates and standardizing exercise naming conventions.
* **Persistent Exercise Notes:** Exercise-specific notes and training cues persist directly to PostgreSQL and synchronize on page load.
* **Dynamic Set Logging (`jsonb` Engine):** Sets are logged using dynamic key-value metric maps (`weight`, `reps`) stored in PostgreSQL binary JSON, allowing flexible set structures without requiring database migrations.
* **Automatic Asynchronous Save:** Weight and rep inputs trigger automatic backend persistence on unfocus or completion, eliminating manual save buttons during workout sessions.
* **Full CRUD on All Entities:** Complete lifecycle management for Workouts, Exercises, Sets/Logs, and Notes.
* **Zero Local Dependencies:** 100% decoupled from local SQLite storage; operational on cellular data (LTE/5G) and Wi-Fi anywhere in the world.

---

## 3. Technology Stack

### Mobile Client
* **Framework:** .NET MAUI (.NET 9)
* **Language:** C# 13 / XAML
* **Networking:** Reusable `HttpClient` managed through `NetworkService` and `ApiService`
* **Target Platforms:** Android, iOS, macOS, Windows

### Backend API
* **Framework:** ASP.NET Core Web API (.NET 8)
* **Language:** C# 12
* **ORM:** Entity Framework Core 8 via `Npgsql.EntityFrameworkCore.PostgreSQL`
* **Serialization:** Dynamic JSON with `NewtonsoftJson` & `System.Text.Json`
* **Containerization:** Multi-stage `Dockerfile` (`mcr.microsoft.com/dotnet/sdk:8.0` and `aspnet:8.0`)
* **Host:** Render Web Services (Container Engine)

### Database
* **Engine:** PostgreSQL 16
* **Provider:** Neon Serverless PostgreSQL
* **Specialized Features:** Native `jsonb` indexing and storage for set metrics

---

## 4. Database Schema

```
┌─────────────────────────┐
│        Workouts         │
├─────────────────────────┤
│ Id: int (PK, Identity)  │
│ Name: text              │
└────────────┬────────────┘
             │ 1
             │
             │ *
┌────────────┴────────────┐            ┌─────────────────────────┐
│    WorkoutExercises     │            │        Exercises        │
├─────────────────────────┤            ├─────────────────────────┤
│ Id: int (PK, Identity)  │       *    │ Id: int (PK, Identity)  │
│ WorkoutId: int (FK)     ├───────────►│ Name: text              │
│ ExerciseId: int (FK)    │            │ Note: text (nullable)   │
│ OrderIndex: int         │            └─────────────────────────┘
│ RestTimeSeconds: int?   │
└────────────┬────────────┘
             │ 1
             │
             │ *
┌────────────┴────────────┐
│      ExerciseLogs       │
├─────────────────────────┤
│ Id: int (PK, Identity)  │
│ WorkoutExerciseId: (FK) │
│ SetNumber: int          │
│ CompletedAt: timestamptz│
│ Metrics: jsonb          │◄─── {"weight": 225.0, "reps": 8.0}
└─────────────────────────┘
```

---

## 5. Core API Endpoints

### Workouts
| Method | Route | Description |
| :--- | :--- | :--- |
| `GET` | `/api/workouts` | Retrieve all workouts |
| `GET` | `/api/workouts/{id}` | Get workout by ID |
| `POST` | `/api/workouts` | Create a new workout |
| `PUT` | `/api/workouts/{id}` | Update workout details |
| `DELETE` | `/api/workouts/{id}` | Delete a workout and all linked exercise records |

### Exercises
| Method | Route | Description |
| :--- | :--- | :--- |
| `GET` | `/api/exercises` | Global list of all exercises (used for autocomplete) |
| `GET` | `/api/workouts/{workoutId}/exercises` | List exercises attached to a specific workout |
| `GET` | `/api/workouts/{workoutId}/exercises/{id}` | Get exercise details and notes |
| `POST` | `/api/workouts/{workoutId}/exercises` | Link an exercise to a workout (creates record if new) |
| `PUT` | `/api/workouts/{workoutId}/exercises/{id}` | Update exercise metadata and notes |
| `DELETE`| `/api/workouts/{workoutId}/exercises/{id}` | Unlink an exercise from a workout |

### Exercise Logs / Sets
| Method | Route | Description |
| :--- | :--- | :--- |
| `GET` | `/api/workouts/{wId}/exercises/{eId}/logs` | Fetch all logged sets for an exercise |
| `POST` | `/api/workouts/{wId}/exercises/{eId}/logs` | Create a new set with dynamic JSONB metrics |
| `PUT` | `/api/workouts/{wId}/exercises/{eId}/logs/{id}` | Update set metrics and completion state |
| `DELETE`| `/api/workouts/{wId}/exercises/{eId}/logs/{id}` | Delete a logged set |

---

## 6. Local Development Setup

### Prerequisites
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) & [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [Visual Studio 2022](https://visualstudio.microsoft.com/) with **.NET Multi-platform App UI development** and **ASP.NET and web development** workloads
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) or a local PostgreSQL instance (e.g., port 5432)
* [DBeaver Community Edition](https://dbeaver.io/)

### 1. Backend Setup (`ZenithAPI`)
```bash
# Clone the repository
git clone https://github.com/aaron-cherry/ZenithAPI.git
cd ZenithAPI/ZenithAPI

# Restore dependencies
dotnet restore

# Apply migrations to your local or cloud PostgreSQL database
dotnet ef database update

# Run the API locally
dotnet run
```
The local API documentation will be available at `http://localhost:5163/swagger`.

### 2. Client Setup (`Zenith`)
```bash
# Clone the mobile repository
git clone https://github.com/aaron-cherry/Zenith.git
cd Zenith

# Configure endpoint in Zenith/Services/NetworkService.cs
# Local LAN: "http://<YOUR_LOCAL_IP>:5163/"
# Production: "https://zenithapi-eonb.onrender.com/"

# Build and deploy to an attached Android device
dotnet build WorkoutApp.csproj -t:Run -f net9.0-android
```

---

## 7. Cloud Deployment Reference

### Backend Docker Container (Render)
The API runs as a containerized web service on Render:
* **Base Image:** `mcr.microsoft.com/dotnet/aspnet:8.0`
* **Internal Port:** `8080`
* **Public Domain:** `https://zenithapi-eonb.onrender.com`
* **Environment Variable:** `ConnectionStrings__ZenithDb` configured with Neon connection URI.

### Database (Neon)
* **SSL Mode:** `require`
* **Serverless Scale-to-Zero:** Active during periods of inactivity.

---

## 8. Release Roadmap (v1.1.0+)

- [ ] **Custom Modality Components:** Specialized UI layouts for Myo-Reps, Interval/HIIT, and Cardio (distance/duration) taking advantage of the existing `jsonb` storage layer.
- [ ] **Interactive Progression Graphs:** Visual progression charts tracking estimated 1RM and volume load over time per exercise.
- [ ] **PR Tracking:** Automatic personal record detection and celebration tags.
- [ ] **Workout Reordering:** Drag-and-drop reordering for workouts and exercises within a session.

# AvansedFood - Food Waste Reduction Platform

Een ASP.NET Core MVC applicatie waarmee studenten overtollige maaltijdpakketten van Avans kantines kunnen reserveren om voedselverspilling tegen te gaan.

## Projectinformatie

- **Student:** Thomas Quartel
- **Studentnummer:** 2187613
- **Datum:** Februari 2026
- **Vak:** Server-Side Web Programming

## Projectomschrijving

AvansedFood is een webapplicatie die studenten helpt om overtollige maaltijdpakketten van Avans kantines te reserveren. Dit vermindert voedselverspilling en biedt studenten voordelige maaltijden. De applicatie is geïnspireerd op Too Good To Go en biedt functionaliteit voor zowel studenten als kantinemedewerkers.

### Belangrijkste Features

**Voor Studenten:**
- Bekijken van beschikbare maaltijdpakketten
- Filteren op stad en type maaltijd
- Reserveren van pakketten
- Overzicht van eigen reserveringen
- Leeftijdsverificatie voor 18+ pakketten (alcohol)

**Voor Kantinemedewerkers:**
- Beheer van eigen kantine pakketten
- Overzicht van alle kantines (read-only)
- Aanmaken, bewerken en verwijderen van pakketten
- Automatische validatie van business rules

## Architectuur

### Technology Stack

- **Framework:** ASP.NET Core 8.0 MVC
- **Database:** SQL Server met Entity Framework Core 8.0
- **ORM:** Entity Framework Core (Code-First)
- **Authentication:** ASP.NET Core Identity
- **Testing:** xUnit, Moq, FluentAssertions
- **API:** REST (RMM Level 2) + GraphQL
- **Documentation:** Swagger/OpenAPI, Banana Cake Pop
- **CI/CD:** GitHub Actions

### Architectuur Pattern

Het project volgt **Onion Architecture** met 4 lagen:
```
┌─────────────────────────────────────┐
│         Web (Presentation)          │  ← Controllers, Views, ViewModels
├─────────────────────────────────────┤
│       Application (Services)        │  ← Business Logic, Use Cases
├─────────────────────────────────────┤
│    Infrastructure (Data Access)     │  ← Repositories, EF Core, DbContext
├─────────────────────────────────────┤
│      Domain (Core/Entities)         │  ← Models, Interfaces, Enums
└─────────────────────────────────────┘
```

##  Database Schema

### Tabellen

**Students**
- StudentId (PK)
- Name, Email, StudentNumber
- BirthDate (voor 18+ validatie)
- StudyCity, PhoneNumber
- NoShowCount

**Canteens**
- CanteenId (PK)
- City (Breda/Tilburg/Den Bosch)
- Location (LA/LD/HA/TA)
- OffersHotMeals (boolean)

**Packages**
- PackageId (PK)
- Name, Price
- City, MealType
- PickupTime, ExpirationTime
- Is18Plus (berekend op basis van producten)
- CanteenId (FK), ReservedByStudentId (FK nullable)

**Products**
- ProductId (PK)
- Name
- ContainsAlcohol (boolean)
- PhotoUrl

**PackageProducts** (Many-to-Many)
- PackageId (FK)
- ProductId (FK)

**Reservations**
- ReservationId (PK)
- PackageId (FK)
- StudentId (FK)
- CreatedAt, IsPickedUp

## 🚀 Installatie & Setup

### Vereisten

- **.NET 8.0 SDK** - https://dotnet.microsoft.com/download
- **SQL Server** (LocalDB of volledige installatie)
- **Visual Studio 2022** of **JetBrains Rider**
- **Git** (voor CI/CD)

### Stap-voor-stap Installatie

#### 1. Clone Repository
```bash
git clone https://github.com/[jouw-username]/AvansedFood.git
cd AvansedFood
```

#### 2. Update Connection Strings

Open `AvansedFood.Web/appsettings.json` en pas aan indien nodig:
```json
{
  "ConnectionStrings": {
    "DomainConnection": "Server=(localdb)\\mssqllocaldb;Database=AvansedFoodDb;Trusted_Connection=True;",
    "IdentityConnection": "Server=(localdb)\\mssqllocaldb;Database=AvansedFoodIdentityDb;Trusted_Connection=True;"
  }
}
```

Voor SQL Server (niet LocalDB):
```json
"DomainConnection": "Server=localhost;Database=AvansedFoodDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

#### 3. Restore Dependencies
```bash
dotnet restore
```

#### 4. Run Database Migrations

**Optie A: Via Package Manager Console (Visual Studio)**
```powershell
# Domain database
Update-Database -Context AppDbContext

# Identity database  
Update-Database -Context IdentityDbContext
```

**Optie B: Via CLI**
```bash
# Domain database
dotnet ef database update --project Infrastructure --startup-project AvansedFood.Web --context AppDbContext

# Identity database
dotnet ef database update --project AvansedFood.Web --startup-project AvansedFood.Web --context IdentityDbContext
```

#### 5. Run de Applicatie

**Via Visual Studio:**
- Open `AvansedFood.sln`
- Druk op **F5** of klik op **Start**

**Via CLI:**
```bash
cd AvansedFood.Web
dotnet run
```

#### 6. Open in Browser

Navigeer naar: `https://localhost:7275`

## Test Accounts

De applicatie wordt automatisch gevuld met test data bij de eerste run.

### Student Accounts

| Email | Password | Leeftijd | Beschrijving |
|-------|----------|----------|--------------|
| student@avans.nl | Avans123! | 26 jaar | Volwassen student (kan alles reserveren) |
| mark.bakker@avans.nl | Avans123! | 16 jaar | Minderjarige (kan geen 18+ pakketten) |

### Employee Account

| Email | Password | Kantine |
|-------|----------|---------|
| employee@avans.nl | Avans123! | Breda - LA |

## Testing

### Unit Tests

Het project bevat **23 unit tests** met **95%+ coverage** op de business logic laag.

**Test Overzicht:**
- **PackageServiceTests** (10 tests) - Reservering business rules
- **StudentServiceTests** (5 tests) - Validaties en registratie
- **CanteenServiceTests** (8 tests) - Kantine operaties

**Tests Uitvoeren:**

**Via Visual Studio:**
- Open **Test Explorer** (Test → Test Explorer)
- Klik op **Run All**

  ## 🔌 API Endpoints

### REST API

**Base URL:** `https://localhost:7275/api`

**Swagger Documentatie:** `https://localhost:7275/swagger`

#### Endpoints

| Method | Endpoint | Beschrijving | Auth |
|--------|----------|--------------|------|
| GET | `/packages` | Alle beschikbare pakketten | Student |
| GET | `/packages?city=Breda` | Filter op stad | Student |
| GET | `/packages/{id}` | Pakket details | Student |
| POST | `/packages/{id}/reserve` | Reserveer pakket | Student |
| GET | `/packages/my-reservations?studentId={id}` | Mijn reserveringen | Student |

**Voorbeeld Request:**
```bash
curl -X GET "https://localhost:7275/api/packages?city=Breda" -H "accept: application/json"
```

**Voorbeeld Response:**
```json
[
  {
    "id": 1,
    "name": "Brood Lunch Pakket",
    "city": "Breda",
    "mealType": "Brood",
    "canteenLocation": "LA",
    "pickupTime": "2026-02-07T12:00:00",
    "expirationTime": "2026-02-07T14:00:00",
    "price": 3.50,
    "is18Plus": false,
    "products": [
      {
        "id": 1,
        "name": "Broodje Kaas",
        "containsAlcohol": false
      }
    ]
  }
]
```

### GraphQL API

**Endpoint:** `https://localhost:7275/graphql`

**Banana Cake Pop UI:** Open de endpoint in je browser voor interactieve documentatie

#### Voorbeeld Queries

**Alle pakketten:**
```graphql
query {
  packages {
    id
    name
    price
    city
    mealType
    canteen {
      location
    }
  }
}
```

**Pakket details:**
```graphql
query {
  package(id: 1) {
    name
    price
    is18Plus
    canteen {
      city
      location
    }
    products {
      name
      containsAlcohol
    }
  }
}
```

**Filter op stad:**
```graphql
query {
  packagesByCity(city: BREDA) {
    name
    price
    pickupTime
  }
}
```

## Business Rules

### Geïmplementeerde User Stories

**US_01:** Als student wil ik kunnen zien welke pakketten er aangeboden worden
-  Overzicht met beschikbare pakketten
-  Filter op stad en maaltijdtype
-  Toon 18+ indicator

**US_02:** Als kantinemedewerker wil ik het aanbod kunnen bekijken
-  Overzicht eigen kantine
-  Overzicht alle kantines (read-only)

**US_03:** Als kantinemedewerker wil ik een pakket kunnen aanbieden
-  CRUD operaties op pakketten
-  Gereserveerde pakketten NIET bewerken/verwijderen
-  Max 2 dagen van tevoren aanbieden

**US_04:** Als student wil ik geen alcohol pakketten kunnen reserveren als ik <18 ben
-  Geboortedatum check
-  Automatische 18+ markering op basis van producten

**US_05:** Als student wil ik een pakket kunnen reserveren
-  Reserveer functionaliteit
-  Max 1 pakket per dag regel

**US_06:** Als student wil ik kunnen zien wat er in een pakket zit
-  Details pagina met product lijst
-  Alcohol indicator per product

**US_07:** Als student wil ik niet twee keer hetzelfde pakket kunnen reserveren
-  Check of pakket al gereserveerd is
-  Pakket verdwijnt uit lijst na reservering

**US_08:** Als student wil ik kunnen filteren op stad en type maaltijd
-  Dropdown filters op pakket overzicht pagina
-  Filter op City (Breda, Tilburg, Den Bosch)
-  Filter op MealType (Brood, Warme Avondmaaltijd, Drank)
-  Combinatie van filters mogelijk

**US_09:** Als kantinemedewerker wil ik alleen warme maaltijden aanbieden als mijn kantine dit ondersteunt
-  Kantine OffersHotMeals property
-  Validatie bij aanmaken warme maaltijd pakket
-  

## CI/CD Pipeline

Het project maakt gebruik van **GitHub Actions** voor automatische builds en tests.

**Workflow:** `.github/workflows/dotnet-ci.yml`

### Pipeline Stappen

1. **Checkout** - Code ophalen
2. **Setup .NET** - .NET 8.0 SDK installeren
3. **Restore** - Dependencies herstellen
4. **Build** - Applicatie builden (Release mode)
5. **Test** - Alle 23 unit tests uitvoeren
6. **Migrations Check** - EF Core migrations verifiëren

Here's the updated `README.md` with a new section explaining how to enable and use **Swagger** in the **AlaskaTicketManagement API**:

---

```markdown
# AlaskaTicketManagement API

An ASP.NET Core Web API for managing concert ticket reservations, purchases, and venue/event details. This project supports reservation windows, ticket availability checks, and integrates with an external payment processing system (assumed).

---

## Features

- Create and manage **events** and **venues**
- **Reserve**, **purchase**, and **cancel** tickets
- View ticket **availability** per event
- Built-in Swagger UI for API testing and documentation

---

## Getting Started

### Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download)
- IDE: [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- It uses entity framework in-memory database

### Running the Project

```bash
git clone https://github.com/yourusername/AlaskaTicketManagement.git
cd AlaskaTicketManagement
dotnet restore
dotnet run
```

API will be available at:

- `https://localhost:5001`
- `http://localhost:5000`

---

## Using Swagger

Swagger is enabled by default for testing and exploring endpoints.

Once the app is running, navigate to:

```
https://localhost:5001/swagger
```

From there, you can:

- View all available endpoints
- Execute API requests interactively
- Inspect request/response formats
- Review status codes and example schemas

---

## API Endpoints

### Venues

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/v1/venues` | Create a new venue |
| `GET`  | `/api/v1/venues` | Get all venues |
| `GET`  | `/api/v1/venues/{venueId}` | Get a specific venue |
| `PUT`  | `/api/v1/venues/{venueId}` | Update a venue |

### Events

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/v1/events` | Create a new event |
| `GET`  | `/api/v1/events/{eventId}` | Get event details |
| `PUT`  | `/api/v1/events/{eventId}` | Update an event |

### Tickets

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/v1/tickets/reserve` | Reserve tickets |
| `POST` | `/api/v1/tickets/purchase` | Purchase tickets |
| `POST` | `/api/v1/tickets/cancel` | Cancel reservation |
| `GET`  | `/api/v1/tickets/availability/{eventId}` | Get ticket availability for an event |

---

## 🔧 Sample Request (Reserve Ticket)

```json
POST /api/v1/tickets/reserve
{
  "eventId": 1,
  "ticketType": "VIP",
  "quantity": 2,
  "userEmail": "user@example.com"
}
```

---

---

## Contact

- **Maintainer:** Ruth Ogunnaike
- **LinkedIn:** https://linkedin.com/in/ruthogunnaike

---
# Example.Cqrs

Standalone Clean Architecture sample that demonstrates a practical reading of CQRS:

- a **command** mutates state and may return information about the mutation
- a **query** only reads data
- both sides use the **same SQLite tables** (simple CQS-style CQRS from the blog post, not full read-model separation)

## Solution layout

```text
Example.Cqrs.slnx
├── src/
│   ├── Example.Cqrs.Domain          # entities + repository ports
│   ├── Example.Cqrs.Application     # MediatR commands/queries + FluentValidation
│   ├── Example.Cqrs.Infrastructure  # EF Core SQLite + repositories + seed
│   └── Example.Cqrs.Api             # Minimal APIs + Swagger + ProblemDetails
└── tests/
    ├── Example.Cqrs.Domain.UnitTests        # domain rules (Order, Product)
    └── Example.Cqrs.Application.UnitTests     # handlers + FluentValidation
```
Dependency direction:

`Api -> Infrastructure`  
`Infrastructure -> Application` (and Domain transitively)  
`Application -> Domain`

No shared platform libraries. Only NuGet packages: MediatR, FluentValidation, EF Core SQLite, Swashbuckle.

## Why the command returns data

`POST /api/orders` creates an order and returns:

- `id` – needed for `Location` / follow-up GET
- `totalPrice` – derived during the write
- `expectedDeliveryAt` – also computed during the write

That keeps the client from issuing an extra round-trip just to learn what the command already knows.

`GET /api/orders/{id}` is a pure query: no tracking writes, no side effects.

## Run

```bash
dotnet restore
dotnet run --project src/Example.Cqrs.Api
```

Swagger UI: `http://localhost:5140/swagger`

HTTP file: [`Example.Cqrs.http`](Example.Cqrs.http)

Seeded product ids:

| Product | Id |
|---------|----|
| Notebook | `11111111-1111-1111-1111-111111111111` |
| Mechanical Keyboard | `22222222-2222-2222-2222-222222222222` |
| Wireless Mouse | `33333333-3333-3333-3333-333333333333` |

Example:

```bash
curl -X POST http://localhost:5140/api/orders \
  -H "Content-Type: application/json" \
  -d "{\"productId\":\"11111111-1111-1111-1111-111111111111\",\"quantity\":2}"
```

## Design notes

1. Domain owns order creation rules (`Order.Create` computes total and delivery date).
2. Handlers stay thin: load → domain method → save → map response.
3. Application uses a small local `Result<T>`.
4. Same database tables for commands and queries on purpose — this is the simple variant that is often enough.
5. Full CQRS with a separate read model, outbox, and message broker is intentionally out of scope.

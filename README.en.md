<div align="center">

<img src="docs/screenshots/vohumanah-logo.png" width="260" alt="VohuManah" />

# VohuManah

Digital Library Management System

</div>

---

## About

**VohuManah** is a unified system for libraries, archives and documentation centers. It brings books, manuscripts and publications together in one place — from recording bibliographic details and preparing digital copies to searching, statistics and reporting.

### Main sections

- **Books** — bibliographic data, volumes, attachments, cover images and digital file archiving
- **Manuscripts** — managing handwritten manuscripts, place of writing, script gap and attributes of historical documents
- **Publications** — periodicals and journals with type, place of printing and language

Supporting tools are also built in, and the whole system database can be backed up easily.

For copyright protection, the details of each book and its physical copies are recorded. Only notes, article attachments and other attachments are appended to a book, with no restriction on that. Please respect this clause to protect the rights of publishers and authors.

---

## Screenshots

<div align="center">

![Dashboard](docs/screenshots/dashboard.png)

![Books](docs/screenshots/books.png)

</div>

---

## Key features

- **Advanced search** — search across a combination of criteria: title, subject, language, publisher, city and publication year
- **Digital book attachments** — upload and keep multiple attachments per book in different formats (PDF, Word, Excel, images)
- **Barcode & QR** — scan a barcode or QR code to find and open a book quickly
- **Reporting** — export tables and statistics to PDF and Excel
- **Interactive charts** — visual statistics across books, manuscripts and publications
- **Automatic, graphical backup** — back up your data on a schedule through a simple UI
- **Access control** — roles and permission levels for users across sections

---

## Architecture

The project is built on **Clean Architecture** with the **CQRS** pattern. The layers are clearly separated:

- **Domain** — entities and domain rules
- **Application** — business logic, commands, queries and validation (FluentValidation)
- **Infrastructure** — data access (Entity Framework Core), JWT authentication, file storage and the reporting engine
- **Web.Api** — the REST API
- **Web.UI** — the React UI

```
┌──────────────────────────────────────────────────────────────┐
│                        Web.UI (React)                        │
└──────────────────────────────┬───────────────────────────────┘
                               │ REST / JSON
┌──────────────────────────────▼───────────────────────────────┐
│                    Web.Api (REST API)                         │
├──────────────────────────────────────────────────────────────┤
│   Application (CQRS: Command/Query/Handler/Validator)         │
├──────────────────────────────────────────────────────────────┤
│   Domain (Entities / Rules / Errors)                          │
├──────────────────────────────────────────────────────────────┤
│   Infrastructure (EF Core / JWT / File Storage / Reports)     │
└──────────────────────────────────────────────────────────────┘
                               │
                    ┌──────────▼──────────┐
                    │  SQL Server (DB)    │
                    └─────────────────────┘
```

---

## Technology stack

| Area | Technology |
|---|---|
| Language & platform | C# / .NET 10 |
| Architecture pattern | Clean Architecture + CQRS |
| Data access | Entity Framework Core + SQL Server |
| Authentication | JWT |
| Validation | FluentValidation |
| Frontend | React 19 + TypeScript 6 |
| Component library | Mantine 9 |
| Charts | Recharts |
| Reporting | RDLC |
| Containerization | Docker Compose |

---

## Quick start

The project runs with Docker Compose:

```bash
docker compose up -d --build
```

---

## Project structure

```
VohuManah.Core/
├── src/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   ├── SharedKernel/
│   ├── External/
│   ├── Web.Api/
│   └── Web.UI/
├── docker/
├── docs/
└── VohuManah.slnx
```

---

## License

Distributed under the **GPL-3.0** license. See `COPYING` for details.

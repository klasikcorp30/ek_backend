# Ekklesia Church Directory API

A comprehensive .NET 9 Web API backend for managing church directories and registrations.

## Features

### Functional Requirements Implemented

- **FR-1: Add Church** - Admins can create church records with address, contact info, denomination, and coordinates
- **FR-2: Update/Delete Church** - Admins can update church details or deactivate churches
- **FR-3: View Church** - Public users can retrieve detailed church information including service schedules
- **FR-4: Church Verification** - Churches have status: Pending, Verified, or Rejected to ensure data quality
- **FR-5: CSV Import** - Data curators can upload CSV files to bulk import or update church data

### Core Features

- **Church Management**: Full CRUD operations for church records
- **User Authentication**: JWT-based authentication and authorization
- **Role-based Access Control**: Admin, DataCurator, and User roles
- **CSV Import**: Bulk import functionality for church data
- **Search & Filtering**: Text search and location-based filtering
- **Church Verification**: Status management for data quality
- **RESTful API**: Clean REST endpoints with proper HTTP status codes
- **API Documentation**: Swagger/OpenAPI integration with JWT auth

## Technology Stack

- **.NET 9** - Latest .NET framework
- **Entity Framework Core** - ORM for database operations
- **SQLite** - Default database for development
- **MySQL** - Production database support (Pomelo provider)
- **JWT Bearer Authentication** - Secure token-based auth
- **BCrypt** - Password hashing
- **CsvHelper** - CSV file processing
- **Swagger/OpenAPI** - API documentation
- **CORS** - Cross-origin resource sharing

## Quick Start

### Prerequisites

- .NET 9 SDK
- Database: SQLite (default) or MySQL Server

### Database Setup

#### Option 1: SQLite (Default - No setup required)
The application uses SQLite by default for development. No additional setup needed.

#### Option 2: MySQL Setup
1. **Install MySQL Server**
2. **Run setup script**: `Scripts/mysql-setup.sql`
3. **Update configuration**: Set `"UseMySQL": true` in `appsettings.json`
4. **Update connection string** with your MySQL credentials

See `Docs/MySQL-Setup.md` for detailed MySQL setup instructions.

### Setup

1. **Clone and navigate to the project**
   ```bash
   cd /path/to/project
   ```

2. **Configure database**

   **For SQLite (Default)**:
   No configuration needed - database file will be created automatically.

   **For MySQL**:
   - Set up MySQL database (see `Docs/MySQL-Setup.md`)
   - Update `appsettings.json`:
   ```json
   {
     "UseMySQL": true,
     "ConnectionStrings": {
       "MySqlConnection": "Server=localhost;Port=3306;Database=ekklesia_db;Uid=ekklesia_user;Pwd=your_password;"
     }
   }
   ```

3. **Configure JWT settings**
   Update JWT settings in `appsettings.json`:
   ```json
   {
     "Jwt": {
       "Key": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!",
       "Issuer": "Ekklesia.Api",
       "Audience": "Ekklesia.Client"
     }
   }
   ```

4. **Run the application**
   ```bash
   dotnet run --project Ekklesia.Api.csproj
   ```

5. **Access the API**
   - API Base URL: `https://localhost:7071`
   - Swagger UI: `https://localhost:7071` (in development)

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/auth/profile` - Get user profile (authenticated)
- `POST /api/auth/change-password` - Change password (authenticated)
- `GET /api/auth/users` - Get all users (Admin only)
- `PATCH /api/auth/users/{id}/role` - Update user role (Admin only)
- `DELETE /api/auth/users/{id}` - Deactivate user (Admin only)

### Churches
- `GET /api/churches` - Get all churches with filtering
- `GET /api/churches/{id}` - Get specific church
- `POST /api/churches` - Create church (Admin only)
- `PUT /api/churches/{id}` - Update church (Admin only)
- `DELETE /api/churches/{id}` - Delete church (Admin only)
- `PATCH /api/churches/{id}/status` - Update church status (Admin only)
- `GET /api/churches/search` - Search churches
- `POST /api/churches/import` - Import churches from CSV (DataCurator/Admin only)

### Health & Monitoring
- `GET /api/health` - Basic health check
- `GET /api/health/database` - Database connectivity and statistics
- `GET /api/health/info` - System information

## User Roles

- **User**: Can view verified churches and search
- **DataCurator**: Can import CSV data + User permissions
- **Admin**: Full access to all operations

## Default Admin Account

The system creates a default admin account:
- **Email**: admin@ekklesia.com
- **Password**: Admin123!

⚠️ **Important**: Change this password in production!

## Database Schema

### Church Entity
- Id, Name, Address, City, State, ZipCode
- Phone, Email, Website, Denomination
- Latitude, Longitude (for location services)
- Status (Pending, Verified, Rejected)
- Description, ServiceSchedule (JSON)
- Audit fields (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)

### User Entity
- Id, Email, FirstName, LastName
- PasswordHash, Role, IsActive
- Audit fields (CreatedAt, UpdatedAt, LastLoginAt)

## CSV Import Format

The CSV import expects these columns:
- Name, Address, City, State, ZipCode
- Phone, Email, Website, Denomination
- Latitude, Longitude, Description

## Development

### Building
```bash
dotnet build Ekklesia.Api.csproj
```

### Running Tests
```bash
dotnet test
```

### Database Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Security Features

- JWT token-based authentication
- BCrypt password hashing
- Role-based authorization
- CORS configuration
- Input validation and sanitization

## API Response Format

All API responses follow a consistent format:
- Success: HTTP 200/201/204 with data
- Client errors: HTTP 400/401/403/404 with error message
- Server errors: HTTP 500 with generic error message

## License

This project is licensed under the MIT License.
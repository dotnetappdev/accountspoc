# Database Setup Feature

## Overview

The Database Setup feature provides a user-friendly interface for configuring database connections, similar to SQL Server Management Studio. It allows administrators to:

1. Configure database connection parameters
2. Test connections before applying
3. Choose between different database providers (SQL Server, SQLite, PostgreSQL)
4. Optionally seed sample user data with a toggle button

## Accessing the Setup Page

Navigate to `/setup` in your Blazor application to access the Database Setup page.

## Features

### Connection Settings

#### Database Provider Selection
- **SQL Server**: Enterprise-grade relational database
- **SQLite**: Lightweight file-based database (ideal for development)
- **PostgreSQL**: Open-source relational database

#### Server Configuration
- **Server/Host**: Database server address (e.g., localhost, .\SQLEXPRESS, IP address)
- **Port**: Optional port number (SQL Server default: 1433, PostgreSQL: 5432)
- **Database Name**: Name of the database to create/use

#### Authentication Options
- **Windows Authentication / Trusted Connection**: Use Windows credentials
- **Integrated Security**: Use integrated Windows authentication
- **SQL Authentication**: Username and password (when not using Windows Auth)

#### Security Options
- **Encrypt Connection**: Enable SSL/TLS encryption for the connection
- **Trust Server Certificate**: Trust self-signed certificates (useful for development)

### Data Seeding Toggle

**Seed Sample User Data**: When enabled, automatically populates the database with:
- 30 pre-configured users across 14 roles
- 44 granular permissions
- Complete role-permission mappings
- Realistic accounting department structure

This is perfect for:
- Development and testing
- Demo environments
- Quick proof-of-concept setups

### Action Buttons

1. **Build Connection String**: Generates a preview of the connection string based on your settings
2. **Test Connection**: Validates the connection without making changes
3. **Apply Configuration**: Creates the database and optionally seeds data

## API Endpoints

### POST /api/setup/build-connection-string
Builds a connection string from configuration parameters.

**Request Body**:
```json
{
  "server": "localhost",
  "port": "1433",
  "database": "AccountsPOCDb",
  "trustedConnection": true,
  "trustServerCertificate": true,
  "encrypt": false,
  "databaseProvider": "SqlServer",
  "seedUserData": false
}
```

**Response**:
```json
{
  "connectionString": "Server=localhost,1433;Database=AccountsPOCDb;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
}
```

### POST /api/setup/test-connection
Tests a database connection.

**Request Body**:
```json
{
  "connectionString": "Server=localhost;Database=AccountsPOCDb;...",
  "databaseProvider": "SqlServer"
}
```

**Response**:
```json
{
  "success": true,
  "message": "Connection successful",
  "serverVersion": "Microsoft SQL Server 2022"
}
```

### POST /api/setup/apply-configuration
Applies the configuration, creates the database, and optionally seeds data.

**Request Body**: Same as build-connection-string

**Response**:
```json
{
  "success": true,
  "message": "Database configured successfully | Seeded 30 users and 14 roles",
  "connectionString": "Server=localhost;...",
  "usersSeeded": 30,
  "rolesSeeded": 14
}
```

## Connection String Examples

### SQL Server with Windows Authentication
```
Server=localhost;Database=AccountsPOCDb;Integrated Security=true;TrustServerCertificate=true;Encrypt=false
```

### SQL Server with SQL Authentication
```
Server=localhost,1433;Database=AccountsPOCDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true;Encrypt=false
```

### SQL Server with Encryption
```
Server=myserver.database.windows.net;Database=AccountsPOCDb;User Id=admin;Password=Pass123!;Encrypt=true;TrustServerCertificate=false
```

### SQLite
```
Data Source=AccountsPOCDb.db
```

### PostgreSQL
```
Host=localhost;Port=5432;Database=AccountsPOCDb;Username=postgres;Password=mypassword;SSL Mode=Require
```

## Security Considerations

### Production Deployment

1. **Never commit connection strings** with passwords to source control
2. **Use environment variables** or secure vaults for sensitive credentials
3. **Enable encryption** for production databases
4. **Use strong passwords** for SQL authentication
5. **Restrict access** to the setup endpoint after initial configuration

### Development

- The setup page is ideal for development and testing environments
- Consider disabling or protecting the `/setup` route in production
- SQLite is perfect for local development
- Windows Authentication simplifies local SQL Server setup

## Workflow

1. **Navigate** to `/setup`
2. **Select** your database provider
3. **Configure** connection parameters
4. **Toggle** "Seed Sample User Data" if you want pre-configured users
5. **Click** "Build Connection String" to preview
6. **Click** "Test Connection" to verify settings
7. **Click** "Apply Configuration" to create database and seed data
8. **Navigate** to `/login` to start using the application

## Seeded User Data

When "Seed Sample User Data" is enabled, you get:

### Users by Role
- 2 Super Administrators
- 3 Administrators  
- 2 Financial Controllers
- 2 Accounting Managers
- 3 Senior Accountants
- 4 Accountants
- 3 Bookkeepers
- 2 AP Clerks
- 2 AR Clerks
- 1 Payroll Manager
- 2 Auditors
- 2 Support staff
- 2 Field agents

For complete user credentials, see `SEED_DATA_REFERENCE.md`.

## Troubleshooting

### Connection Failed
- Verify server name and port
- Check if database service is running
- Confirm firewall allows connections
- For SQL Server, ensure SQL Server Browser service is running for named instances

### Authentication Failed
- Verify credentials if using SQL authentication
- Ensure Windows user has database permissions if using Windows Auth
- Check if the user has CREATE DATABASE permissions

### Permission Denied
- The user needs permissions to create databases
- Consider using a more privileged account for initial setup
- For SQL Server, the user should be a member of `dbcreator` role

## Future Enhancements

Potential improvements for this feature:
- Support for Azure SQL Database connection strings
- Import/export connection configurations
- Multiple environment profiles (Dev, Staging, Production)
- Connection pooling settings
- Advanced timeout and retry configurations
- Backup and restore database functionality

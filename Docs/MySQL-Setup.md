# MySQL Database Setup Guide

This guide will help you set up MySQL database for the Ekklesia API.

## Prerequisites

1. **Install MySQL Server**
   - **macOS**: `brew install mysql`
   - **Ubuntu**: `sudo apt-get install mysql-server`
   - **Windows**: Download from [MySQL Downloads](https://dev.mysql.com/downloads/mysql/)

2. **Start MySQL Service**
   - **macOS**: `brew services start mysql`
   - **Ubuntu**: `sudo systemctl start mysql`
   - **Windows**: Start MySQL service from Services

## Database Setup

### Method 1: Using MySQL Command Line

1. **Connect to MySQL as root**:
   ```bash
   mysql -u root -p
   ```

2. **Run the setup script**:
   ```sql
   source Scripts/mysql-setup.sql
   ```

3. **Update passwords** in the script before running:
   - Replace `your_password_here` with a secure password

### Method 2: Manual Setup

1. **Create databases**:
   ```sql
   CREATE DATABASE ekklesia_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   CREATE DATABASE ekklesia_dev_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```

2. **Create user**:
   ```sql
   CREATE USER 'ekklesia_user'@'localhost' IDENTIFIED BY 'your_secure_password';
   CREATE USER 'ekklesia_user'@'%' IDENTIFIED BY 'your_secure_password';
   ```

3. **Grant permissions**:
   ```sql
   GRANT ALL PRIVILEGES ON ekklesia_db.* TO 'ekklesia_user'@'localhost';
   GRANT ALL PRIVILEGES ON ekklesia_dev_db.* TO 'ekklesia_user'@'localhost';
   GRANT ALL PRIVILEGES ON ekklesia_db.* TO 'ekklesia_user'@'%';
   GRANT ALL PRIVILEGES ON ekklesia_dev_db.* TO 'ekklesia_user'@'%';
   FLUSH PRIVILEGES;
   ```

## Configuration

### Switch to MySQL

1. **Update connection string** in `appsettings.json`:
   ```json
   {
     "UseMySQL": true,
     "ConnectionStrings": {
       "MySqlConnection": "Server=localhost;Port=3306;Database=ekklesia_db;Uid=ekklesia_user;Pwd=your_secure_password;"
     }
   }
   ```

2. **For development**, update `appsettings.Development.json`:
   ```json
   {
     "UseMySQL": true,
     "ConnectionStrings": {
       "MySqlConnection": "Server=localhost;Port=3306;Database=ekklesia_dev_db;Uid=ekklesia_user;Pwd=your_secure_password;"
     }
   }
   ```

### Environment Variables (Recommended for Production)

Set these environment variables instead of hardcoding passwords:

```bash
export UseMySQL=true
export ConnectionStrings__MySqlConnection="Server=localhost;Port=3306;Database=ekklesia_db;Uid=ekklesia_user;Pwd=your_secure_password;"
```

## Database Migration

### Create Initial Migration

```bash
# Remove SQLite database files if switching
rm ekklesia.db*

# Create migration for MySQL
dotnet ef migrations add InitialMySQLMigration

# Update database
dotnet ef database update
```

### Run Application

```bash
dotnet run --project Ekklesia.Api.csproj
```

## Connection String Parameters

| Parameter | Description | Example |
|-----------|-------------|---------|
| Server | MySQL server hostname | localhost, 192.168.1.100 |
| Port | MySQL port number | 3306 (default) |
| Database | Database name | ekklesia_db |
| Uid | Username | ekklesia_user |
| Pwd | Password | your_secure_password |
| SslMode | SSL connection mode | Required, Preferred, None |
| CharSet | Character set | utf8mb4 |

### Advanced Connection String Example

```
Server=localhost;Port=3306;Database=ekklesia_db;Uid=ekklesia_user;Pwd=your_password;SslMode=Required;CharSet=utf8mb4;AllowUserVariables=true;UseAffectedRows=false;
```

## Production Considerations

1. **Security**:
   - Use strong passwords
   - Enable SSL/TLS
   - Restrict user permissions
   - Use environment variables for secrets

2. **Performance**:
   - Configure connection pooling
   - Set appropriate timeouts
   - Monitor database performance

3. **Backup**:
   - Set up regular database backups
   - Test restore procedures

## Troubleshooting

### Common Issues

1. **Connection refused**:
   - Check if MySQL service is running
   - Verify port 3306 is open
   - Check firewall settings

2. **Authentication failed**:
   - Verify username and password
   - Check user permissions
   - Ensure user can connect from the host

3. **Database not found**:
   - Verify database exists
   - Check database name in connection string

### Useful Commands

```sql
-- Check MySQL version
SELECT VERSION();

-- Show all databases
SHOW DATABASES;

-- Show users
SELECT User, Host FROM mysql.user;

-- Check user privileges
SHOW GRANTS FOR 'ekklesia_user'@'localhost';

-- Show database tables
USE ekklesia_db;
SHOW TABLES;
```

## Docker Setup (Optional)

For development, you can use Docker:

```bash
# Run MySQL in Docker
docker run --name mysql-ekklesia \
  -e MYSQL_ROOT_PASSWORD=rootpassword \
  -e MYSQL_DATABASE=ekklesia_db \
  -e MYSQL_USER=ekklesia_user \
  -e MYSQL_PASSWORD=userpassword \
  -p 3306:3306 \
  -d mysql:8.0

# Connect to Docker MySQL
docker exec -it mysql-ekklesia mysql -u root -p
```

Update connection string for Docker:
```
Server=localhost;Port=3306;Database=ekklesia_db;Uid=ekklesia_user;Pwd=userpassword;
```
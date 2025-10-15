-- MySQL Database Setup Script for Ekklesia API
-- Run these commands in MySQL to set up the database

-- 1. Create database
CREATE DATABASE IF NOT EXISTS ekklesia_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS ekklesia_dev_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 2. Create user (replace 'your_password_here' with a secure password)
CREATE USER IF NOT EXISTS 'ekklesia_user'@'localhost' IDENTIFIED BY 'your_password_here';
CREATE USER IF NOT EXISTS 'ekklesia_user'@'%' IDENTIFIED BY 'your_password_here';

-- 3. Grant privileges
GRANT ALL PRIVILEGES ON ekklesia_db.* TO 'ekklesia_user'@'localhost';
GRANT ALL PRIVILEGES ON ekklesia_dev_db.* TO 'ekklesia_user'@'localhost';
GRANT ALL PRIVILEGES ON ekklesia_db.* TO 'ekklesia_user'@'%';
GRANT ALL PRIVILEGES ON ekklesia_dev_db.* TO 'ekklesia_user'@'%';

-- 4. Flush privileges
FLUSH PRIVILEGES;

-- 5. Verify setup
SHOW DATABASES;
SELECT User, Host FROM mysql.user WHERE User = 'ekklesia_user';

-- Optional: Create a production user with limited privileges
-- CREATE USER IF NOT EXISTS 'ekklesia_app'@'%' IDENTIFIED BY 'production_password_here';
-- GRANT SELECT, INSERT, UPDATE, DELETE ON ekklesia_db.* TO 'ekklesia_app'@'%';
-- FLUSH PRIVILEGES;
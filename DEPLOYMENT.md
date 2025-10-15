# Google Cloud Deployment Guide

This guide explains how to deploy the Ekklesia API to Google Cloud Platform.

## Deployment Options

### Option 1: Cloud Run (Recommended)

1. **Enable required APIs:**
   ```bash
   gcloud services enable cloudbuild.googleapis.com
   gcloud services enable run.googleapis.com
   gcloud services enable container.googleapis.com
   ```

2. **Deploy using Cloud Build:**
   ```bash
   gcloud builds submit --config cloudbuild.yaml
   ```

3. **Set environment variables in Cloud Run:**
   - `MYSQL_CONNECTION_STRING`: Your Cloud SQL connection string
   - `JWT_SECRET_KEY`: Your JWT secret key (generate a secure 32+ character string)

### Option 2: App Engine Flexible

1. **Deploy to App Engine:**
   ```bash
   gcloud app deploy app.yaml
   ```

2. **Set environment variables:**
   Update the `env_variables` section in `app.yaml` with your actual values.

## Environment Variables Required

- `MYSQL_CONNECTION_STRING`: Format: `Server=YOUR_CLOUD_SQL_IP;Port=3306;Database=ekklesia_db;Uid=ekklesia_user;Pwd=YOUR_PASSWORD;`
- `JWT_SECRET_KEY`: A secure random string (32+ characters)

## Database Setup

1. **Create Cloud SQL MySQL instance:**
   ```bash
   gcloud sql instances create ekklesia-db \
     --database-version=MYSQL_8_0 \
     --tier=db-n1-standard-1 \
     --region=us-central1
   ```

2. **Create database and user:**
   ```bash
   gcloud sql databases create ekklesia_db --instance=ekklesia-db
   gcloud sql users create ekklesia_user \
     --instance=ekklesia-db \
     --password=YOUR_SECURE_PASSWORD
   ```

3. **Allow Cloud Run/App Engine access:**
   ```bash
   gcloud sql instances patch ekklesia-db \
     --authorized-networks=0.0.0.0/0
   ```

## Troubleshooting

If you encounter buildpack errors (status code 51), use the Dockerfile approach by ensuring:

1. The `cloudbuild.yaml` uses Docker builder instead of buildpacks
2. The `Dockerfile` is properly configured for .NET 8
3. All necessary files are included and unnecessary files are in `.dockerignore`

## Production Configuration

The application uses `appsettings.Production.json` for production settings. Environment variables will override the placeholder values in this file.
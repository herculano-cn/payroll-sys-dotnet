# Docker Deployment Guide

This guide explains how to run the Payroll Management System using Docker.

## Prerequisites

- Docker Engine 20.10+
- Docker Compose 2.0+
- 2GB RAM minimum
- 5GB disk space

## Quick Start

### 1. Build and Run

```bash
# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Check status
docker-compose ps
```

### 2. Access the Application

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger

### 3. Stop the Application

```bash
# Stop services
docker-compose down

# Stop and remove volumes (WARNING: deletes database)
docker-compose down -v
```

## Architecture

```
┌─────────────────┐
│   Frontend      │
│  (React + Nginx)│
│   Port: 3000    │
└────────┬────────┘
         │
         │ HTTP
         │
┌────────▼────────┐
│   Backend API   │
│   (.NET 9)      │
│   Port: 5000    │
└────────┬────────┘
         │
         │ SQLite
         │
┌────────▼────────┐
│   Database      │
│  (SQLite File)  │
│   Volume        │
└─────────────────┘
```

## Services

### Backend (payroll-api)
- **Image**: Built from `backend/Dockerfile`
- **Port**: 5000 → 8080 (container)
- **Health Check**: `http://localhost:8080/health`
- **Volume**: `payroll-data:/app/data` (SQLite database)

### Frontend (payroll-frontend)
- **Image**: Built from `frontend/Dockerfile`
- **Port**: 3000 → 80 (container)
- **Health Check**: `http://localhost/health`
- **Nginx**: Serves static files and proxies API requests

## Configuration

### Environment Variables

Create a `.env` file (optional):

```bash
# Copy example
cp .env.example .env

# Edit as needed
nano .env
```

### Custom Ports

Edit `docker-compose.yml`:

```yaml
services:
  backend:
    ports:
      - "YOUR_PORT:8080"  # Change 5000 to your port
  
  frontend:
    ports:
      - "YOUR_PORT:80"    # Change 3000 to your port
```

## Development Mode

For development with hot-reload:

```bash
# Backend only
cd backend
dotnet run --project Payroll.API

# Frontend only
cd frontend
npm run dev
```

## Production Deployment

### 1. Build Optimized Images

```bash
# Build with no cache
docker-compose build --no-cache

# Tag for registry
docker tag payroll-api:latest your-registry/payroll-api:v1.0.0
docker tag payroll-frontend:latest your-registry/payroll-frontend:v1.0.0
```

### 2. Push to Registry

```bash
docker push your-registry/payroll-api:v1.0.0
docker push your-registry/payroll-frontend:v1.0.0
```

### 3. Deploy to Server

```bash
# On production server
docker-compose -f docker-compose.prod.yml up -d
```

## Volumes

### Database Persistence

The SQLite database is stored in a Docker volume:

```bash
# Backup database
docker run --rm -v payroll-data:/data -v $(pwd):/backup alpine \
  tar czf /backup/payroll-backup-$(date +%Y%m%d).tar.gz -C /data .

# Restore database
docker run --rm -v payroll-data:/data -v $(pwd):/backup alpine \
  tar xzf /backup/payroll-backup-YYYYMMDD.tar.gz -C /data
```

### Inspect Volume

```bash
# List volumes
docker volume ls

# Inspect volume
docker volume inspect payroll-data

# Remove volume (WARNING: deletes data)
docker volume rm payroll-data
```

## Networking

Services communicate via `payroll-network`:

```bash
# Inspect network
docker network inspect payroll-network

# Test connectivity
docker exec payroll-frontend ping backend
```

## Troubleshooting

### Container Won't Start

```bash
# Check logs
docker-compose logs backend
docker-compose logs frontend

# Check container status
docker-compose ps

# Restart specific service
docker-compose restart backend
```

### Database Issues

```bash
# Access backend container
docker exec -it payroll-api sh

# Check database file
ls -lh /app/data/

# View database
sqlite3 /app/data/payroll.db ".tables"
```

### Network Issues

```bash
# Test backend from frontend
docker exec payroll-frontend curl http://backend:8080/health

# Test from host
curl http://localhost:5000/health
```

### Port Conflicts

```bash
# Check what's using the port
lsof -i :3000
lsof -i :5000

# Kill process
kill -9 <PID>
```

## Health Checks

Both services have health checks:

```bash
# Check backend health
curl http://localhost:5000/health

# Check frontend health
curl http://localhost:3000/health

# View health status
docker-compose ps
```

## Performance Tuning

### Resource Limits

Add to `docker-compose.yml`:

```yaml
services:
  backend:
    deploy:
      resources:
        limits:
          cpus: '1.0'
          memory: 512M
        reservations:
          cpus: '0.5'
          memory: 256M
```

### Nginx Caching

Already configured in `frontend/nginx.conf`:
- Static assets cached for 1 year
- Gzip compression enabled
- Security headers added

## Monitoring

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f backend

# Last 100 lines
docker-compose logs --tail=100 backend
```

### Resource Usage

```bash
# Real-time stats
docker stats

# Specific container
docker stats payroll-api
```

## Security

### Best Practices

1. **Don't expose ports unnecessarily**
   - Only expose what's needed
   - Use reverse proxy in production

2. **Use secrets for sensitive data**
   ```yaml
   secrets:
     db_password:
       file: ./secrets/db_password.txt
   ```

3. **Run as non-root user**
   - Already configured in Dockerfiles

4. **Keep images updated**
   ```bash
   docker-compose pull
   docker-compose up -d
   ```

## Cleanup

```bash
# Stop and remove containers
docker-compose down

# Remove images
docker-compose down --rmi all

# Remove volumes (WARNING: deletes data)
docker-compose down -v

# Full cleanup
docker system prune -a --volumes
```

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Docker Build and Push

on:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Build images
        run: docker-compose build
      
      - name: Push to registry
        run: |
          docker push your-registry/payroll-api:latest
          docker push your-registry/payroll-frontend:latest
```

## Support

For issues or questions:
- Check logs: `docker-compose logs`
- Restart services: `docker-compose restart`
- Rebuild: `docker-compose up -d --build`

## Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [Nginx Docker Images](https://hub.docker.com/_/nginx)
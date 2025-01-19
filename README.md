# PhotSpot

## Before first run

### Server api

```bash
cd server_api
dotnet user-secrets set "Authentication:Google:ClientId" "<client-id>"
```

### Web admin

```bash
cd web_admin
dotnet user-secrets set "Authentication:Google:ClientId" "<client-id>"
dotnet user-secrets set "Authentication:Google:ClientSecret" "<client-secret>"
```

## Before every run

```bash
docker compose up -d
```


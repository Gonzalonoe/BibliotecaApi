# BibliotecaDigitalApi (.NET 8, MySQL, JWT)

API lista para usar con Android (Retrofit). Incluye:
- Login con JWT (PBKDF2)
- Bootstrap del primer Admin
- Creación de usuario Lector (solo Admin)
- Libros CRUD
- Pedidos (solicitudes) y flujo de préstamo/devolución
- Reportes simples

## Requisitos
- .NET 8 SDK
- MySQL 8+

## Configuración rápida
1. Edita `appsettings.json` si es necesario (cadena de conexión y claves JWT).
2. Crea la base ejecutando `sql/schema.sql` (opcional; la API llama `EnsureCreated()` en el arranque).
3. Ejecuta:
```bash
dotnet restore
dotnet run
```
Swagger: http://localhost:5140/swagger (o el puerto que indique la consola).

## Endpoints clave

### Auth
- `POST /api/auth/bootstrap-admin`
```json
{ "Nombre":"Admin", "Email":"admin@demo.com", "Password":"Admin123!" }
```
Solo funciona si aún no existe un Admin.

- `POST /api/auth/login`
```json
{ "Email":"admin@demo.com", "Password":"Admin123!" }
```
Respuesta:
```json
{ "token":"...", "userId":1, "nombre":"Admin", "email":"admin@demo.com", "rol":"Admin" }
```

### Usuarios
- `POST /api/usuarios/crear-lector` (Admin)
```json
{ "Nombre":"Lector 1", "Email":"lector@demo.com", "Password":"Lector123!" }
```

### Libros
- `GET /api/libros` (Admin/Lector)
- `POST /api/libros` (Admin)
```json
{ "Titulo":"La Santa Biblia", "Autor":"Varios", "Anio":1960, "Stock":3 }
```

### Pedidos
- `POST /api/pedidos` (Lector/Admin)
```json
{ "LibroId": 1, "TituloSolicitado": null }
```
o si no existe el libro:
```json
{ "LibroId": null, "TituloSolicitado": "La Santa Biblia" }
```

- `GET /api/pedidos/mios` (Lector/Admin)
- `GET /api/pedidos?estado=Prestado` (Admin)
- `POST /api/pedidos/{id}/aprobar` (Admin)
```json
{ "FechaVencimiento": "2025-12-31T23:59:00Z" }
```
- `POST /api/pedidos/{id}/marcar-prestado` (Admin)
```json
{ "Observaciones": "Retirado en mostrador" }
```
- `POST /api/pedidos/{id}/marcar-devolucion` (Admin)
```json
{ "Observaciones": "Devuelto en buen estado" }
```

### Reportes
- `GET /api/reportes/pedidos` (Admin)
- `GET /api/reportes/vencimientos` (Admin)

## Notas de seguridad
- Passwords con PBKDF2 (SHA-256, 100k iteraciones, salt por usuario + pepper de `appsettings:Salt`).
- Cambia `TokenJwt.SecretKey` por una cadena larga y aleatoria.
- Ajusta CORS y tiempos de expiración de JWT para producción.

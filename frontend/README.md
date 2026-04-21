# Viamatica Frontend

Frontend de la aplicación Viamatica desarrollado con **Angular 20** y **PrimeNG**.

## 🚀 Tecnologías

- **Angular 20.3** - Framework principal
- **PrimeNG 20.4** - Biblioteca de componentes UI
- **PrimeFlex** - Utilidades CSS
- **PrimeIcons** - Iconos
- **RxJS 7.8** - Programación reactiva
- **TypeScript 5.9** - Lenguaje tipado
- **XLSX** - Manejo de archivos Excel para carga masiva

## 📁 Estructura del Proyecto

```
src/
├── app/
│   ├── core/                      # Módulo Core (singleton)
│   │   ├── guards/                # Guards de autenticación y autorización
│   │   ├── interceptors/          # Interceptores HTTP
│   │   ├── models/                # Interfaces y modelos
│   │   └── services/              # Servicios globales
│   │
│   ├── shared/                    # Módulo Shared (reutilizable)
│   │   └── components/
│   │       ├── grid-shared/       # Grid reutilizable con filtros
│   │       ├── input-search-shared/ # Input con búsqueda en diálogo
│   │       ├── page-header/       # Encabezado de página
│   │       ├── stat-card/         # Tarjeta de estadísticas
│   │       └── loading-overlay/   # Overlay de carga
│   │
│   ├── layout/                    # Módulo de Layout
│   │   ├── main-layout/           # Layout principal
│   │   ├── topbar/                # Barra superior
│   │   ├── sidebar/               # Barra lateral con menú tree
│   │   └── footer/                # Pie de página
│   │
│   └── features/                  # Módulos lazy loaded
│       ├── auth/                  # Autenticación (login, forgot-password)
│       ├── welcome/               # Página de bienvenida
│       ├── dashboard/             # Dashboard (solo Administrator)
│       ├── users/                 # Usuarios + carga masiva xlsx/csv
│       ├── clients/               # Clientes + carga masiva xlsx/csv
│       ├── cashes/                # Cajas
│       ├── turns/                 # Turnos
│       ├── services/              # Servicios
│       ├── contracts/             # Contratos
│       └── attentions/            # Atenciones
│
├── environments/
│   ├── environment.ts             # Desarrollo
│   └── environment.prod.ts        # Producción
```

## 🔑 Características Principales

### Autenticación JWT
- Login con validación
- Tokens en localStorage
- Interceptor HTTP con Authorization header
- Guards para proteger rutas
- Logout con limpieza de sesión

### Autorización por Roles
- **Administrador**: Acceso completo + dashboard
- **Gestor**: Operaciones y catálogos
- **Cajero**: Operaciones de caja

### Menú Dinámico
- Consume `/api/navigation/menu`
- Árbol con PrimeNG Tree
- Cargado desde base de datos según rol

### Componentes Reutilizables
- **GridShared**: Grid con paginación, filtros, acciones
- **InputSearchShared**: Búsqueda en diálogo
- **LoadingOverlay**: Carga global
- **PageHeader**: Encabezado con acciones
- **StatCard**: Tarjetas de estadísticas

### Carga Masiva (users, clients)
- Archivos .xlsx o .csv
- Parsing con biblioteca XLSX
- Validación y procesamiento por lotes
- Reporte de resultados

## 🛠️ Comandos

```bash
# Instalar dependencias
npm install

# Desarrollo (http://localhost:4200)
npm start

# Build producción
npm run build

# Tests
npm test
```

## 🐳 Docker

```bash
docker compose up --build
```

- Frontend: `http://localhost:4200`
- Backend: `http://localhost:8080/swagger`
- El contenedor frontend usa **Nginx** y redirige `/api` hacia `viamatica-api:8080`

## 🔧 Configuración

### API URL
La aplicación usa rutas relativas (`/api`) y, en desarrollo, `ng serve` redirige automáticamente al backend con `proxy.conf.json`.

Si necesitas apuntar a otro host, cambia `apiUrl` en `src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: ''
};
```

## 📡 Endpoints Principales

- `POST /api/auth/login` - Login
- `GET /api/navigation/menu` - Menú dinámico
- `GET /api/users`, `POST /api/users`, etc. - CRUD usuarios
- `GET /api/clients`, `POST /api/clients`, etc. - CRUD clientes
- Y similares para cashes, turns, services, contracts, attentions

## 📦 Formato Carga Masiva

### Usuarios (users.xlsx/csv)
```
userName | email | password | firstName | lastName | role
user1 | user1@mail.com | Pass123! | Juan | Pérez | Cajero
```

### Clientes (clients.xlsx/csv)
```
identification | firstName | lastName | email | phone | address
1234567890 | Carlos | Ruiz | carlos@mail.com | 0991234567 | Av. Principal 123
```

## 🚦 Rutas

### Públicas
- `/auth/login`
- `/auth/forgot-password`

### Protegidas (autenticadas)
- `/welcome` - Todos
- `/dashboard` - Solo Administrador
- `/users`, `/clients`, `/cashes`, etc. - Según roles

## 🎨 Estilos
- PrimeFlex para utilidades
- PrimeIcons
- Variables CSS custom
- Responsive

## 📝 Notas
- Componentes sin sufijo `.component`
- `standalone: false` (módulos tradicionales)
- Lazy loading en features
- Validaciones con ReactiveFormsModule
- Toast y ConfirmDialog de PrimeNG

## ✅ Build Exitoso
```
✔ Application bundle generation complete.
Output: dist/viamatica-frontend/
```

---
Copyright © 2026 Viamatica.

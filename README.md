# Parent2Parent

Parent2Parent is a platform designed to connect parents within the same school community, facilitating communication and support.

## 🚀 Live Links

- **Frontend (UI)**: [https://parent2-parent-1.vercel.app](https://parent2-parent-1.vercel.app)
- **Backend (API)**: [https://parent2parent-1.onrender.com](https://parent2parent-1.onrender.com)
- **API Documentation**: [https://parent2parent-1.onrender.com/swagger](https://parent2parent-1.onrender.com/swagger)

## 📁 Project Structure

- **[Parent2Parent](./Parent2Parent)**: The backend API built with .NET 8.0 and C#. It handles user authentication, connection requests, and messaging.
- **[parent2parent-ui](./parent2parent-ui)**: The frontend application built with Angular 18. It provides a modern, responsive user interface for parents.

## 🛠️ Technology Stack

- **Backend**: .NET 8.0 Core Web API, SQL Server
- **Frontend**: Angular 18, RxJS, Tailwind CSS
- **Deployment**: Vercel (Frontend), Render (Backend with Docker)

## 🔧 Configuration

### Backend (Render)
Required Environment Variables:
- `ConnectionStrings__Parent2ParentDb`: Your SQL Server connection string.
- `AllowedOrigins`: `["https://parent2-parent-1.vercel.app"]`
- `ASPNETCORE_HTTP_PORTS`: `8080`

### Frontend (Vercel)
- **Root Directory**: `parent2parent-ui`
- **Output Directory**: `dist/browser`

## 👨‍💻 Local Development

1. **Backend**:
   ```bash
   cd Parent2Parent
   dotnet run
   ```
2. **Frontend**:
   ```bash
   cd parent2parent-ui
   npm install
   ng serve
   ```

## 📄 License
This project is licensed under the MIT License.

# Parent2Parent

## Overview

This application’s main purpose is to help parents enquire about school details without directly contacting school management. Instead, parents can connect with other parents whose children are already studying in that school.

Here, users can get genuine and real information about the school from experienced parents.

## 🚀 Live Links

- **Frontend (UI)**: [https://parent2-parent-1.vercel.app](https://parent2-parent-1.vercel.app)
- **Backend (API)**: [https://parent2parent-1.onrender.com](https://parent2parent-1.onrender.com)
- **API Documentation**: [https://parent2parent-1.onrender.com/swagger](https://parent2parent-1.onrender.com/swagger)

## Website Video Demo

[Watch Video Demo](https://drive.google.com/file/d/16zzkJ0azTcdsg1HJRnMyI8C4U70DEKFm/view?usp=sharing)

## 📁 Project Structure

- **[Parent2Parent](./Parent2Parent)**: The backend API built with .NET 8.0 and C#. It handles user authentication, connection requests, and messaging.
- **[parent2parent-ui](./parent2parent-ui)**: The frontend application built with Angular 18. It provides a modern and responsive user interface for parents.
- **Database**: Microsoft SQL Server (MSSQL).

## 🔧 Configuration

### Backend (Render)
Required Environment Variables:
- `ConnectionStrings__Parent2ParentDb`: Your SQL Server connection string
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

## 🚀 Application Flow

1. The user opens the website.
2. If the user is new, they can register and then log in.
3. The user searches for a school and gets a list of parents whose children are already studying there.
4. The user can send a connection request to a parent.
5. Once the request is accepted, the chat feature is unlocked.
6. Parents can then message each other and share information.

## 📄 License
This project is licensed under the MIT License.

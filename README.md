# Full-Stack Project with Jenkins CI/CD

This project is a full-stack application (ASP.NET Core API Backend, Blazor WebAssembly Frontend) containerized with Docker and orchestrated with Docker Compose. It includes a Jenkins CI/CD pipeline.

## 🛠️ Quick Start (Development)

Make sure you have **Docker** and **Docker Compose** installed.

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/bouthaina/project-with-jenkinsfile.git
    cd project-with-jenkinsfile
    ```

2.  **Build and start the containers:**
    ```bash
    docker-compose up --build
    ```

3.  **Access the application:**
    *   **Frontend (User Interface):** `http://localhost:5002`
    *   **Backend (API):** `http://localhost:5001`

## 🐳 Production Start

To start the application using the already built Docker images (requires a `.env` file for the database configuration):

```bash
docker-compose -f docker-compose.production.yml up -d
```

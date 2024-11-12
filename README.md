# Shopipy

## Prerequisites

Before running the Shopipy project, ensure you have the following prerequisites installed and configured on your
machine:

### .NET 8

- Install .NET 8 SDK from the official [.NET download page](https://dotnet.microsoft.com/download).

### HTTPS Development Certificate

- Run the following command to trust the HTTPS development certificate if it isn't already trusted on your machine:

```bash
$ dotnet dev-certs https --trust
```

### Aspire Package

The .NET Aspire workload installs internal dependencies and enables additional tooling, such as project templates and
features for Visual Studio and Visual Studio Code.

1. **Update Workloads**: Ensure you have the latest version of the .NET Aspire workload by running:

```bash
$ dotnet workload update
```

2. **Install Aspire**: Install the Aspire workload using the following command:

```bash
$ dotnet workload install aspire
```

3. **Check Aspire Version**: To verify that Aspire installed correctly, check the installed workloads with:

```bash
$ dotnet workload list
```

### Docker

Docker is required for Aspire to run certain components, such as the PostgreSQL database. Install Docker from the
official [Docker download page](https://www.docker.com/), and ensure Docker Desktop is running.

## Running the Project

### Standard .NET Run

1. **Clone the repository**:

```bash
$ git clone https://github.com/imbieras/shopipy
```

2. **Navigate to the project host directory**:

```bash
$ cd Shopipy.AppHost 
```

3. **Run the application**:

```bash
$ dotnet run -lp https
```

This starts the Shopipy application using .NET 8 with the Aspire workload and necessary dependencies.

### Running with Docker Compose (HTTP)

You can also launch the entire project with Docker Compose, with options for both manual rebuilds or hot reloading.

#### Without Hot Reloading

If you don’t need hot reloading and prefer to rebuild the Docker container manually for each change, follow these steps:

1. **Build the Docker containers**:

```bash
$ docker-compose build
```

2. **Start the project**:

```bash
$ docker-compose up
```

- To run the project in a detached state, use:

 ```bash
 $ docker-compose up -d
 ```

3. **Access the project**: Open your browser and navigate to [http://localhost:8000](http://localhost:8000).

4. **Stop the project**:

- If running in attached mode, use `CTRL-C`.
- If running in detached mode, stop it with:

 ```bash
 $ docker-compose down
 ```

Repeat these steps for any code changes you make.

#### With Hot Reloading

To enable a more efficient development process with hot reloading:

1. **Build the Docker containers**:

```bash
$ docker-compose build
```

2. **Start the project in the first terminal**:

```bash
$ docker-compose up
```

3. **Enable hot reloading**:

- Open a second terminal and run:

 ```bash
 $ dotnet watch
 ```

- This command will monitor your code and automatically update the project as changes occur.

4. **Access the project**: The project should open in your browser, and code changes will reflect automatically without
   requiring a rebuild. (Note that for some types of changes, you may still need to rebuild the Docker containers as
   necessary.)

5. **Stop the project**:

- Use `CTRL-C` on both terminals to stop the application, then rebuild if needed.
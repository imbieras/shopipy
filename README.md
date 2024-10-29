# Shopipy

## Prerequisites

Before running the Shopipy project, ensure you have the following prerequisites installed and configured on your machine:

### .NET 8
- Install .NET 8 SDK from the official [.NET download page](https://dotnet.microsoft.com/download).

### HTTPS Development Certificate
- Run the following command to trust the HTTPS development certificate if it isn't already trusted on your machine:

```bash
$ dotnet dev-certs https --trust
```

### Aspire Package
The .NET Aspire workload installs internal dependencies and enables additional tooling, such as project templates and features for Visual Studio and Visual Studio Code.

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

## Running the Project

1. Clone the repository:

```bash
$ git clone https://github.com/imbieras/shopipy
```

2. Change directory to the project host:

```bash
$ cd Shopipy.AppHost 
```

3. Run the application:

```bash
$ dotnet run -lp https
```

This will start the Shopipy application using .NET 8 with the Aspire workload and necessary dependencies.
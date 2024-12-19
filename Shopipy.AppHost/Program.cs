var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithDataVolume(isReadOnly: false).WithPgAdmin();
var postgresdb = postgres.AddDatabase("postgresdb");

var apiService = builder.AddProject<Projects.Shopipy_ApiService>("apiservice", "https").WithReference(postgresdb);

builder.AddProject<Projects.Shopipy_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
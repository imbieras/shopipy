var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithDataVolume(isReadOnly: false).WithPgAdmin();
var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.Shopipy_ApiService>("apiservice", "https").WithReference(postgresdb);

builder.Build().Run();
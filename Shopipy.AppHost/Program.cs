var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Shopipy_ApiService>("apiservice");

builder.AddProject<Projects.Shopipy_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
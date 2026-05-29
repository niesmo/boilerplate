var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var contactDb = postgres.AddDatabase("contactdb");

builder.AddProject<Projects.BoilerplateApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(contactDb)
    .WaitFor(contactDb);

builder.Build().Run();

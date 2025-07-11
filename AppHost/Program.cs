var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.API>("api");
builder.AddProject<Projects.MessageBroker>("messagebroker");

builder.Build().Run();

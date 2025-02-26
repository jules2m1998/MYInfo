var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder.AddSqlServer("myinfo-data");

builder.AddProject<Projects.MYInfo_API>("myinfo-api")
    .WithReference(sqlserver)
    .WaitFor(sqlserver);

await builder.Build().RunAsync();

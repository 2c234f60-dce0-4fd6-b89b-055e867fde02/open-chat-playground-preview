
var builder = DistributedApplication.CreateBuilder(args);

var DASHBOARD_DISPLAY_BASEURL = Environment.GetEnvironmentVariable("DASHBOARD_DISPLAY_BASEURL");
if (string.IsNullOrWhiteSpace(DASHBOARD_DISPLAY_BASEURL)){
    DASHBOARD_DISPLAY_BASEURL = "http://localhost";
}

builder.AddProject<Projects.OpenChat_PlaygroundApp>("playground-app")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5280")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5280");

builder.Build().Run();

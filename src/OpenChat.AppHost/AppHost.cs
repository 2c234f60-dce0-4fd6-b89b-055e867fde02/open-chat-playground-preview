var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.OpenChat_PlaygroundApp>("amazon-bedrock")
    .WithArgs("--connector-type", "AmazonBedrock")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5281")
    .WithUrl("http://tae0yifi.ddns.net:5281");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("azure-ai-foundry")
    .WithArgs("--connector-type", "AzureAIFoundry")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5282")
    .WithUrl("http://tae0yifi.ddns.net:5282");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("github-models")
    .WithArgs("--connector-type", "GitHubModels")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5283")
    .WithUrl("http://tae0yifi.ddns.net:5283");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("google-vertex-ai")
    .WithArgs("--connector-type", "GoogleVertexAI")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5284")
    .WithUrl("http://tae0yifi.ddns.net:5284");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("docker-model-runner")
    .WithArgs("--connector-type", "DockerModelRunner")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5285")
    .WithUrl("http://tae0yifi.ddns.net:5285");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("foundry-local")
    .WithArgs("--connector-type", "FoundryLocal")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5286")
    .WithUrl("http://tae0yifi.ddns.net:5286");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("hugging-face")
    .WithArgs("--connector-type", "HuggingFace")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5287")
    .WithUrl("http://tae0yifi.ddns.net:5287");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("ollama")
    .WithArgs("--connector-type", "Ollama")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5288")
    .WithUrl("http://tae0yifi.ddns.net:5288");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("anthropic")
    .WithArgs("--connector-type", "Anthropic")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5289")
    .WithUrl("http://tae0yifi.ddns.net:5289");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("lg")
    .WithArgs("--connector-type", "LG")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5290")
    .WithUrl("http://tae0yifi.ddns.net:5290");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("naver")
    .WithArgs("--connector-type", "Naver")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5291")
    .WithUrl("http://tae0yifi.ddns.net:5291");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("openai")
    .WithArgs("--connector-type", "OpenAI")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5292")
    .WithUrl("http://tae0yifi.ddns.net:5292");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("upstage")
    .WithArgs("--connector-type", "Upstage")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5293")
    .WithUrl("http://tae0yifi.ddns.net:5293");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("nc")
    .WithArgs("--connector-type", "NC")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5294")
    .WithUrl("http://tae0yifi.ddns.net:5294");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("skt")
    .WithArgs("--connector-type", "SKT")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5295")
    .WithUrl("http://tae0yifi.ddns.net:5295");

builder.Build().Run();

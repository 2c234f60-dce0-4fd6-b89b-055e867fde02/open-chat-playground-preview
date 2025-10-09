var builder = DistributedApplication.CreateBuilder(args);

var DASHBOARD_DISPLAY_BASEURL = Environment.GetEnvironmentVariable("DASHBOARD_DISPLAY_BASEURL");
if (string.IsNullOrWhiteSpace(DASHBOARD_DISPLAY_BASEURL)){
    DASHBOARD_DISPLAY_BASEURL = "http://localhost";
}

// Ollama 모델명 파라미터 (기본값: llama3.2)
var ollamaModel = Environment.GetEnvironmentVariable("OLLAMA_MODEL");
if (string.IsNullOrWhiteSpace(ollamaModel))
{
    ollamaModel = "llama3.2";
}

// Ollama 컨테이너 추가 (Dockerfile.ollama 사용)
var ollamaServer = builder.AddContainer("ollama-server", "ollama/ollama:latest")
    .WithDockerfile("Dockerfile.ollama")
    .WithEnvironment("OLLAMA_MODEL", ollamaModel)
    .WithEndpoint(11434, name: "http", isExternal: true);

builder.AddProject<Projects.OpenChat_PlaygroundApp>("amazon-bedrock")
    .WithArgs("--connector-type", "AmazonBedrock")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5281")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5281");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("azure-ai-foundry")
    .WithArgs("--connector-type", "AzureAIFoundry")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5282")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5282");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("github-models")
    .WithArgs("--connector-type", "GitHubModels")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5283")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5283");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("google-vertex-ai")
    .WithArgs("--connector-type", "GoogleVertexAI")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5284")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5284");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("docker-model-runner")
    .WithArgs("--connector-type", "DockerModelRunner")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5285")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5285");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("foundry-local")
    .WithArgs("--connector-type", "FoundryLocal")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5286")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5286");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("hugging-face")
    .WithArgs("--connector-type", "HuggingFace")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5287")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5287");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("ollama")
    .WithArgs("--connector-type", "Ollama")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5288")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5288")
    .WithEnvironment("OllamaUrl", ollamaServer.GetEndpoint("http"));

builder.AddProject<Projects.OpenChat_PlaygroundApp>("anthropic")
    .WithArgs("--connector-type", "Anthropic")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5289")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5289");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("lg")
    .WithArgs("--connector-type", "LG")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5290")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5290");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("naver")
    .WithArgs("--connector-type", "Naver")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5291")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5291");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("openai")
    .WithArgs("--connector-type", "OpenAI")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5292")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5292");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("upstage")
    .WithArgs("--connector-type", "Upstage")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5293")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5293");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("nc")
    .WithArgs("--connector-type", "NC")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5294")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5294");

builder.AddProject<Projects.OpenChat_PlaygroundApp>("skt")
    .WithArgs("--connector-type", "SKT")
    .WithEnvironment("ASPNETCORE_URLS", "http://*:5295")
    .WithUrl(DASHBOARD_DISPLAY_BASEURL+":5295");

builder.Build().Run();

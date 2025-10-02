using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;
using Mscc.GenerativeAI.Microsoft;
using Mscc.GenerativeAI;

namespace OpenChat.PlaygroundApp.Connectors;

public class GoogleVertexAIConnector(AppSettings settings) : LanguageModelConnector(settings.GoogleVertexAI)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as GoogleVertexAISettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI.");
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI:ApiKey.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI:Model.");
        return true;
    }

    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as GoogleVertexAISettings;

        // 기존 : GeminiClient 사용 
        // IChatClient chatClient = new GeminiChatClient(settings!.ApiKey!, settings!.Model!);
        // return await Task.FromResult(chatClient).ConfigureAwait(false);

        // 변경1 : VertexAI 사용
        string accessToken = await GetAccessTokenFromGcloudAsync();
        var vertexAI = new VertexAI(projectId: settings!.ProjectId!, region: settings!.Region!);
        var model = vertexAI.GenerativeModel(model: settings!.Model!);
        model.AccessToken = accessToken;
        return await Task.FromResult(model.AsIChatClient()).ConfigureAwait(false);

        // 변경2 : GoogleAI 사용
        //var googleAI = new GoogleAI(accessToken: settings!.AccessToken!);
        //var model = googleAI.GenerativeModel(model: settings.Model!);
        //return await Task.FromResult(model.AsIChatClient()).ConfigureAwait(false);
    }

    private static async Task<string> GetAccessTokenFromGcloudAsync()
    {
        // OS별로 gcloud 명령어 실행 방식 분기 및 경로 확인
        System.Diagnostics.ProcessStartInfo psi;
        string gcloudPath = "gcloud";
        string userAliasPath = "/Users/bachtaeyeong/User_Libraries/google-cloud-sdk/bin/gcloud";
        if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
        {
            // macOS/Linux: gcloud 경로 탐색 (기본, which, alias 순서)
            bool found = false;
            // 1. 기본 경로(gcloud)가 실행 가능한지 확인
            try
            {
                var testPsi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = gcloudPath,
                    Arguments = "version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var testProcess = System.Diagnostics.Process.Start(testPsi);
                if (testProcess != null)
                {
                    await testProcess.WaitForExitAsync();
                    if (testProcess.ExitCode == 0)
                        found = true;
                }
            }
            catch { }

            // 2. which gcloud
            if (!found)
            {
                var whichPsi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "which",
                    Arguments = "gcloud",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var whichProcess = System.Diagnostics.Process.Start(whichPsi);
                if (whichProcess != null)
                {
                    string whichOutput = await whichProcess.StandardOutput.ReadToEndAsync();
                    await whichProcess.WaitForExitAsync();
                    if (whichProcess.ExitCode == 0 && !string.IsNullOrWhiteSpace(whichOutput))
                    {
                        gcloudPath = whichOutput.Trim();
                        found = true;
                    }
                }
            }

            // 3. 직접 alias 경로
            if (!found && System.IO.File.Exists(userAliasPath))
            {
                gcloudPath = userAliasPath;
                found = true;
            }

            if (!found)
            {
                throw new InvalidOperationException("gcloud 실행 파일을 찾을 수 없습니다. gcloud CLI가 설치되어 있고 PATH 또는 alias에 등록되어 있는지 확인하세요.");
            }
        }

        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
        {
            // Windows: cmd.exe에서 /c gcloud ...
            psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c gcloud auth application-default print-access-token",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }
        else
        {
            // macOS/Linux: gcloud 경로 직접 지정
            psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = gcloudPath,
                Arguments = "auth application-default print-access-token",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }
        using var process = System.Diagnostics.Process.Start(psi);
        if (process == null)
        {
            throw new InvalidOperationException("gcloud 명령어 실행 실패: 프로세스 시작 불가");
        }
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
        if (process.ExitCode != 0)
        {
            string error = await process.StandardError.ReadToEndAsync();
            throw new InvalidOperationException($"gcloud 명령어 실패: {error}");
        }
        Console.WriteLine(output.Trim());
        return output.Trim();
    }
}

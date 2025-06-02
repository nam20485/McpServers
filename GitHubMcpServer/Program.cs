using GitHubMcpServer.Services;
using GitHubMcpServer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octokit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GitHubMcpServer;

public class Program
{
    private const string TOKEN_ENV_VAR_NAME = "MCPSERVER_GH_TOKEN";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var gitHubService = host.Services.GetRequiredService<GitHubToolsService>();
        
        logger.LogInformation("GitHub MCP Server starting...");
        
        // Run the MCP server
        await RunMcpServerAsync(gitHubService, logger);
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;
                
                // Configure GitHub client
                var githubToken = configuration["GitHub:Token"] ?? 
                                Environment.GetEnvironmentVariable(TOKEN_ENV_VAR_NAME) ??
                                throw new InvalidOperationException($"GitHub token not found. Set {TOKEN_ENV_VAR_NAME} environment variable");
                
                var githubClient = new GitHubClient(new ProductHeaderValue("GitHubMcpServer", "1.0.0"))
                {
                    Credentials = new Credentials(githubToken)
                };
                
                services.AddSingleton(githubClient);
                services.AddSingleton<GitHubToolsService>();
            });

    private static async Task RunMcpServerAsync(GitHubToolsService gitHubService, ILogger logger)
    {
        using var stdin = Console.OpenStandardInput();
        using var stdout = Console.OpenStandardOutput();
        using var reader = new StreamReader(stdin);
        using var writer = new StreamWriter(stdout) { AutoFlush = true };

        logger.LogInformation("MCP Server ready and listening for requests...");

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            try
            {
                logger.LogDebug("Received: {Line}", line);
                
                var request = JsonSerializer.Deserialize<McpRequest>(line, JsonOptions);
                if (request == null)
                {
                    logger.LogWarning("Failed to deserialize request: {Line}", line);
                    continue;
                }

                var response = await ProcessRequestAsync(request, gitHubService, logger);
                var responseJson = JsonSerializer.Serialize(response, JsonOptions);
                
                logger.LogDebug("Sending: {Response}", responseJson);
                await writer.WriteLineAsync(responseJson);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing request: {Line}", line);
                
                var errorResponse = new McpResponse
                {
                    Id = null,
                    Error = new McpError
                    {
                        Code = -32603,
                        Message = "Internal error",
                        Data = ex.Message
                    }
                };
                
                var errorJson = JsonSerializer.Serialize(errorResponse, JsonOptions);
                await writer.WriteLineAsync(errorJson);
            }
        }
    }

    private static async Task<McpResponse> ProcessRequestAsync(McpRequest request, GitHubToolsService gitHubService, ILogger logger)
    {
        logger.LogInformation("Processing method: {Method}", request.Method);
        
        try
        {
            return request.Method switch
            {
                "initialize" => HandleInitialize(request),
                "tools/list" => HandleToolsList(request, gitHubService),
                "tools/call" => await HandleToolsCallAsync(request, gitHubService),
                _ => new McpResponse
                {
                    Id = request.Id,
                    Error = new McpError
                    {
                        Code = -32601,
                        Message = $"Method not found: {request.Method}"
                    }
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling method {Method}", request.Method);
            return new McpResponse
            {
                Id = request.Id,
                Error = new McpError
                {
                    Code = -32603,
                    Message = "Internal error",
                    Data = ex.Message
                }
            };
        }
    }

    private static McpResponse HandleInitialize(McpRequest request)
    {
        var result = new McpInitializeResult
        {
            Capabilities = new McpCapabilities
            {
                Tools = new McpToolsCapability()
            },
            ServerInfo = new McpServerInfo
            {
                Name = "GitHub MCP Server",
                Version = "1.0.0"
            }
        };

        return new McpResponse
        {
            Id = request.Id,
            Result = result
        };
    }

    private static McpResponse HandleToolsList(McpRequest request, GitHubToolsService gitHubService)
    {
        var tools = gitHubService.GetAvailableTools();
        
        return new McpResponse
        {
            Id = request.Id,
            Result = new { tools }
        };
    }

    private static async Task<McpResponse> HandleToolsCallAsync(McpRequest request, GitHubToolsService gitHubService)
    {
        if (request.Params is not JsonElement paramsElement)
        {
            return new McpResponse
            {
                Id = request.Id,
                Error = new McpError
                {
                    Code = -32602,
                    Message = "Invalid params"
                }
            };
        }

        var toolCall = JsonSerializer.Deserialize<McpToolCall>(paramsElement.GetRawText(), JsonOptions);
        if (toolCall == null)
        {
            return new McpResponse
            {
                Id = request.Id,
                Error = new McpError
                {
                    Code = -32602,
                    Message = "Invalid tool call parameters"
                }
            };
        }

        var result = await gitHubService.ExecuteToolAsync(toolCall);
        
        return new McpResponse
        {
            Id = request.Id,
            Result = result
        };
    }
}

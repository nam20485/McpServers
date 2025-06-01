using System.Text.Json.Serialization;

namespace GitHubMcpServer.Models;

// MCP Protocol Base Models
public record McpRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; init; } = "2.0";
    
    [JsonPropertyName("id")]
    public object? Id { get; init; }
    
    [JsonPropertyName("method")]
    public required string Method { get; init; }
    
    [JsonPropertyName("params")]
    public object? Params { get; init; }
}

public record McpResponse
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; init; } = "2.0";
    
    [JsonPropertyName("id")]
    public object? Id { get; init; }
    
    [JsonPropertyName("result")]
    public object? Result { get; init; }
    
    [JsonPropertyName("error")]
    public McpError? Error { get; init; }
}

public record McpError
{
    [JsonPropertyName("code")]
    public int Code { get; init; }
    
    [JsonPropertyName("message")]
    public required string Message { get; init; }
    
    [JsonPropertyName("data")]
    public object? Data { get; init; }
}

// MCP Tool Models
public record McpTool
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    
    [JsonPropertyName("description")]
    public required string Description { get; init; }
    
    [JsonPropertyName("inputSchema")]
    public required McpToolInputSchema InputSchema { get; init; }
}

public record McpToolInputSchema
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "object";
    
    [JsonPropertyName("properties")]
    public required Dictionary<string, McpProperty> Properties { get; init; }
    
    [JsonPropertyName("required")]
    public string[]? Required { get; init; }
}

public record McpProperty
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }
    
    [JsonPropertyName("description")]
    public required string Description { get; init; }
    
    [JsonPropertyName("enum")]
    public string[]? Enum { get; init; }
}

// Tool execution models
public record McpToolCall
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    
    [JsonPropertyName("arguments")]
    public required Dictionary<string, object> Arguments { get; init; }
}

public record McpToolResult
{
    [JsonPropertyName("content")]
    public required McpContent[] Content { get; init; }
    
    [JsonPropertyName("isError")]
    public bool IsError { get; init; }
}

public record McpContent
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }
    
    [JsonPropertyName("text")]
    public required string Text { get; init; }
}

// Server info models
public record McpServerInfo
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    
    [JsonPropertyName("version")]
    public required string Version { get; init; }
    
    [JsonPropertyName("protocolVersion")]
    public string ProtocolVersion { get; init; } = "2024-11-05";
}

public record McpCapabilities
{
    [JsonPropertyName("tools")]
    public McpToolsCapability? Tools { get; init; }
}

public record McpToolsCapability
{
    [JsonPropertyName("listChanged")]
    public bool ListChanged { get; init; } = false;
}

public record McpInitializeResult
{
    [JsonPropertyName("protocolVersion")]
    public string ProtocolVersion { get; init; } = "2024-11-05";
    
    [JsonPropertyName("capabilities")]
    public required McpCapabilities Capabilities { get; init; }
    
    [JsonPropertyName("serverInfo")]
    public required McpServerInfo ServerInfo { get; init; }
}

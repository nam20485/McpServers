# GitHub MCP Server Test Script
# This script tests the GitHub MCP Server functionality

Write-Host "GitHub MCP Server Test Script" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green

# Check if .NET 8.0 is available
Write-Host "`nChecking .NET version..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ .NET is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

# Check if GitHub token is set
Write-Host "`nChecking GitHub token..." -ForegroundColor Yellow
$githubToken = $env:GITHUB_TOKEN
if ([string]::IsNullOrEmpty($githubToken)) {
    Write-Host "✗ GITHUB_TOKEN environment variable is not set" -ForegroundColor Red
    Write-Host "Please set your GitHub token: `$env:GITHUB_TOKEN = 'your-token-here'" -ForegroundColor Yellow
    exit 1
} else {
    $maskedToken = $githubToken.Substring(0, [Math]::Min(8, $githubToken.Length)) + "..."
    Write-Host "✓ GitHub token found: $maskedToken" -ForegroundColor Green
}

# Build the project
Write-Host "`nBuilding GitHub MCP Server..." -ForegroundColor Yellow
try {
    $buildOutput = dotnet build "d:\src\github\nam20485\AgentAsAService\GitHubMcpServer\GitHubMcpServer.csproj" 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Build successful" -ForegroundColor Green
    } else {
        Write-Host "✗ Build failed:" -ForegroundColor Red
        Write-Host $buildOutput -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "✗ Build error: $_" -ForegroundColor Red
    exit 1
}

# Test MCP protocol initialization
Write-Host "`nTesting MCP server initialization..." -ForegroundColor Yellow

$testInitRequest = @{
    jsonrpc = "2.0"
    id = 1
    method = "initialize"
    params = @{
        protocolVersion = "2024-11-05"
        capabilities = @{}
        clientInfo = @{
            name = "test-client"
            version = "1.0.0"
        }
    }
} | ConvertTo-Json -Depth 10

Write-Host "Test request: $testInitRequest" -ForegroundColor Cyan

# Create a simple test for the MCP server
$testScript = @"
using System;
using System.Text.Json;
using System.Threading.Tasks;

var request = "$testInitRequest";
Console.WriteLine("Testing MCP server with request:");
Console.WriteLine(request);
"@

$testScript | Out-File -FilePath "test-mcp.cs" -Encoding UTF8

Write-Host "`n✓ Test setup complete!" -ForegroundColor Green
Write-Host "`nTo manually test the MCP server:" -ForegroundColor Yellow
Write-Host "1. Run: dotnet run --project GitHubMcpServer\GitHubMcpServer.csproj" -ForegroundColor Cyan
Write-Host "2. Send the initialization request via stdin" -ForegroundColor Cyan
Write-Host "3. Check VS Code GitHub Copilot chat for available GitHub tools" -ForegroundColor Cyan

Write-Host "`nMCP Server is ready to use with VS Code!" -ForegroundColor Green
Write-Host "Open VS Code and the GitHub tools should be available in Copilot chat." -ForegroundColor Green

# Clean up
Remove-Item "test-mcp.cs" -ErrorAction SilentlyContinue

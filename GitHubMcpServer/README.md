# GitHub CLI MCP Server

A Model Context Protocol (MCP) server that provides GitHub CLI tools through the Octokit.NET library. This server enables AI assistants to interact with GitHub repositories, issues, pull requests, and more.

## Features

### Repository Operations
- `github_list_repositories` - List repositories for the authenticated user or a specific user
- `github_create_repository` - Create a new GitHub repository

### Issue Management
- `github_list_issues` - List issues in a repository
- `github_create_issue` - Create a new issue
- `github_update_issue` - Update an existing issue (title, body, state)

### Pull Request Operations
- `github_list_pull_requests` - List pull requests in a repository

### File Operations
- `github_read_file` - Read a file from a GitHub repository

### Additional Tools (Coming Soon)
- Branch management
- File writing and commits
- Release management
- GitHub Actions workflows

## Setup

### Prerequisites
- .NET 8.0 SDK
- GitHub Personal Access Token

### Installation

1. **Build the project:**
   ```bash
   dotnet build GitHubMcpServer.csproj
   ```

2. **Set up GitHub authentication:**
   
   Option A: Environment Variable
   ```bash
   $env:GITHUB_TOKEN = "your-github-token-here"
   ```

3. **Run the server:**
   ```bash
   dotnet run

   ```

### VS Code Integration

The server is automatically configured in your VS Code settings. Make sure you have:

1. GitHub Copilot extension installed
2. MCP enabled in VS Code settings
3. GitHub token available when prompted

## Usage Examples

### List Repositories
```json
{
  "name": "github_list_repositories",
  "arguments": {
    "owner": "octocat"
  }

}
```

### Create Issue
```json
{
  "name": "github_create_issue",
  "arguments": {
    "owner": "octocat",
    "repository": "Hello-World",
    "title": "Bug report",
    "body": "There's a bug in the application",
    "labels": ["bug", "high-priority"]
  }
}
```

### Read File
```json
{
  "name": "github_read_file",
  "arguments": {
    "owner": "octocat",
    "repository": "Hello-World",
    "path": "README.md",
    "branch": "main"
  }
}
```

## Security

- Always use GitHub Personal Access Tokens with minimum required permissions
- Tokens are not logged or stored permanently
- Consider using fine-grained personal access tokens for enhanced security

## Documentation References

- [Octokit.NET Documentation](https://octokitnet.readthedocs.io/)
- [GitHub REST API Documentation](https://docs.github.com/en/rest)
- [Model Context Protocol Specification](https://modelcontextprotocol.io/introduction)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)

## Contributing

This MCP server is part of the MCPServers solution. To contribute:

1. Follow ASP.NET Core best practices
2. Ensure all new tools have proper input validation
3. Add comprehensive error handling
4. Update this README with new tool documentation

## License

This project is licensed under the GNU Affero General Public License v3.0 - see the [AGPL-3.0](../agpl-3.0.txt) file for details.

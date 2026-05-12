# Migration to File Server - Implementation Guide

## 🎯 Overview

This guide describes how to migrate from **folder-based file serving** to a **dedicated file server** as your application scales.

## 📊 When to Migrate

### Current Setup (Single Server)
```
Web App Server
    ├─ Application Code
    ├─ WWWRoot/ExampleFiles  ← Local files
    └─ Service
```

**Pros:**
- Simple to set up and maintain
- No external dependencies
- Good for small deployments
- Fast for single server

**Cons:**
- Doesn't scale to multiple servers
- No redundancy
- Difficult to update files without redeploying
- Uses server storage

### Target Setup (Scaled)
```
Web App Server 1    Web App Server 2    Web App Server 3
    └─ Service         └─ Service         └─ Service
         ↓                  ↓                  ↓
         └──────────────────┬──────────────────┘
                            ↓
                    File Server API
                    - Azure Blob Storage
                    - AWS S3
                    - Dedicated File Server
                    - CDN
```

**Pros:**
- Scales to multiple web servers
- Centralized file management
- Easy updates without redeploying
- Can use cloud storage services
- Better performance with CDN

**Cons:**
- Additional complexity
- External service dependency
- Network latency considerations
- Cost for external storage

## 🔄 Migration Steps

### Phase 1: Create FileServerExampleFileProvider

```csharp
// SpreadsheetUtility.UI.Web/Services/FileServerExampleFileProvider.cs

using SpreadsheetUtility.UI.Web.Models;

namespace SpreadsheetUtility.UI.Web.Services;

/// <summary>
/// Serves example files from a dedicated file server.
/// Scales to multiple web application servers.
/// </summary>
public class FileServerExampleFileProvider : IExampleFileProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileServerExampleFileProvider> _logger;
    private readonly string _fileServerUrl;

    public FileServerExampleFileProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<FileServerExampleFileProvider> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _fileServerUrl = configuration["FileServer:Url"] ?? throw new InvalidOperationException("FileServer:Url not configured");
    }

    public async Task<IEnumerable<ExampleFileInfo>> GetAvailableFilesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching available files from file server: {Url}", _fileServerUrl);

            var response = await _httpClient.GetAsync($"{_fileServerUrl}/api/files");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var files = System.Text.Json.JsonSerializer.Deserialize<List<ExampleFileInfo>>(json)
                ?? Enumerable.Empty<ExampleFileInfo>().ToList();

            _logger.LogInformation("Retrieved {FileCount} files from file server", files.Count);
            return files;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error communicating with file server");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving files from file server");
            throw;
        }
    }

    public async Task<FileDownloadDto> GetFileAsync(string fileName)
    {
        ValidateFileName(fileName);

        try
        {
            _logger.LogInformation("Downloading file from file server: {FileName}", fileName);

            var response = await _httpClient.GetAsync($"{_fileServerUrl}/api/files/{fileName}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsByteArrayAsync();

            _logger.LogInformation("Successfully downloaded file: {FileName} ({SizeBytes} bytes)", fileName, content.Length);

            return new FileDownloadDto
            {
                FileName = fileName,
                DisplayName = Path.GetFileNameWithoutExtension(fileName),
                Content = content,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("File not found on file server: {FileName}", fileName);
            throw new FileNotFoundException($"File not found: {fileName}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error downloading file from file server: {FileName}", fileName);
            throw;
        }
    }

    public async Task<ExampleFileInfo?> GetFileInfoAsync(string fileName)
    {
        ValidateFileName(fileName);

        try
        {
            _logger.LogInformation("Getting file info from file server: {FileName}", fileName);

            var request = new HttpRequestMessage(HttpMethod.Head, $"{_fileServerUrl}/api/files/{fileName}");
            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogDebug("File not found on file server: {FileName}", fileName);
                return null;
            }

            response.EnsureSuccessStatusCode();

            // Try to parse from response headers
            var contentLength = response.Content.Headers.ContentLength;
            var lastModified = response.Content.Headers.LastModified;

            return new ExampleFileInfo
            {
                FileName = fileName,
                DisplayName = Path.GetFileNameWithoutExtension(fileName),
                Description = "Example spreadsheet for Gantt chart generation",
                FileSizeBytes = contentLength ?? 0,
                LastModified = lastModified?.DateTime ?? DateTime.Now
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error getting file info from file server: {FileName}", fileName);
            return null;
        }
    }

    private void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));

        if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
            throw new ArgumentException("Invalid file name", nameof(fileName));

        if (!fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Only .xlsx files are allowed", nameof(fileName));
    }
}
```

### Phase 2: Update Configuration

#### appsettings.json
```json
{
  "FileServer": {
    "Enabled": false,
    "Url": "https://files.example.com"
  }
}
```

#### appsettings.Production.json
```json
{
  "FileServer": {
    "Enabled": true,
    "Url": "https://files.production.example.com"
  }
}
```

### Phase 3: Update Program.cs

```csharp
// In Program.cs, replace the service registration:

// OLD (Single server):
// builder.Services.AddScoped<IExampleFileProvider, FolderExampleFileProvider>();

// NEW (With configuration switching):
var fileServerEnabled = builder.Configuration.GetValue<bool>("FileServer:Enabled");

if (fileServerEnabled)
{
    builder.Services.AddScoped<IExampleFileProvider, FileServerExampleFileProvider>();
    builder.Services.AddHttpClient<IExampleFileProvider, FileServerExampleFileProvider>();
}
else
{
    builder.Services.AddScoped<IExampleFileProvider, FolderExampleFileProvider>();
}
```

### Phase 4: Deploy File Server

Choose your file server backend:

#### Option A: Azure Blob Storage
```csharp
public class AzureBlobExampleFileProvider : IExampleFileProvider
{
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<AzureBlobExampleFileProvider> _logger;

    public AzureBlobExampleFileProvider(BlobContainerClient containerClient, ILogger<AzureBlobExampleFileProvider> logger)
    {
        _containerClient = containerClient;
        _logger = logger;
    }

    public async Task<IEnumerable<ExampleFileInfo>> GetAvailableFilesAsync()
    {
        var files = new List<ExampleFileInfo>();
        await foreach (var item in _containerClient.GetBlobsAsync())
        {
            if (item.Name.EndsWith(".xlsx"))
            {
                var properties = await _containerClient.GetBlobClient(item.Name).GetPropertiesAsync();
                files.Add(new ExampleFileInfo
                {
                    FileName = item.Name,
                    DisplayName = Path.GetFileNameWithoutExtension(item.Name),
                    Description = "Example spreadsheet",
                    FileSizeBytes = properties.Value.ContentLength,
                    LastModified = properties.Value.LastModified.DateTime
                });
            }
        }
        return files.OrderBy(f => f.DisplayName);
    }

    // Implement other methods similarly...
}
```

#### Option B: AWS S3
Similar pattern using AWS SDK

#### Option C: Simple HTTP File Server
Deploy a simple REST API that serves files from a networked location

### Phase 5: Testing

```csharp
// Test both implementations work
[TestClass]
public class ExampleFileProviderTests
{
    [TestMethod]
    public async Task FolderProvider_ReturnsFiles()
    {
        var provider = new FolderExampleFileProvider(hostEnvironment, logger);
        var files = await provider.GetAvailableFilesAsync();
        Assert.IsTrue(files.Any());
    }

    [TestMethod]
    public async Task FileServerProvider_ReturnsFiles()
    {
        var httpClient = new HttpClient();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new[] { new KeyValuePair<string, string>("FileServer:Url", "http://localhost:3000") })
            .Build();

        var provider = new FileServerExampleFileProvider(httpClient, config, logger);
        var files = await provider.GetAvailableFilesAsync();
        Assert.IsTrue(files.Any());
    }
}
```

## 📋 Migration Checklist

### Pre-Migration
- [ ] Current setup working properly
- [ ] All files properly located
- [ ] Backups created
- [ ] Team notified
- [ ] Maintenance window scheduled

### Migration Execution
- [ ] Create FileServerExampleFileProvider
- [ ] Set up file server infrastructure
- [ ] Configure appsettings.json
- [ ] Update Program.cs with conditional registration
- [ ] Deploy file server
- [ ] Copy files to file server
- [ ] Test with file server enabled
- [ ] Deploy to staging environment
- [ ] Run full integration tests
- [ ] Verify API endpoints work
- [ ] Verify Blazor UI works

### Post-Migration
- [ ] Monitor for errors
- [ ] Verify performance
- [ ] Check logs
- [ ] Update documentation
- [ ] Train team on new setup
- [ ] Remove FolderExampleFileProvider from production (keep in dev)
- [ ] Archive old files

## 🎯 Migration Strategy

### Option 1: Big Bang (Recommended for small teams)
1. Deploy file server
2. Update all application instances simultaneously
3. Verify everything works
4. Remove old code

**Risk:** Low if thoroughly tested
**Downtime:** Minimal (seconds)
**Testing:** Medium

### Option 2: Blue-Green Deployment
1. Deploy new infrastructure
2. Route 10% of traffic to new system
3. Monitor for issues
4. Gradually increase percentage
5. Full cutover once confident

**Risk:** Very low
**Downtime:** None
**Testing:** Extensive

### Option 3: Gradual Rollout
1. Deploy new FileServerExampleFileProvider
2. Keep configuration to use FolderExampleFileProvider
3. Add feature flag to switch between providers
4. Gradually enable for users
5. Complete migration after stabilization

**Risk:** Very low
**Downtime:** None
**Testing:** Extensive, with real usage data

## 🔍 Monitoring and Troubleshooting

### Performance Metrics to Track
- File download time
- API response time
- Error rates
- File server availability
- Network latency

### Common Issues and Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| 404 errors | Files not on server | Copy files to file server |
| Slow downloads | Network latency | Use CDN or edge caching |
| Timeout errors | Server overloaded | Scale file server |
| Authentication errors | No API key | Configure API credentials |
| Connection refused | Server offline | Check file server status |

### Logging Points
```csharp
// Add logging for migration troubleshooting
_logger.LogInformation("Using {Provider} for example files", 
    fileServerEnabled ? "FileServerExampleFileProvider" : "FolderExampleFileProvider");
```

## 🔄 Rollback Plan

If migration fails:

1. **Immediate**: Revert configuration to use FolderExampleFileProvider
2. **Deploy**: Push updated configuration to production
3. **Verify**: Test that files are being served
4. **Investigate**: Analyze logs to find issue
5. **Fix**: Resolve file server issues
6. **Retry**: Attempt migration again

## 📚 Deployment Options Summary

| Option | Cost | Complexity | Scalability | Pros | Cons |
|--------|------|-----------|------------|------|------|
| Local Folder | $0 | Low | Poor | Simple | Doesn't scale |
| Azure Blob | $ | Medium | Excellent | CDN, redundancy | Cost, complexity |
| AWS S3 | $ | Medium | Excellent | CDN, global | Cost, vendor lock-in |
| File Server | $$ | Medium | Good | Controlled | Maintenance needed |
| CDN | $$$ | High | Excellent | Fast, global | Most complex |

## ✅ Completion Checklist

- [ ] FileServerExampleFileProvider created
- [ ] Configuration updated
- [ ] Program.cs updated for conditional registration
- [ ] File server set up
- [ ] Files migrated to file server
- [ ] Testing completed
- [ ] Deployment plan created
- [ ] Rollback procedure documented
- [ ] Team trained
- [ ] Monitoring set up

## 🎁 Benefits After Migration

✅ **Scalability**: Works with multiple web servers
✅ **Flexibility**: Easy to add/update files without redeploying
✅ **CDN Ready**: Can serve through CDN for faster downloads
✅ **Cloud Native**: Works with cloud storage services
✅ **Separation**: Application and files on separate systems

---

**Documentation Version:** 1.0
**Last Updated:** 2024-01-15
**Status:** Ready for Implementation

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SpreadsheetUtilities_Auth_Api>("spreadsheetutilities-auth-api");

builder.AddProject<Projects.SpreadsheetUtility_UI_Web>("spreadsheetutility-ui-web");

builder.Build().Run();

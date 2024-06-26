using Elsa.EntityFrameworkCore.Modules.Identity;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Samples.AspNet.DynamicActivityProvider.ActivityProviders;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add Elsa services.
services.AddElsa(elsa => elsa
    // Expose API endpoints.
    .UseWorkflowsApi()
    
    .UseWorkflowManagement(management => management.UseEntityFrameworkCore())
    .UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore())

    // Configure identity so that we can create a default admin user.
    .UseIdentity(identity =>
    {
        identity.UseAdminUserProvider();
        identity.UseEntityFrameworkCore();
        identity.TokenOptions = options =>
        {
            options.SigningKey = "secret-token-signing-key-with-a-minimum-length-of-256-bits";
            options.AccessTokenLifetime = TimeSpan.FromDays(1);
        };
    })
    .UseJavaScript()
    .UseHttp()
    .UseDefaultAuthentication(auth => auth.UseAdminApiKey())
);

// Add custom activity provider.
services.AddActivityProvider<ApiActivityProvider>();

services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

// Configure middleware pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.UseWorkflowsApi();
app.UseWorkflows();
app.Run();
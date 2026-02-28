WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, config) =>
    config.ReadFrom.Configuration(ctx.Configuration));
builder.ConfigureBuilder();

WebApplication app = builder.Build();
app.ConfigureApp();
app.Run();

public partial class Program { }


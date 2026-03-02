WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.ConfigureBuilder();

WebApplication app = builder.Build();
app.ConfigureApp();
app.Run();

public partial class Program { }


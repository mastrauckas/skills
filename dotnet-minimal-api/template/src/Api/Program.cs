var builder = WebApplication.CreateBuilder(args);
builder.RegisterOpenApi();
builder.RegisterAuthentication();
builder.RegisterCors();
builder.RegisterServices();

var app = builder.Build();
app.ConfigureApp();
app.Run();

public partial class Program { }


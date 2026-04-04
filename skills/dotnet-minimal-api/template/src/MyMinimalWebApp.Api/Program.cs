var builder = WebApplication.CreateBuilder(args);
builder.ConfigureBuilder();

var app = builder.Build();
app.ConfigureApp();
app.Run();

public partial class Program { }


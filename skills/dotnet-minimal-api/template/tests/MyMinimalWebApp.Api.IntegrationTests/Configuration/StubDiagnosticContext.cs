namespace MyMinimalWebApp.Api.IntegrationTests.Configuration;

internal sealed class StubDiagnosticContext : IDiagnosticContext
{
    public string? LastPropertyName { get; private set; }

    public void Set(string propertyName, object? value, bool destructureObjects = false)
        => LastPropertyName = propertyName;

    public void SetException(Exception exception) { }
}

namespace MyMinimalWebApp.Api.IntegrationTests.Configuration;

internal sealed class StubDiagnosticContext : IDiagnosticContext
{
    public bool WasCalled { get; private set; }

    public void Set(string propertyName, object? value, bool destructureObjects = false)
        => WasCalled = true;

    public void SetException(Exception exception) { }
}

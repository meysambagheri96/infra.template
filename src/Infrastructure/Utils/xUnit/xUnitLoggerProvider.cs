using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Campaign.Infrastructure.Utils.xUnit;

public sealed class xUnitLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly LoggerExternalScopeProvider _scopeProvider = new();

    public xUnitLoggerProvider(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new xUnitLogger(_testOutputHelper, _scopeProvider, categoryName);
    }

    public void Dispose()
    {
    }
}
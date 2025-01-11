namespace Campaign.Infrastructure.Utils.xUnit;

public class xUnitCustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    private readonly ITestOutputHelper _testOutputHelper;

    public xUnitCustomWebApplicationFactory(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("xUnit", "true");

        builder.UseEnvironment(Environments.Staging);
        
        base.ConfigureWebHost(builder);

        builder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.Services.Replace(ServiceDescriptor.Singleton<ILoggerProvider>(_ => 
                new xUnitLoggerProvider(_testOutputHelper)));
        });
    }
}
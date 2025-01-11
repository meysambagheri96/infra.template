namespace Campaign.Infrastructure.Utils.Serializers.Helpers;

/// <summary>
/// Input formatter (use it instead of [FromBody])
/// </summary>
public class SystemTextJsonModelBinderAttribute : ModelBinderAttribute
{
    public SystemTextJsonModelBinderAttribute() : base(typeof(SystemTextJsonModelBinderModelBinder))
    {
        BindingSource = BindingSource.Body;
    }
}

internal class SystemTextJsonModelBinderModelBinder : IModelBinder
{
    private readonly BodyModelBinder _bodyModelBinder;

    public SystemTextJsonModelBinderModelBinder(
        IHttpRequestStreamReaderFactory readerFactory,
        ILoggerFactory loggerFactory,
        IOptions<MvcOptions> options)
    {
        var formatters = options.Value.InputFormatters.ToList();

        int jsonPatchFormatterIndex = formatters.IndexOf(
            formatters.First(formatter => formatter is NewtonsoftJsonPatchInputFormatter));
        formatters.RemoveAt(jsonPatchFormatterIndex);

        int jsonFormatterIndex = formatters.IndexOf(
            formatters.First(formatter => formatter is NewtonsoftJsonInputFormatter));

        formatters[jsonFormatterIndex] = new SystemTextJsonInputFormatter(new JsonOptions
        {
            JsonSerializerOptions =
            {
                Converters = { CustomJsonSerializer.SystemTextJsonOptions.Converters.FirstOrDefault() },
                PropertyNameCaseInsensitive =
                    CustomJsonSerializer.SystemTextJsonOptions.PropertyNameCaseInsensitive,
                PropertyNamingPolicy = CustomJsonSerializer.SystemTextJsonOptions.PropertyNamingPolicy,
                Encoder = CustomJsonSerializer.SystemTextJsonOptions.Encoder
            }
        }, loggerFactory.CreateLogger<SystemTextJsonInputFormatter>());

        _bodyModelBinder = new BodyModelBinder(formatters, readerFactory, loggerFactory, options.Value);
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        return _bodyModelBinder.BindModelAsync(bindingContext);
    }
}
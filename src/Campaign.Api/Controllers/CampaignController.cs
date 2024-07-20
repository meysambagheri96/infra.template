using Extensions.Http.Mvc;
using Infra.Commands;
using Infra.Queries;

namespace Campaign.Api.Controllers;

public class CampaignController : BaseController
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;

    public CampaignController(ICommandProcessor commandProcessor, IQueryProcessor queryProcessor)
    {
        this._commandProcessor = commandProcessor;
        this._queryProcessor = queryProcessor;
    }
}

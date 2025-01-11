using Extensions.Http.Mvc;
using Infra.Commands;
using Infra.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Campaign.Api.Controllers;

[ApiController]
[Area("campaign")]
[Route("[area]/campaigns")]
public class CampaignController : BaseController
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ICommandProcessor _commandProcessor;

    public CampaignController(IQueryProcessor queryProcessor, ICommandProcessor commandProcessor)
    {
        _queryProcessor = queryProcessor;
        _commandProcessor = commandProcessor;
    }
}

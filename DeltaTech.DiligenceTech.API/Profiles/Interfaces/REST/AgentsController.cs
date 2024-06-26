﻿using System.Net.Mime;
using DeltaTech.DiligenceTech.API.Profiles.Domain.Model.Queries;
using DeltaTech.DiligenceTech.API.Profiles.Domain.Services;
using DeltaTech.DiligenceTech.API.Profiles.Interfaces.REST.Resources;
using DeltaTech.DiligenceTech.API.Profiles.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DeltaTech.DiligenceTech.API.Profiles.Interfaces.REST;

[ApiController]
[Route("api/v1/agents")]
[Produces(MediaTypeNames.Application.Json)]
public class AgentsController(IAgentCommandService agentCommandService, IAgentQueryService agentQueryService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAgent(AgentResource resource)
    {
        var createAgentCommand = CreateAgentCommandFromResourceAssembler.ToCommandFromResource(resource);
        var agent = await agentCommandService.Handle(createAgentCommand);
        if (agent is null) return BadRequest();
        var agentResource = AgentResourceFromEntityAssembler.ToResourceFromEntity(agent);
        
        // Proporciona el valor del parámetro "agentCode" para CreatedAtAction
        return CreatedAtAction(nameof(GetAgentByCode), new { agentCode = agent.Code }, agentResource);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAgents()
    {
        var getAllAgentsQuery = new GetAllAgentsQuery();
        var agents = await agentQueryService.Handle(getAllAgentsQuery);
        var agentsResources = agents.Select(AgentResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(agentsResources);
    }

    [HttpGet("{agentCode}")]
    public async Task<IActionResult> GetAgentByCode(string agentCode)
    {
        var getAgentByCodeQuery = new GetAgentByCodeQuery(agentCode);
        var agent = await agentQueryService.Handle(getAgentByCodeQuery);
        if (agent is null) return NotFound();
        var agentResource = AgentResourceFromEntityAssembler.ToResourceFromEntity(agent);
        return Ok(agentResource);
    }
}

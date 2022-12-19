﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Configuration;
using NotificationService.Models;
using Dapr;
namespace NotificationService.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : Controller
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly NotificationServiceConfiguration _config;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IHubContext<NotificationHub> hubContext, NotificationServiceConfiguration config, ILogger<OrdersController> logger)
    {
        _hubContext = hubContext;
        _config = config;
        _logger = logger;
    }

    [HttpPost]
    [Route("processed")]
    [Topic("orders", "processed_orders")]
    public async Task<IActionResult> OnOrderProcessedAsync([FromBody]CloudEvent<DispatchedOrder> e)
    {
        _logger.LogTrace("OnOrderProcessed invoked with {Subject} and {Type}", e.Subject, e.Type);
        _logger.LogTrace("OrderProcessed invoked for User {UserId} and order {OrderId}", e.Data.UserId, e.Data.OrderId);

        var group = _hubContext.Clients.Group(e.Data.UserId);
        if (group == null)
        {
            _logger.LogWarning("SignalR group with name {Name} not found, request will fail with 404", e.Data.UserId);

            return NotFound();
        }

        await group.SendAsync(_config.OnOrderProcessedMethodName, e.Data.OrderId.);
        return Ok();
    }
}

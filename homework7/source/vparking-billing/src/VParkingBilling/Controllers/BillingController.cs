using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Services.Abstractions;
using Services.Contracts;
using VParkingBilling.Models;

namespace VParkingBilling.Controllers;

/// <summary>
/// CRUD для клиентов
/// </summary>
[ApiController]
[Route("[controller]")]
public class BillingController(IBillingService service, ILogger<BillingController> logger, IMapper mapper)
   : ControllerBase
{
    private static readonly Counter ListRequestCount = Metrics.CreateCounter("vparking_billing_client_list_request_count", "Number of requests");
    private static readonly Counter AddedCounter = Metrics.CreateCounter("vparking_billing_client_added", "Number of added"); 
    private static readonly Gauge FreeMem = Metrics.CreateGauge("vparking_billing_client_free_mem", "free-mem");
    private static readonly Histogram LoadLatency = Metrics.CreateHistogram("vparking_billing_client_list_latency_1", "List latency hystogram",
        new HistogramConfiguration
        {
            Buckets = Histogram.LinearBuckets(start:0,width:0.1,count:10)
        });

    [HttpPut("pay")]
    public async Task<IActionResult> Payment([FromBody]decimal price, [FromHeader(Name = "X-Auth-Request-Preferred-Username")] string clientID)
    {
        logger.LogInformation($"Оплата: {clientID}:{price}");
        var dto = new PaymentDto
        {
            ClientID = clientID,
            Price = price
        };
        var result = await service.Pay(dto);
        return Ok(result);
    }

    [HttpPut()]
    public async Task<IActionResult> Change([FromBody()] AccountInputModel account,
        [FromHeader(Name = "X-Auth-Request-Preferred-Username")] string clientID)
    {
        var changeDto = new ChangeBallanceDto()
        {
            ClientID = clientID,
            Amount = account.Amount
        };
        var changed = await service.ChangeBalance(changeDto);
        return Ok(changed);
    }

    [HttpGet]
    public async Task<IActionResult> GetAmount([FromHeader(Name = "X-Auth-Request-Preferred-Username")] string clientId)
    {
        var account = await service.GetByClientId(clientId);
        if (account == null)
            return NotFound();
        return Ok(account.Amount);
    }
    
    /// <summary>
    /// Удаление карточки заказа
    /// </summary>
    /// <param name="id">Идентификатор карточки</param>
    /// <returns>Статус операции</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        logger.LogInformation($"Удаление карточки клиента по ID {id}");
        await service.Delete(id);
        return Ok();
    }
}
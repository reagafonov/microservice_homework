using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Services.Abstractions;
using Services.Contracts;
using VParkingNotification.Models;

namespace VParkingNotification.Controllers;

/// <summary>
/// CRUD для клиентов
/// </summary>
[ApiController]
[Route("[controller]")]
public class NotificationsController(INotificationService service, ILogger<NotificationsController> logger, IMapper mapper)
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

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromHeader(Name = "X-Auth-Request-Preferred-Username")] string clientId)
    {
        var notificationDtos = await service.GetNotifications();
        var enumerable = notificationDtos.
            Where(x=>x.ClientID == clientId);
        var notificationOutputModels = enumerable.Select(x=>mapper.Map<NotificationDto>(x));
        return Ok(notificationOutputModels);
    }
    
    
    

  
}
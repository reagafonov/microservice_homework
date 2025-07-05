using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Services.Abstractions;
using Services.Contracts;
using VParkingSettings.Models;

namespace VParkingSettings.Controllers;

/// <summary>
/// CRUD для клиентов
/// </summary>
[ApiController]
[Route("[controller]")]
public class OrdersController(IOrderService service, ILogger<OrdersController> logger, IMapper mapper)
   : ControllerBase
{
    private static readonly Counter ListRequestCount = Metrics.CreateCounter("vparking_settings_client_list_request_count", "Number of requests");
    private static readonly Counter AddedCounter = Metrics.CreateCounter("vparking_settings_client_added", "Number of added"); 
    private static readonly Gauge FreeMem = Metrics.CreateGauge("vparking_settings_client_free_mem", "free-mem");
    private static readonly Histogram LoadLatency = Metrics.CreateHistogram("vparking_settings_client_list_latency_1", "List latency hystogram",
        new HistogramConfiguration
        {
            Buckets = Histogram.LinearBuckets(start:0,width:0.1,count:10)
        });
    /// <summary>
    /// Получение карточки заказа
    /// </summary>
    /// <param name="id">Идентификатор заказа</param>
    /// <returns>Карточка заказа</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        logger.LogInformation($"Получение карточки клиента по ID {id}");
        return Ok(mapper.Map<OrderOutputModel>(await service.GetById(id)));
    }

    /// <summary>
    /// Добавление карточки заказа
    /// </summary>
    /// <param name="orderInputModel">Карточка заказа</param>
    /// <returns>Идентификатор созданной карточки заказа</returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody]OrderInputModel orderInputModel, 
        [FromHeader(Name = "X-Auth-Request-Preferred-Username")] string clientId,
        [FromHeader(Name="X-Auth-Request-Email")] string email)
    {
        var orderDto = mapper.Map<OrderDto>(orderInputModel);
        orderDto.ClientID = clientId;
        orderDto.Email = email;
        var okObjectResult = Ok(await service.Create(orderDto));
        AddedCounter.Inc();
        var memInfo = GC.GetGCMemoryInfo(GCKind.Any);

        FreeMem.Set(memInfo.TotalAvailableMemoryBytes - GC.GetTotalMemory(false));
        return okObjectResult;
    }

    /// <summary>
    /// Редактирование карточки заказа
    /// </summary>
    /// <param name="id">Идентификатор карточки заказа</param>
    /// <param name="orderInputModel">Новые значения полей карточки заказа</param>
    /// <returns>Статус операции</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody]OrderInputModel orderInputModel)
    {
        logger.LogInformation($"Редактирование карточки клиента по ID {id} данными {orderInputModel}");
        var result = await service.Update(id, mapper.Map<OrderDto>(orderInputModel));
        return Ok(result);
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

    /// <summary>
    /// Получение списка заказов 
    /// </summary>
    /// <param name="filter">фильтр</param>
    /// <param name="page">номер страницы</param>
    /// <param name="itemsPerPage">число записей на странице</param>
    /// <returns>Список карточек заказов</returns>
    [HttpGet("list/{page:int}/{itemsPerPage:int}/")]
    public async Task<IActionResult> GetList([FromQuery] OrderFilterModel filter, [FromRoute] int page = 1,
        [FromRoute] int itemsPerPage = 10)
    {
        var timer = new Stopwatch();
        timer.Start();
        var filterDto = mapper.Map<OrderFilterDto>(filter);
        var okObjectResult = Ok(mapper.Map<List<OrderOutputModel>>(await service.GetPaged(page, itemsPerPage, filterDto)));
        timer.Stop();
        ListRequestCount.Inc();
        LoadLatency.Observe((double)timer.ElapsedMilliseconds / 100);
        return okObjectResult;
    }
    
    /// <summary>
    /// Получение списка заказов 
    /// </summary>
    /// <param name="filter">фильтр</param>
    /// <returns>Список карточек заказов</returns>
    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] OrderFilterModel filter)
    {
        var filterDto = mapper.Map<OrderFilterDto>(filter);
        var clientDtos = await service.GetPaged(1, 10, filterDto);
        var clientOutputModels = mapper.Map<List<OrderOutputModel>>(clientDtos);
        return Ok(clientOutputModels);
    }
    
}
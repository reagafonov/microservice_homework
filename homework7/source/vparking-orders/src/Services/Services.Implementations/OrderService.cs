using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities;
using Services.Abstractions;
using Services.Contracts;
using Services.Repositories.Abstractions;
using Services.Repositories.Abstractions.Exceptions;

namespace Services.Implementations;

/// <summary>
/// Сервис работы с клиентами
/// </summary>
public class OrderService(
    IMapper mapper,
    IOrderRepository orderRepository,
    IValidateDto<OrderDto> authorDtoValidator,
    IHttpPayment httpPayment,
    INotificationAsyncMessage queueAsyncMessage) : IOrderService
{
    /// <summary>
    /// Получить список
    /// </summary>
    /// <param name="page">номер страницы</param>
    /// <param name="pageSize">объем страницы</param>
    /// <param name="filterDto">Фильтр</param>
    /// <returns>список клиентов</returns>
    public async Task<ICollection<OrderDto>> GetPaged(int page, int pageSize, OrderFilterDto filterDto)
    {
        var filter = mapper.Map<OrderFilter>(filterDto);
        ICollection<Order?> entities = await orderRepository.GetPagedAsync(page, pageSize, filter);
        return mapper.Map<ICollection<OrderDto>>(entities);
    }

    /// <summary>
    /// Получить
    /// </summary>
    /// <param name="id">идентификатор</param>
    /// <returns>ДТО клиента</returns>
    public async Task<OrderDto> GetById(Guid id)
    {
        var order = await orderRepository.GetAsync(id);
        return mapper.Map<OrderDto>(order);
    }

    /// <summary>
    /// Создать
    /// </summary>
    /// <param name="orderDto">ДТО клиента</param>
    /// <returns>идентификатор</returns>
    public async Task<Guid> Create(OrderDto orderDto)
    {
        authorDtoValidator.Validate(orderDto);
        var entity = mapper.Map<Order>(orderDto);
        entity.Id = Guid.NewGuid();
        var res = await orderRepository.AddAsync(entity);
        var addedDto = mapper.Map<OrderDto>(res);
        await orderRepository.SaveChangesAsync();

        var message = mapper.Map<Notification>(addedDto);
        
        var model = mapper.Map<PaymentModel>(addedDto);

        if (!await httpPayment.Pay(model))
        {
            res.IsPayed = false;
            orderRepository.Update(res);
            message.MessageType = Domain.Entities.NotificationType.Failure;
        }
        else
            message.MessageType = Domain.Entities.NotificationType.Success;

        await queueAsyncMessage.ProduceAsync(message);

        return res.Id;
    }

    /// <summary>
    /// Изменить
    /// </summary>
    /// <param name="id">идентификатор</param>
    /// <param name="orderDto">ДТО клиента</param>
    public async Task<OrderDto> Update(Guid id, OrderDto orderDto)
    {
            authorDtoValidator.Validate(orderDto);
            var entity = mapper.Map<Order>(orderDto);
            entity.Id = id;
            orderRepository.Update(entity);
            await orderRepository.SaveChangesAsync();

            return mapper.Map<OrderDto>(entity);
    }

    /// <summary>
    /// Удалить
    /// </summary>
    /// <param name="id">идентификатор</param>
    public async Task Delete(Guid id)
    {
            var order = await orderRepository.GetAsync(id);
            if (order is null)
                throw new ObjectNotFoundException(nameof(Order));
            order.IsDeleted = true;
            await orderRepository.SaveChangesAsync();
        }
}
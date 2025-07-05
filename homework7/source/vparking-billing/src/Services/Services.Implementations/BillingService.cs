using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Services.Abstractions;
using Services.Contracts;
using Services.Repositories.Abstractions;
using Services.Repositories.Abstractions.Exceptions;

namespace Services.Implementations;

/// <summary>
/// Сервис работы с клиентами
/// </summary>
public class BillingService(
    IMapper mapper,
    IAccountRepository accountRepository,
    IValidateDto<BillingDto> authorDtoValidator,
    ILogger<BillingService> logger) : IBillingService
{
    /// <summary>
    /// Оплата
    /// </summary>
    /// <param name="dto">Данные оплаты</param>
    /// <returns>Успешность оплаты</returns>
    public async Task<bool> Pay(PaymentDto dto)
    {
        logger.LogInformation("Paying billing request");
        var filter = mapper.Map<AccountFilter>(dto);
        var accounts = await accountRepository.GetPagedAsync(1, 1, filter);
        if (accounts == null || accounts.Count == 0 || accounts.Count > 1)
            return false;
        
        var account = accounts.Single();

        if (account.Amount < dto.Price)
        {
            logger.LogInformation($"Account {account.Id} does not have enough {dto.Price}");
            return false;
        }
        account.Amount -= dto.Price;
        accountRepository.Update(account);
        logger.LogInformation($"Остаток {account.Amount}");
        await accountRepository.SaveChangesAsync();
        logger.LogInformation($"Account {account.Id} has {account.Amount}");
        return true;

    }

    public async Task<bool> RegisterUser(NewUserDto userDto)
    {
        logger.LogInformation($"Регистрация {userDto.ClientID}");
        try
        {
            var filter = mapper.Map<AccountFilter>(userDto);
            var searchResult = await accountRepository.GetPagedAsync(1, 1, filter);
            if (!(searchResult == null || searchResult.Count == 0))
                return true;
            var account = mapper.Map<Account>(userDto);
            account.Id = Guid.NewGuid();
            await accountRepository.AddAsync(account);
            await accountRepository.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e,"Ошибка регистрации");
        }

        return false;
    }

    public async Task<bool> ChangeBalance(ChangeBallanceDto changeBallanceDto)
    {
        var filter = mapper.Map<AccountFilter>(changeBallanceDto);
        var accounts = await accountRepository.GetPagedAsync(1, 1, filter);
        Account account = null;
        if (accounts == null || accounts.Count == 0)
        {
            logger.LogInformation("Аккаунт пуст");
            account = new Account()
            {
                Id = Guid.NewGuid(),
                ClientID = changeBallanceDto.ClientID,
                Amount = changeBallanceDto.Amount,
            };
            await accountRepository.AddAsync(account);
            logger.LogInformation($"Записано {account.Amount}");
        }
        else
        {
            logger.LogInformation("");
            if (accounts.Count == 1)
                account = accounts.Single();
            else
            {
                logger.LogInformation("Аккаунт содержит более 1");
                return false;
            }
            account.Amount = changeBallanceDto.Amount;
            accountRepository.Update(account);
            logger.LogInformation($"Записано {account.Amount}");
        }
        await accountRepository.SaveChangesAsync();
        logger.LogInformation("Сохранено");
        return true;
    }

    /// <summary>
    /// Получить список
    /// </summary>
    /// <param name="page">номер страницы</param>
    /// <param name="pageSize">объем страницы</param>
    /// <param name="filterDto">Фильтр</param>
    /// <returns>список клиентов</returns>
    public async Task<ICollection<BillingDto>> GetPaged(int page, int pageSize, AccountFilterDto filterDto)
    {
            var filter = mapper.Map<AccountFilter>(filterDto);
            ICollection<Account?> entities = await accountRepository.GetPagedAsync(page, pageSize, filter);
            return mapper.Map<ICollection<BillingDto>>(entities);
        }

    /// <summary>
    /// Получить
    /// </summary>
    /// <param name="id">идентификатор</param>
    /// <returns>ДТО клиента</returns>
    public async Task<BillingDto> GetById(Guid id)
    {
            var order = await accountRepository.GetAsync(id);
            return mapper.Map<BillingDto>(order);
        }

    public async Task<BillingDto?> GetByClientId(string clientId)
    {
        var account = await accountRepository.GetByClientId(clientId);
        if (account == null)
            return null;
        var byClientId = mapper.Map<BillingDto>(account);
        return byClientId;
    }

    /// <summary>
    /// Создать
    /// </summary>
    /// <param name="billingDto">ДТО клиента</param>
    /// <returns>идентификатор</returns>
    public async Task<Guid> Create(BillingDto billingDto)
    {
            authorDtoValidator.Validate(billingDto);
            var entity = mapper.Map<Account>(billingDto);
            var res = await accountRepository.AddAsync(entity);
            await accountRepository.SaveChangesAsync();
            return res.Id;
        }

    /// <summary>
    /// Изменить
    /// </summary>
    /// <param name="id">идентификатор</param>
    /// <param name="billingDto">ДТО клиента</param>
    public async Task<BillingDto> Update(Guid id, BillingDto billingDto)
    {
            authorDtoValidator.Validate(billingDto);
            var entity = mapper.Map<Account>(billingDto);
            entity.Id = id;
            accountRepository.Update(entity);
            await accountRepository.SaveChangesAsync();

            return mapper.Map<BillingDto>(entity);
    }

    /// <summary>
    /// Удалить
    /// </summary>
    /// <param name="id">идентификатор</param>
    public async Task Delete(Guid id)
    {
            var order = await accountRepository.GetAsync(id);
            if (order is null)
                throw new ObjectNotFoundException(nameof(Account));
            order.IsDeleted = true;
            await accountRepository.SaveChangesAsync();
        }
}
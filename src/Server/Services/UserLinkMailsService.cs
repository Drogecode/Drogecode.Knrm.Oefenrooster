using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserLinkMailsService : DrogeService, IUserLinkedMailsService
{
    public UserLinkMailsService(
        ILogger<ScheduleService> logger,
        DataContext database,
        IMemoryCache memoryCache,
        IDateTimeProvider dateTimeProvider) : base(logger, database, memoryCache, dateTimeProvider)
    {
    }

    public async Task<PutUserLinkedMailResponse> PutUserLinkedMail(UserLinkedMail? userLinkedMail, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new PutUserLinkedMailResponse();

        if (userLinkedMail is not null)
        {
            var dbOld = await Database.UserLinkedMails.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.Email == userLinkedMail.Email, clt);
            if (dbOld is null)
            {
                var dbCount = await Database.UserLinkedMails.CountAsync(x => x.CustomerId == customerId && x.UserId == userId && x.DeletedOn == null, cancellationToken: clt);
                if (dbCount > 5)
                    result.Error = PutUserLinkedMailError.TooMany;
                else
                {
                    var dbLink = new DbUserLinkedMails()
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customerId,
                        UserId = userId,
                        Email = userLinkedMail.Email,
                        ActivateKey = SecretHelper.CreateSecret(11),
                        ActivateRequestedOn = DateTime.UtcNow,
                        IsActive = false,
                        IsEnabled = false
                    };
                    await Database.UserLinkedMails.AddAsync(dbLink, clt);
                    result.Success = await Database.SaveChangesAsync(clt) > 0;
                    if (result.Success)
                    {
                        result.NewId = dbLink.Id;
                        result.ActivateKey = dbLink.ActivateKey;
                    }
                }
            }
            else if (dbOld.DeletedOn is not null)
            {
                dbOld.DeletedOn = null;
                dbOld.DeletedBy = null;
                dbOld.ActivateKey = SecretHelper.CreateSecret(11);
                Database.UserLinkedMails.Update(dbOld);
                result.Success = await Database.SaveChangesAsync(clt) > 0;
                if (result.Success)
                {
                    result.NewId = dbOld.Id;
                    result.ActivateKey = dbOld.ActivateKey;
                }
            }
            else
            {
                result.Error = PutUserLinkedMailError.MailAlreadyExists;
            }
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<ValidateUserLinkedActivateKeyResponse> ValidateUserLinkedActivateKey(ValidateUserLinkedActivateKeyRequest body, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new ValidateUserLinkedActivateKeyResponse();

        var dbOld = await Database.UserLinkedMails.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.DeletedBy == null && x.Id == body.UserLinkedMailId, clt);
        if (dbOld is not null)
        {
            var update = false;
            if (dbOld.ActivationFailedAttempts > 5)
            {
                result.Error = ValidateUserLinkedActivateKeyError.TooManyTries;
            }
            else if (string.Compare(body.ActivationKey, dbOld.ActivateKey, StringComparison.OrdinalIgnoreCase) == 0)
            {
                dbOld.IsActive = true;
                dbOld.IsEnabled = true;
                update = true;
                result.Success = true;
            }
            else
            {
                dbOld.ActivationFailedAttempts++;
                update = true;
                result.Success = false;
            }

            if (update)
            {
                Database.UserLinkedMails.Update(dbOld);
                var saved = await Database.SaveChangesAsync(clt) > 0;
                if (result.Success && !saved)
                    result.Success = false;
            }
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<IsEnabledChangedResponse> IsEnabledChanged(IsEnabledChangedRequest body, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new IsEnabledChangedResponse();

        var dbOld = await Database.UserLinkedMails.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.Id == body.UserLinkedMailId && x.DeletedBy == null && x.IsActive,
            clt);
        if (dbOld is not null)
        {
            dbOld.IsEnabled = body.IsEnabled;
            Database.UserLinkedMails.Update(dbOld);
            result.Success = await Database.SaveChangesAsync(clt) > 0;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PatchUserLinkedMailResponse> PatchUserLinkedMail(UserLinkedMail userLinkedMail, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new PatchUserLinkedMailResponse();

        var dbOld = await Database.UserLinkedMails.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.Id == userLinkedMail.Id && x.DeletedOn == null, clt);
        if (dbOld is not null)
        {
            dbOld.Email = userLinkedMail.Email;
            dbOld.ActivateKey = SecretHelper.CreateSecret(11);
            dbOld.IsActive = false;
            dbOld.IsEnabled = false;
            dbOld.ActivateRequestedOn = DateTime.UtcNow;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<AllUserLinkedMailResponse> AllUserLinkedMail(int take, int skip, Guid userId, Guid customerId, bool cache, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var cacheKey = $"AllUserLinkedMail-{customerId}{userId}-{take}-{skip}";
        MemoryCache.TryGetValue(cacheKey, out AllUserLinkedMailResponse? result);
        if (result is not null && cache)
        {
            sw.Stop();
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }

        result = new AllUserLinkedMailResponse();
        var linksRequest = Database.UserLinkedMails.Where(x => x.CustomerId == customerId && x.UserId == userId && x.DeletedOn == null);
        var userMail = await Database.Users.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == userId && x.DeletedOn == null, cancellationToken: clt);
        result.TotalCount = await linksRequest.CountAsync(clt);
        result.UserLinkedMails = await linksRequest
            .OrderBy(x => x.Email)
            .ThenBy(x => x.ActivateRequestedOn)
            .Skip(skip)
            .Take(take)
            .Select(x => x.ToDrogecode())
            .ToListAsync(clt);
        if (userMail is not null)
        {
            result.UserLinkedMails.Add(new UserLinkedMail() { Email = userMail.Email, IsActive = true, IsEnabled = true, IsDrogeCodeUser = true });
        }

        result.Success = result.UserLinkedMails.Count != 0;

        MemoryCache.Set(cacheKey, result, CacheOptions);

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<DeleteResponse> DeleteUserLinkMail(Guid userId, Guid customerId, Guid id, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new DeleteResponse();
        
        var dbOld = await Database.UserLinkedMails.FirstOrDefaultAsync(x => x.CustomerId == customerId&& x.Id == id && x.DeletedOn == null, clt);
        if (dbOld is not null)
        {
            dbOld.DeletedOn = DateTime.UtcNow;
            dbOld.DeletedBy = userId;
            Database.UserLinkedMails.Update(dbOld);
            result.Success = await Database.SaveChangesAsync(clt) > 0;
        }
        
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}
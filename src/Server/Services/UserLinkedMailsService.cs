using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserLinkedMailsService : IUserLinkedMailsService
{
    private readonly ILogger<UserLinkedMailsService> _logger;
    private readonly Database.DataContext _database;

    public UserLinkedMailsService(ILogger<UserLinkedMailsService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<PutUserLinkedMailResponse> PutUserLinkedMail(UserLinkedMail? userLinkedMail, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutUserLinkedMailResponse();

        if (userLinkedMail is not null)
        {
            var dbOld = await _database.UserLinkedMails.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.Email == userLinkedMail.Email, clt);
            if (dbOld is null)
            {
                var dbCount = await _database.UserLinkedMails.CountAsync(x => x.CustomerId == customerId && x.UserId == userId, cancellationToken: clt);
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
                    await _database.UserLinkedMails.AddAsync(dbLink, clt);
                    result.Success = await _database.SaveChangesAsync(clt) > 0;
                    if (result.Success)
                    {
                        result.NewId = dbLink.Id;
                        result.ActivateKey = dbLink.ActivateKey;
                    }
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
        var sw = Stopwatch.StartNew();
        var result = new ValidateUserLinkedActivateKeyResponse();

        var dbOld = await _database.UserLinkedMails.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.DeletedBy == null && x.Id == body.UserLinkedMailId, clt);
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
                _database.UserLinkedMails.Update(dbOld);
                var saved = await _database.SaveChangesAsync(clt) > 0;
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
        var sw = Stopwatch.StartNew();
        var result = new IsEnabledChangedResponse();

        var dbOld = await _database.UserLinkedMails.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.Id == body.UserLinkedMailId && x.DeletedBy == null && x.IsActive,
            clt);
        if (dbOld is not null)
        {
            dbOld.IsEnabled = body.IsEnabled;
            _database.UserLinkedMails.Update(dbOld);
            result.Success = await _database.SaveChangesAsync(clt) > 0;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PatchUserLinkedMailResponse> PatchUserLinkedMail(UserLinkedMail userLinkedMail, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PatchUserLinkedMailResponse();

        var dbOld = await _database.UserLinkedMails.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.Id == userLinkedMail.Id, clt);
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

    public async Task<AllUserLinkedMailResponse> AllUserLinkedMail(int take, int skip, Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new AllUserLinkedMailResponse();

        var linksRequest = _database.UserLinkedMails.Where(x => x.CustomerId == customerId && x.UserId == userId && x.DeletedBy == null);
        var userMail = await _database.Users.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == userId && x.DeletedBy == null, cancellationToken: clt);
        result.TotalCount = await linksRequest.CountAsync(clt);
        result.UserLinkedMails = await linksRequest.Skip(skip).Take(take).Select(x => x.ToDrogecode()).ToListAsync(clt);
        if (userMail is not null)
            result.UserLinkedMails.Add(new UserLinkedMail() { Email = userMail.Email, IsActive = true, IsEnabled = true, IsDrogeCodeUser = true });
        result.Success = result.UserLinkedMails.Count != 0;

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}
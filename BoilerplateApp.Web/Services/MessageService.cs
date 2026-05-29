using BoilerplateApp.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace BoilerplateApp.Web.Services;

public sealed class MessageService(AppDbContext db, TimeProvider timeProvider) : IMessageService
{
    public async Task SubmitAsync(ContactMessage message, CancellationToken cancellationToken = default)
    {
        message.CreatedAt = timeProvider.GetUtcNow().UtcDateTime;

        db.ContactMessages.Add(message);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<List<ContactMessage>> GetMessagesAsync(CancellationToken cancellationToken = default)
    {
        return db.ContactMessages
            .AsNoTracking()
            .OrderByDescending(message => message.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
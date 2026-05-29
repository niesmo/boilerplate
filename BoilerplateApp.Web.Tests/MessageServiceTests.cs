using BoilerplateApp.Web.Data;
using BoilerplateApp.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace BoilerplateApp.Web.Tests;

public sealed class MessageServiceTests
{
    [Fact]
    public async Task SubmitAsync_persists_message_with_timestamp()
    {
        var fixedTime = new DateTimeOffset(2026, 5, 29, 12, 30, 0, TimeSpan.Zero);

        await using var db = CreateDbContext();
        var service = new MessageService(db, new FixedTimeProvider(fixedTime));

        var message = new ContactMessage
        {
            Name = "Ada Lovelace",
            Email = "ada@example.com",
            Message = "Hello there",
        };

        await service.SubmitAsync(message);

        var saved = await db.ContactMessages.SingleAsync();

        Assert.Equal("Ada Lovelace", saved.Name);
        Assert.Equal("ada@example.com", saved.Email);
        Assert.Equal("Hello there", saved.Message);
        Assert.Equal(fixedTime.UtcDateTime, saved.CreatedAt);
    }

    [Fact]
    public async Task GetMessagesAsync_returns_newest_first()
    {
        await using var db = CreateDbContext();
        db.ContactMessages.AddRange(
            new ContactMessage
            {
                Name = "Old",
                Email = "old@example.com",
                Message = "older message",
                CreatedAt = new DateTime(2026, 5, 28, 10, 0, 0, DateTimeKind.Utc),
            },
            new ContactMessage
            {
                Name = "New",
                Email = "new@example.com",
                Message = "newer message",
                CreatedAt = new DateTime(2026, 5, 29, 10, 0, 0, DateTimeKind.Utc),
            });
        await db.SaveChangesAsync();

        var service = new MessageService(db, new FixedTimeProvider(DateTimeOffset.UtcNow));

        var messages = await service.GetMessagesAsync();

        Assert.Collection(messages,
            message => Assert.Equal("New", message.Name),
            message => Assert.Equal("Old", message.Name));
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
using BoilerplateApp.Web.Data;

namespace BoilerplateApp.Web.Services;

public interface IMessageService
{
    Task SubmitAsync(ContactMessage message, CancellationToken cancellationToken = default);

    Task<List<ContactMessage>> GetMessagesAsync(CancellationToken cancellationToken = default);
}
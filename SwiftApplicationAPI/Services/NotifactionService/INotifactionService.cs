using SwiftApplicationAPI.Models.ParseMTModels;

namespace SwiftApplicationAPI.Services.NotifactionService
{
    public interface INotifactionService
    {
        public Task sendStatusMT799Message(MT103Model mT103Model, string Message, bool Successful);
    }
}

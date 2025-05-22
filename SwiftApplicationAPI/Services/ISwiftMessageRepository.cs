using SwiftApplicationAPI.Models.ParseMTModels;

namespace SwiftApplicationAPI.Services
{
    public interface ISwiftMessageRepository
    {
        public Task<int> Create(MT799Model mT799Model);
    }
}

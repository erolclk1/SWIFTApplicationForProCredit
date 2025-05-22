using SwiftApplicationAPI.Models;

namespace SwiftApplicationAPI.Services
{
    public interface ISwiftParserService<TModel>
    {
        Task <TModel> Parser(string swiffContent);
    }
}

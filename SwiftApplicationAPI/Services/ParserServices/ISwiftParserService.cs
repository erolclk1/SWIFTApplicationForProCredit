using SwiftApplicationAPI.Models;

namespace SwiftApplicationAPI.Services.ParserServices
{
    public interface ISwiftParserService<TModel>
    {
        Task<TModel> Parser(string swiffContent);
    }
}

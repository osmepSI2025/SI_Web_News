using SME_WEB_News.Models;

namespace SME_WEB_News.Services
{
    public interface ICallAPIService
    {
       Task<string> GetDataApiAsync(MapiInformationModels apiModels, object xdata);

    }
}

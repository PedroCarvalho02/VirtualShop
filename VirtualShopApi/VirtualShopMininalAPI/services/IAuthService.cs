using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace VirtualShopMinimalAPI.Services
{
    public interface IAuthService
    {
        Task<IResult> GoogleLogin(HttpContext http);
        Task<IResult> GoogleCallback(HttpContext http);
    }
}
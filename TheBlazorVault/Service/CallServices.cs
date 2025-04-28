using System.Security.Claims;
using Microsoft.Identity.Abstractions;
using TheApiDto;

namespace TheBlazorVault.Service
{
    public class CallServices(IDownstreamApi downstreamApi)
    {
        
        private List<VaultDto> _vaultsDtos = [];
        public async Task<List<VaultDto>> GetVaultsAsync(int UserId)
        {
            _vaultsDtos = await downstreamApi.CallApiForUserAsync<List<VaultDto>>("EntraIDAuthWebAPI", options =>
                {
                    options.HttpMethod = "GET";
                    options.RelativePath = $"api/Users/vaults/{UserId}";
                }) ?? [];
            
            return _vaultsDtos ;
        }
    } 
}

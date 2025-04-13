using System.Security.Claims;
using Microsoft.Identity.Abstractions;
using TheApiDto;

namespace TheBlazorVault.Service
{
    /// <summary>
    /// Fournit des méthodes pour récupérer les informations de l'utilisateur connecté
    /// en appelant l'API sécurisée.
    /// </summary>
    public class CallServices(IDownstreamApi downstreamApi)
    {
        
        private List<VaultDto> _vaultsDtos = [];
        public async Task<List<VaultDto>> GetVaultsAsync(Guid entraIdUser)
        {
            _vaultsDtos = await downstreamApi.CallApiForUserAsync<List<VaultDto>>("EntraIDAuthWebAPI", options =>
                {
                    options.HttpMethod = "GET";
                    options.RelativePath = $"vaults";
                }) ?? [];
            
            return _vaultsDtos ;
        }
    } 
}

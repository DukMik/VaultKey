using System.Security.Claims;
using System.Text;
using System.Text.Json;
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

        // je veut crée une entrée
        public async Task GetVaultsAsync(int vaultId, EntrieDtoCreation entrieDtoCreation)
        {
            var json = JsonSerializer.Serialize(entrieDtoCreation);
            var client = new HttpClient();
            
            var response = await client.PostAsync($"api/vaults/{vaultId}/entries", new StringContent(json, Encoding.UTF8, "application/json"));
            Console.WriteLine(response);
        }
    } 
}

using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Abstractions;
using TheApiDto;

namespace TheBlazorVault.Service
{
    /// <summary>
    /// classe que rassemble tout les appels a l'API
    /// </summary>
    /// <remarks>l'appelle via la downstream API est plus dangeureux car il fait sortir les information pour les réutiliser, en Blazor serveur ce n'est pas nécéssaire.</remarks>
    /// <param name="downstreamApi"></param>
    public class CallServices(IDownstreamApi downstreamApi)
    {
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        private List<VaultDto> _vaultsDtos = [];
        private List<EntrieDto> _entriesDtos = [];
        
        #region For users
        
        
        public async Task<List<VaultDto>> GetVaultsAsync()
        {
            _vaultsDtos = await downstreamApi.CallApiForUserAsync<List<VaultDto>>("EntraIDAuthWebAPI", options =>
                {
                    options.HttpMethod = "GET";
                    options.RelativePath = $"api/Users/vaults";
                }) ?? [];

                return _vaultsDtos;
        }

        public async Task<VaultDto> GetOneVaultAsync(int vaultId)
        {
            var vaultDto = await downstreamApi.CallApiForUserAsync<VaultDto>("EntraIDAuthWebAPI", options =>
            {
                options.HttpMethod = "GET";
                options.RelativePath = $"api/Users/vault/{vaultId}";
            });

            return vaultDto!;
        }

        public async Task<bool> IsConnectionValidAsync(int vaultId)
        {
            // Appel via DownstreamApi pour bénéficier de l'authentification et des jetons
            var result = await downstreamApi.CallApiForUserAsync<BoolResult>(
                "EntraIDAuthWebAPI",
                options =>
                {
                    options.HttpMethod = "GET";
                    options.RelativePath = $"api/Users/IsConnectionValid/{vaultId}";
                });
            return result?.Value ?? false;
        }

        #endregion

        #region For vaults

        // je veut crée un vault 
       
        
      
        public Task<HttpResponseMessage> CreateVaultAsync(VaultDtoCreation vaultDtoCreation)
            => downstreamApi.CallApiForUserAsync(                    
                "EntraIDAuthWebAPI",
                o =>
                {
                    o.HttpMethod   = "POST";
                    o.RelativePath = "api/vault";
                    o.CustomizeHttpRequestMessage = msg =>
                    {
                        msg.Content = JsonContent.Create(vaultDtoCreation);
                    };
                });

        public Task<HttpResponseMessage> DesactivateVaultAsync(int id,VaultDtoActivation vaultDtoActivation)
            => downstreamApi.CallApiForUserAsync(
                "EntraIDAuthWebAPI",
                o =>
                {
                    o.HttpMethod   = "PATCH";
                    o.RelativePath = $"api/Vault/{id}/activation";
                    o.CustomizeHttpRequestMessage = msg =>
                    {
                        msg.Content = JsonContent.Create(vaultDtoActivation);
                    };
                });



        public Task<HttpResponseMessage> CanEnterVaultAsync(int id,Byte[] vaultPassword)
            => downstreamApi.CallApiForUserAsync(
                "EntraIDAuthWebAPI",
                o =>
                {
                    o.HttpMethod = "POST";
                    o.RelativePath = $"api/vault/{id}/canEnter";
                    o.CustomizeHttpRequestMessage = msg =>
                    {
                        msg.Content = JsonContent.Create(vaultPassword);
                    };
                });


        #endregion

        #region For Entries

        public Task<HttpResponseMessage> AddEntryAsync(int vaultId, EntrieDtoCreation entrieDtoCreation)
            => downstreamApi.CallApiForUserAsync(
                "EntraIDAuthWebAPI",
                o =>
                {
                    o.HttpMethod = "POST";
                    o.RelativePath = $"api/vault/{vaultId}/entries";
                    o.CustomizeHttpRequestMessage = msg =>
                    {
                        msg.Content = JsonContent.Create(entrieDtoCreation);
                    };
                });       

        public async Task<List<EntrieDto>> GetEntriesAsync(int vaultId)
        {
            try
            {
                _entriesDtos = await downstreamApi.CallApiForUserAsync<List<EntrieDto>>(
                    "EntraIDAuthWebAPI",
                    options =>
                    {
                        options.HttpMethod = "GET";
                        options.RelativePath = $"/api/vault/{vaultId}/entries";
                    }) ?? [];

                return _entriesDtos;
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e);
                return [];
            } 
        }

        
        public async Task<List<EntrieDto>> GetEntriePasswordAsync(int entrieId)
        {
            _entriesDtos = await downstreamApi.CallApiForUserAsync<List<EntrieDto>>(
                "EntraIDAuthWebAPI",
                options =>
                {
                    options.HttpMethod = "GET";
                    options.RelativePath = $"/api/vault/{entrieId}/entries";
                }) ?? [];

            return _entriesDtos;
        }







  //     // est ce que j'update ?
  //     public Task<HttpResponseMessage> UpdateEntryAsync(int id, VaultDtoActivation vaultDtoActivation)
  //         => downstreamApi.CallApiForUserAsync(
  //             "EntraIDAuthWebAPI",
  //             o =>
  //             {
  //                 o.HttpMethod = "PATCH";
  //                 o.RelativePath = $"api/Vault/{id}/activation";
  //                 o.CustomizeHttpRequestMessage = msg =>
  //                 {
  //                     msg.Content = JsonContent.Create(vaultDtoActivation);
  //                 };
  //             });


      #endregion
    }



    public class BoolResult
    {
        public bool Value { get; set; }
    }
}


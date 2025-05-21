using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
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
        
        private List<VaultDto> _vaultsDtos = [];
        private List<EntrieDto> _entriesDtos = [];
        private readonly HttpClient http = new HttpClient();
        
        #region For users
        
        // récupérations des vault via la downstream api 
        // plus gourmand que l'appelle normal de l'api (de toute façon le user est contoler au controler de l'api)'
        public async Task<List<VaultDto>> GetVaultsAsync(int UserId)
        {
            _vaultsDtos = await downstreamApi.CallApiForUserAsync<List<VaultDto>>("EntraIDAuthWebAPI", options =>
                {
                    options.HttpMethod = "GET";
                    options.RelativePath = $"api/Users/vaults/{UserId}";
                }) ?? [];
            
            return _vaultsDtos ;
        }
        public async Task<VaultDto> GetOneVaultAsync(int VaultId)
        {
            var vaultDto = await downstreamApi.CallApiForUserAsync<VaultDto>("EntraIDAuthWebAPI", options =>
            {
                options.HttpMethod = "GET";
                options.RelativePath = $"api/Users/vault/{VaultId}";
            });

            return vaultDto!;
        }
        
        
        
        // async obligatoir mais pourquoi ?
        public async Task<int?> GetCurrentUserIdAsync()
        {
            var userDto = await downstreamApi.CallApiForUserAsync<UserDto>(
                "EntraIDAuthWebAPI",
                options =>
                {
                    options.HttpMethod = "GET";
                    options.RelativePath = "api/Users/user"; // <-- corresponde au [HttpGet("user")]
                });
            return userDto?.IdUser;
        }
        #endregion
        
        #region For vaults

        // je veut crée un vault 
        // apelle directe de l'api dans downsrtream api ce qui est moins gourmands et moins risquer
        
        // 1) Appel HTTP « brut » – plus léger, aucun jeton n’est ré‑émis côté client.
        public async Task<HttpResponseMessage> CreateVaultAsyncRest(VaultDtoCreation vaultDtoCreation)
            => await http.PostAsJsonAsync("api/vault", vaultDtoCreation);

        
        // 2) Même chose mais via DownstreamApi – MSAL ajoute access‑token + gestion erreur AAD.
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



        public Task<HttpResponseMessage> CanEnterVaultAsync(int id,Byte vaultPassword)
            => downstreamApi.CallApiForUserAsync(
                "EntraIDAuthWebAPI",
                o =>
                {
                    o.HttpMethod = "POST";
                    o.RelativePath = "api/vault/{id}/canEnter";
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
            _entriesDtos = await downstreamApi.CallApiForUserAsync<List<EntrieDto>>(
                "EntraIDAuthWebAPI",
                options =>
                {
                    options.HttpMethod = "GET";
                    options.RelativePath = $"/api/vault/{vaultId}/entries";
                }) ?? [];

            return _entriesDtos;
        }

        public async Task<List<EntrieDto>> GetEntriePasswordAsync(int EntrieId)
        {
            _entriesDtos = await downstreamApi.CallApiForUserAsync<List<EntrieDto>>(
                "EntraIDAuthWebAPI",
                options =>
                {
                    options.HttpMethod = "GET";
                    options.RelativePath = $"/api/vault/{EntrieId}/entries";
                }) ?? [];

            return _entriesDtos;
        }







        // est ce que j'update ?
        public Task<HttpResponseMessage> UpdateEntryAsync(int id, VaultDtoActivation vaultDtoActivation)
            => downstreamApi.CallApiForUserAsync(
                "EntraIDAuthWebAPI",
                o =>
                {
                    o.HttpMethod = "PATCH";
                    o.RelativePath = $"api/Vault/{id}/activation";
                    o.CustomizeHttpRequestMessage = msg =>
                    {
                        msg.Content = JsonContent.Create(vaultDtoActivation);
                    };
                });


        #endregion
    }





    //public class UserIdDto
    //{
    //    public int Value { get; set; }
    //}
}


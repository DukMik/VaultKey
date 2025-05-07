using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using TheApiDto;
using TheBlazorVault.Service;

namespace TheBlazorVault.Components.Pages;

public partial class VaultsPage
{
    #region Injected Services

    [Inject]
    private CallServices                 CallServices { get; set; }
    [Inject]
    private AuthenticationStateProvider  AuthStateProvider{ get; set; }
    [Inject]
    private NavigationManager            Nav{ get; set; }
    [Inject]
    private IJSRuntime                   JS{ get; set; }
    
    #endregion
    
    
    
    
   
    private List<VaultDto>?   Vaults;
   
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                var auth = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = auth.User;

                if (!user.Identity?.IsAuthenticated ?? true)
                {
                    Nav.NavigateTo("authentication/login");
                    return;
                }

                // Id user exposé par l'API dans un claim "IdUser"
                var idClaim = user.FindFirst("IdUser")?.Value;
        
                // appler une route pour chopper le userID
                // if (!int.TryParse(idClaim, out var userId))
                // {
                //     ErrorMessage = "Impossible de récupérer l'identifiant utilisateur.";
                //     return;
                // }

                var userId = 2; // a modifier par une route de récupération
                Vaults = await CallServices.GetVaultsAsync(userId);
                StateHasChanged();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    private void EnterVault()
    {
        throw new NotImplementedException();
    }

    private void DesactivateVault()
    {
        throw new NotImplementedException();
    }

    
}

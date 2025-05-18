using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using TheApiDto;
using TheBlazorVault.Service;
using TheBlazorVault.Components.Pages.Modules;

namespace TheBlazorVault.Components.Pages;

public partial class VaultsPage
{
    #region Injected Services

    [Inject]
    private CallServices                 CallServices { get; set; } = default!;
    [Inject]
    private AuthenticationStateProvider  AuthStateProvider{ get; set; } = default!;
    [Inject]
    private NavigationManager            Navigation { get; set; } = default!;
    [Inject]
    private IJSRuntime                   JS{ get; set; } = default!;

    #endregion

    private List<VaultDto>?   _vaults;
    private string?           _errorMessage;

    private VaultDtoCreation  _newVault = new VaultDtoCreation();
    
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
                    Navigation.NavigateTo("authentication/login");
                    return;
                }

                // Récupère l'IdUser depuis l'API (base de données)
                var userId = await CallServices.GetCurrentUserIdAsync();
                if (userId == null)
                {
                    _errorMessage = "Impossible de récupérer l'identifiant utilisateur.";
                    return;
                }

                _vaults = await CallServices.GetVaultsAsync(userId.Value);
                StateHasChanged();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    /* ------------  création d'un vault  ------------ */
    private async Task CreateVaultAsync(VaultDtoCreation vault)
    {
        try
        {
            _newVault = vault;
            _errorMessage = null;
            // NewVault.UserId = _userId;
            _newVault.DateCreated = DateTime.UtcNow;

            // les deux implémentations devrais fonctionner mais celui décommenter fonctionne pour sur  
            // var resp = await CallServices.CreateVaultAsyncRest(NewVault);
            var resp = await CallServices.CreateVaultAsync(vault);

            if (resp.IsSuccessStatusCode)
            {
                _newVault = new VaultDtoCreation();
                await OnInitializedAsync();
            }
            else
            {
                _errorMessage = $"Erreur {resp.StatusCode} : " + await resp.Content.ReadAsStringAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    /* ------------  entrer dans un vault  ------------ */
    private async void EnterVault(VaultDto clickedVault)
    {
        // Récupère l'IdUser depuis l'API (base de données)
        var userId = await CallServices.GetCurrentUserIdAsync();
        if (userId == null || userId.Value != clickedVault.UserId)
        {
            Navigation.NavigateTo("/unauthorized");
            return;
        }

        // Navigation vers EntriePage avec l'ID du vault en paramètre
        Navigation.NavigateTo($"/entries/{clickedVault.IdVault}");
    }

    
    
    
    /* ------------  desactiver un vault  ------------ */
    private async Task DesactivateVault(VaultDto clickedVault)
    {
        try
        {
            VaultDtoActivation vaultDtoActivation = new VaultDtoActivation();
            if (clickedVault.IsDesactivated)
            {
                vaultDtoActivation.IsDesactivated = false;
            }
            else if (!clickedVault.IsDesactivated)
            {
                vaultDtoActivation.IsDesactivated = true;
            }
        
            var resp = await CallServices.DesactivateVaultAsync(clickedVault.IdVault, vaultDtoActivation);

            if (resp.IsSuccessStatusCode)
            {
                _newVault = new VaultDtoCreation();
                await OnInitializedAsync();
            }
            else
            {
                _errorMessage = $"Erreur {resp.StatusCode} : " + await resp.Content.ReadAsStringAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

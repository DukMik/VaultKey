using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using TheApiDto;
using TheBlazorVault.Service;
using TheBlazorVault.Components.Pages.Modules;
using EntityFrameworkComm.EfModel.Models;

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

    private VaultDto _currentVault = new VaultDto();

    public bool IsAtemptConnecting { get; set; } = false;



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

                _vaults = await CallServices.GetVaultsAsync();
                
                StateHasChanged();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private void OpenDialog(VaultDto clickedVault)
    {
        IsAtemptConnecting = true;
        _currentVault = clickedVault;
        StateHasChanged();
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
            Navigation.NavigateTo("/vaults", true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    /* ------------  entrer dans un vault  ------------ */
    private async void EnterVault(Byte[] hashPassword)
    {
        try
        {
            HttpResponseMessage response = await CallServices.CanEnterVaultAsync(_currentVault.IdVault, hashPassword);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                bool canEnter = bool.Parse(resultJson);

                if (canEnter)
                {
                    // Navigation vers la page des entrées si autorisé
                    Navigation.NavigateTo($"/entries/{_currentVault.IdVault}");
                }
                else
                {
                    // Affiche page non autorisé si mot de passe incorrect
                    Navigation.NavigateTo("/vaults");
                    return;
                }
            }
            else
            {
                Navigation.NavigateTo("/vaults");
                return;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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
    
    private void Close()
    {
        IsAtemptConnecting = false;
        StateHasChanged();
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using TheApiDto;
using TheBlazorVault.Service;

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
    private IJSRuntime                   Js{ get; set; } = default!;

    #endregion

    private List<VaultDto>?   _vaults;
    private string?           _errorMessage;

    private VaultDtoCreation  _newVault = new();

    private VaultDto _currentVault = new();

    private bool IsAtemptConnecting { get; set; }



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
            _newVault.DateCreated = DateTime.UtcNow;
            
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
    private void EnterVault(byte[]? hashPassword)
    {
        try
        {

            if (hashPassword != null)
            {
                Navigation.NavigateTo($"/entries/{_currentVault.IdVault}");
            }
            else
                Navigation.NavigateTo("/vaults", true);
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

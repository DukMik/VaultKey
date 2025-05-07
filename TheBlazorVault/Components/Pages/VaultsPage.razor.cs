using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using TheApiDto;
using TheBlazorVault.Components.Dialogs;

namespace TheBlazorVault.Components.Pages;

public partial class VaultsPage
{
    /* ------------  données bindées au front  ------------ */
    private List<VaultDto>?   Vaults;
    private VaultDtoCreation  NewVault     = new();
    private string?           ErrorMessage;

    /* ------------  cycle de vie  ------------ */
    protected override async Task OnInitializedAsync()
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
    }

    /* ------------  création d'un vault  ------------ */
    private async Task CreateVaultAsync()
    {
        ErrorMessage = null;

        // Choisis l'une des deux implémentations pour tester
        // var resp = await CallServices.CreateVaultAsyncRest(NewVault);
        var resp = await CallServices.CreateVaultAsyncDap(NewVault);

        if (resp.IsSuccessStatusCode)
        {
            NewVault = new VaultDtoCreation();     // ré‑initialise le formulaire
            await OnInitializedAsync();            // recharge la liste
            StateHasChanged();
        }
        else
        {
            ErrorMessage = $"Erreur {resp.StatusCode} : " +
                           await resp.Content.ReadAsStringAsync();
        }
    }

    private async Task PromptPasswordAsync(TableRowClickEventArgs<VaultDto> e)
    {
        var parameters = new DialogParameters { ["Vault"] = e.Item };
        var dialog = DialogService.Show<PasswordDialog>("Mot de passe", parameters);

        var result = await dialog.Result;
        if (!result.Canceled)
            Nav.NavigateTo($"/vaults/{(int)result.Data}");
    }
}

// using Microsoft.AspNetCore.Components;
// using System.Security.Cryptography;
// using System.Text;
// using TheApiDto;
//
//
//
// namespace TheBlazorVault.Components.Pages
// {
//     public partial class VaultsPage
//     {
//         [Parameter]
//         public int UserId { get; set; }
//         private List<VaultDto> Vaults { get; set; } = new ();
//         
//         private string newVaultName;
//         private string? creationError;
//         
//         private Guid _entraIdUser = Guid.Parse("a64e2dda-b127-4219-8225-7ea10fdc0e0e") ;
//         protected override async Task OnInitializedAsync()
//         {
//            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
//            var user = authState.User;
//            if (user.Identity.IsAuthenticated)
//            {
//                //var contextDebug = await CallServices.GetContextDebugAsync();
//                //Console.WriteLine("Contexte Debug reçu depuis l'API : " + contextDebug);
//                
//                
//                // Vous pouvez logger ou afficher le nom de l'utilisateur par exemple
//                Console.WriteLine("Utilisateur authentifié : " + user.Identity.Name);
//                
//                
//                // Appel du service pour récupérer la liste des vaults de l'utilisateur connecté
//                Vaults = await CallServices.GetVaultsAsync(UserId);
//            }
//            else
//            {
//                NavigationManager.NavigateTo("authentication/login");
//            }
//         }
//         
//         private async Task CreateVault()
//         {
//            
//         }
//     }
//
//     
// }
//
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TheApiDto;

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
        if (!int.TryParse(idClaim, out var userId))
        {
            ErrorMessage = "Impossible de récupérer l'identifiant utilisateur.";
            return;
        }

        userId = 2;
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
}

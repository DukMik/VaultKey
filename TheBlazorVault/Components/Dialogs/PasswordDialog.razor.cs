using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using TheApiDto;

namespace TheBlazorVault.Components.Dialogs;

public partial class PasswordDialog
{
    /* --------- paramètres & DI --------- */
    [CascadingParameter] private MudDialogInstance Dialog { get; set; } = default!;
    [Parameter] public VaultDto Vault { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    /* --------- état interne --------- */
    private string? Password { get; set; }   // ← propriété (get; set;)
    private bool Error { get; set; }

    /* --------- actions --------- */
    private async Task ValidateAsync()
    {
        Error = false;

        bool ok = await JS.InvokeAsync<bool>(
            "vaultCrypto.check",
            Password,
            Vault.Salt,
            Vault.KeyHash);

        if (ok)
        {
            Dialog.Close(DialogResult.Ok(Vault.Id));
        }
        else
        {
            Error = true;
            StateHasChanged();         // rafraîchit pour afficher l'alerte
        }
    }

    private void Cancel() => Dialog.Cancel();
}

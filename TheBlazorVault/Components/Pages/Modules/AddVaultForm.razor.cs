using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TheApiDto;

namespace TheBlazorVault.Components.Pages.Modules;

public partial class AddVaultForm : ComponentBase
{
    [Inject]private IJSRuntime IjsRuntime { get; set; } = default!;
    private VaultDtoCreation _newvault = new();
    
    private string Password { get; set; }= "";
    private string Salt { get; set; } = "";
    private string Name  { get; set; } = "";
    
    [Parameter]
    public EventCallback<VaultDtoCreation> CreateVaultCallback { get; set; } = default ;
    
    
    
    public async void CreateMethodCallback()
    {
        var passwordHash = await IjsRuntime.InvokeAsync<Byte[]>("sha256HashString", Password);
        var saltHash = await IjsRuntime.InvokeAsync<Byte[]>("sha256HashString", Password);

        var newVault = new VaultDtoCreation()
        {
            VaultName = Name,
            KeyHash = passwordHash,
            Salt = saltHash,
            PrivateKey = Array.Empty<byte>()
        };
        
       await CreateVaultCallback.InvokeAsync(newVault);
    }
}
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
        var passwordHash = await IjsRuntime.InvokeAsync<byte[]>("getAndHashPassword");
        var salt = await IjsRuntime.InvokeAsync<byte[]>("generateSalt");  
        
        var newVault = new VaultDtoCreation()
        {
            VaultName = Name,
            KeyHash = passwordHash,
            Salt = salt,
            PrivateKey = Array.Empty<byte>()
        };
        
       await CreateVaultCallback.InvokeAsync(newVault);
    }
}
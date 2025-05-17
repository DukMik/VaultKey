using Microsoft.AspNetCore.Components;
using TheApiDto;

namespace TheBlazorVault.Components.Pages.Modules;

public partial class AddVaultForm : ComponentBase
{
    private VaultDtoCreation _newvault = new();
    
    [Parameter]
    public EventCallback<VaultDtoCreation> CreateVaultCallback { get; set; } = default ;
    
    
    
    public Task CreateMethodCallback()
    {
        Console.WriteLine(_newvault + " -- " + DateTime.Now.ToString());
        
        return CreateVaultCallback.InvokeAsync(_newvault);
    }
}
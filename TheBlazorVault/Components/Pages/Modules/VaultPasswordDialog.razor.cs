using Microsoft.AspNetCore.Components;
using TheApiDto;
using Microsoft.JSInterop;
using TheBlazorVault.Service;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TheBlazorVault.Components.Pages.Modules;

public partial class VaultPasswordDialog : ComponentBase
    
{
   

    [Inject] IJSRuntime IjsRuntime { get; set; } = default!;
    [Inject] CallServices CallServices { get; set; } = null!;

    [Parameter]
    public EventCallback<int> CloseCallback { get; set; }
    [Parameter]
    public EventCallback<byte[]> EnterCallback { get; set; }

    [Parameter]
    public VaultDto CurrentVault { get; set; } = new VaultDto();
    private string Password { get; set; } = "";
    
    
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    protected async void Enter()
    {
        
        var passwordHash = await IjsRuntime.InvokeAsync<Byte[]>("getAndHashPassword");

        HttpResponseMessage response = await CallServices.CanEnterVaultAsync(CurrentVault.IdVault, passwordHash);

        if (response.IsSuccessStatusCode)
        {
            var resultJson = await response.Content.ReadAsStringAsync();
            bool canEnter = bool.Parse(resultJson);
            if (canEnter)
            {
                var localKey = await IjsRuntime.InvokeAsync<Byte[]>("deriveKey");
                  
              
                await EnterCallback.InvokeAsync(passwordHash);

                Close();
            }
            else
            {
                Close();
            }
        }      
    }
    
    protected async void Close()
    {
        await CloseCallback.InvokeAsync();
    }
}
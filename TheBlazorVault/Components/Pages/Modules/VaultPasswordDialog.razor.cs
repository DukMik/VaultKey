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

    //protected override void OnInitialized()
    //{
    //    base.OnInitialized();
    //}

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    protected async void Enter()
    {
        
        var passwordHash = await IjsRuntime.InvokeAsync<byte[]>("getAndHashPassword"); // erreur atypique

        HttpResponseMessage response = await CallServices.CanEnterVaultAsync(CurrentVault.IdVault, passwordHash);

        if (response.IsSuccessStatusCode)
        {
            var resultJson = await response.Content.ReadAsStringAsync();
            bool canEnter = bool.Parse(resultJson);
            if (canEnter)
            {
                await IjsRuntime.InvokeAsync<byte[]>("deriveKey", CurrentVault.Salt );
                  
              
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
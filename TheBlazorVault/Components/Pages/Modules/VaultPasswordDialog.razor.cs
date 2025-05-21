using Microsoft.AspNetCore.Components;
using TheApiDto;
using Microsoft.JSInterop;

namespace TheBlazorVault.Components.Pages.Modules;

public partial class VaultPasswordDialog : ComponentBase
{
    [Inject] IJSRuntime IJSRuntime { get; set; } = default!;

    [Parameter]
    public EventCallback<int> CloseCallback { get; set; } = default!;
    [Parameter]
    public EventCallback<Byte[]> EnterCallback { get; set; } = default!;

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
        // appler le JS pour hacher 
        var passwordHash = await IJSRuntime.InvokeAsync<Byte[]>("sha256HashString", Password);

        EnterCallback.InvokeAsync(passwordHash);
    }
    
    protected async void Close()
    {
        CloseCallback.InvokeAsync();
    }
}
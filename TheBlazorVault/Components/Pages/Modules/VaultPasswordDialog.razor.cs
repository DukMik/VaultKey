using Microsoft.AspNetCore.Components;
using TheApiDto;
using Microsoft.JSInterop;

namespace TheBlazorVault.Components.Pages.Modules;

public partial class VaultPasswordDialog : ComponentBase
{
    [Inject] IJSRuntime IjsRuntime { get; set; } = default!;

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
        // todo : voir si de cette manière je ne communique pas avec le C# avant de passer dans le JS 
        var passwordHash = await IjsRuntime.InvokeAsync<Byte[]>("sha256HashString", Password);

        EnterCallback.InvokeAsync(passwordHash);
        Close();
    }
    
    protected async void Close()
    {
        CloseCallback.InvokeAsync();
    }
}
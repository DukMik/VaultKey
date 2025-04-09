using EntityFrameworkComm.EfModel.Models;
using Microsoft.AspNetCore.Components;
using TheBlazorVault.Service;
using TheApiDto;

namespace TheBlazorVault.Components.Pages;

public partial class AuthPage
{
    [Parameter]
    public string? Action { get; set; }   

    protected override void OnParametersSet()
    {
        if (Action == "logged-out")
        {
            NavigationManager.NavigateTo("/");
        }
    }
}

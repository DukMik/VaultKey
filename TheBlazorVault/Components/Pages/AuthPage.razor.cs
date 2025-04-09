using Microsoft.AspNetCore.Components;

namespace ArchiWebEntraID.Components.Pages;

public partial class AuthPage
{
    [Parameter]
    public string? Action { get; set; }

   // protected override void OnParametersSet()
   // {
   //     if (Action == "logged-out")
   //     {
   //         NavigationManager.NavigateTo("/");
   //     }
   // }
}

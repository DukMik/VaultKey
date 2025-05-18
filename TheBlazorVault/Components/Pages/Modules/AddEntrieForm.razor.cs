using Microsoft.AspNetCore.Components;
// using TheApiDto;
//
// namespace TheBlazorVault.Components.Pages.Modules;
//
// public partial class AddEntrieForm : ComponentBase
// {
// }

using Microsoft.AspNetCore.Components;
using TheApiDto;

namespace TheBlazorVault.Components.Pages.Modules;

public partial class AddEntrieForm : ComponentBase
{
    private EntrieDtoCreation _newEntrie = new();
    
    [Parameter]
    public EventCallback<EntrieDtoCreation> CreateEntrieCallback { get; set; } = default ;
    
    
    
    public Task CreateMethodCallback()
    {
        Console.WriteLine(_newEntrie + " -- " + DateTime.Now.ToString());
        
        return CreateEntrieCallback.InvokeAsync(_newEntrie);
    }
};
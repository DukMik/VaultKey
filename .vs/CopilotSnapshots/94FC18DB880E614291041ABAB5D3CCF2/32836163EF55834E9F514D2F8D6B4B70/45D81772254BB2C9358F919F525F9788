using Microsoft.AspNetCore.Components;
using TheApiDto;

namespace TheBlazorVault.Components.Pages;

public partial class EntriePage : ComponentBase
{
    private EntrieDtoCreation _newentrie = new();

    private EntrieDtoCreation NewVault
    {
        get => _newentrie;
        set
        {
            Console.WriteLine("entie a été modifié.");
            _newentrie = value;
        }
    }
}
using MudExRichTextEditor;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.MudExRichText;

public partial class MudExRichTextReader
{
    [Parameter, EditorRequired] public string? Text { get; set; }
    private MudExRichTextEdit? _editor;
}
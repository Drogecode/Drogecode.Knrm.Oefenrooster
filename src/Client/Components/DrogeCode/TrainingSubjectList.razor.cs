using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class TrainingSubjectList : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<TrainingSubjectList>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private TrainingTargetRepository? TrainingTargetRepository { get; set; }
    [Parameter] public bool TargetSetReadonly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public SelectionMode SelectionMode { get; set; }
    [Parameter] public RefreshModel? RefreshModel { get; set; }
    [Parameter] public RenderFragment<TrainingSubject>? SubjectBodyContent { get; set; }
    [Parameter] public RenderFragment<TrainingTarget>? TargetBodyContent { get; set; }
    [Parameter] public RenderFragment<TrainingSubject>? AddSubjectOrTargetContent { get; set; }

    private IReadOnlyCollection<Guid>? _selectedTargets;
    [Parameter] public EventCallback<IReadOnlyCollection<Guid>> SelectedTargetsChanged { get; set; }

    [Parameter]
    public IReadOnlyCollection<Guid>? SelectedTargets
    {
        get => _selectedTargets;
        set
        {
            if (Equals(_selectedTargets, value)) return;
            _selectedTargets = value;
            SelectedTargetsChanged.InvokeAsync(value);
        }
    }

    private readonly CancellationTokenSource _cls = new();
    private List<TrainingSubject>? _trainingSubjects;
    private string _searchTargetText = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (RefreshModel is not null)
            {
                RefreshModel.RefreshRequestedAsync += RefreshMeAsync;
            }

            _trainingSubjects = (await TrainingTargetRepository.AllTrainingTargets(_cls.Token))?.TrainingSubjects;
            StateHasChanged();
        }
    }

    private void OnSearchTarget(string search)
    {
        if (!string.IsNullOrEmpty(search) && _trainingSubjects is not null)
        {
            UpdateTrainingVisibilityBasedOnSearch(_trainingSubjects, search);
        }
    }

    private bool UpdateTrainingVisibilityBasedOnSearch(List<TrainingSubject> trainingSubjects, string search)
    {
        var isVisible = false;
        foreach (var subject in trainingSubjects)
        {
            subject.IsVisible = false;
            if (subject.TrainingTargets is { Count: > 0 })
            {
                foreach (var target in subject.TrainingTargets)
                {
                    if (target.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) == true || (SelectedTargets is not null && SelectedTargets.Contains(target.Id)))
                    {
                        target.IsVisible = true;
                        subject.IsVisible = true;
                        continue;
                    }

                    target.IsVisible = false;
                }
            }
            else
            {
                subject.IsVisible = subject.TrainingSubjects is not null && UpdateTrainingVisibilityBasedOnSearch(subject.TrainingSubjects, search);
            }

            if (subject.IsVisible)
            {
                isVisible = true;
            }
        }

        return isVisible;
    }

    private async Task RefreshMeAsync()
    {
        _trainingSubjects = (await TrainingTargetRepository.AllTrainingTargets(_cls.Token))?.TrainingSubjects;
        StateHasChanged();
    }

    public void Dispose()
    {
        if (RefreshModel is not null)
        {
            RefreshModel.RefreshRequestedAsync -= RefreshMeAsync;
        }

        _cls.Cancel();
    }
}
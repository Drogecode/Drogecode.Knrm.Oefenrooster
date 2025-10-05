using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages;

public sealed partial class LandingPage
{
    [Inject, NotNull] private IStringLocalizer<LandingPage>? L { get; set; }
    private const string CUSTOMER_NAME = "KNRM Huizen & Huizer Reddingsbrigade";
}
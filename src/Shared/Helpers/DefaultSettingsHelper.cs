﻿namespace Drogecode.Knrm.Oefenrooster.Shared.Helpers;

public static class DefaultSettingsHelper
{
    public const string CURRENT_VERSION = "#{VERSION_TOKEN}#";
    public static Guid KnrmHuizenId { get; } = new Guid("d9754755-b054-4a9c-a77f-da42a4009365");
    public static Guid KompasOefeningId { get; } = new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0");
    public static Guid IdTaco { get; } = new Guid("04093c7a-11e5-4887-af51-319ecc59efe0");

    public const int MAX_LENGTH_TRAINING_TITLE = 30;
}

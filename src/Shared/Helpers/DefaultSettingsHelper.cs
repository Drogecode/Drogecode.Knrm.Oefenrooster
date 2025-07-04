namespace Drogecode.Knrm.Oefenrooster.Shared.Helpers;

public static class DefaultSettingsHelper
{
    /// <summary>
    /// Current version, set by tag while compiling release code.
    /// </summary>
    public const string CURRENT_VERSION = "#{VERSION_TOKEN}#";

    /// <summary>
    /// Increase to show the update button when BUTTON_VERSION is on this number or above.
    /// </summary>
    public const int UPDATE_VERSION = 9; // Set on: v0.5.28
    
    // Does not show for versions below v0.5.0
    // Old versions, version when it was last on this UPDATE_VERSION
    // 8 = v0.5.28
    
    /// <summary>
    /// From what UPDATE_VERSION we should show the update button.
    /// </summary>
    public const int BUTTON_VERSION = 7; // v0.5.18 version when it was last on this UPDATE_VERSION
    // 7 = v0.5.18
    // 6 = v0.5.12
    // 5 = v0.4.32

    public static Guid KnrmHuizenId { get; } = new Guid("d9754755-b054-4a9c-a77f-da42a4009365");
    public static Guid SystemUser { get; } = new Guid("b4bcc37b-321a-4027-b02b-30630ad8f75e");
    public static Guid DefaultRoosterTuesday { get; } = new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54");
    public static Guid DefaultRoosterSaturdayMorning { get; } = new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978");
    public static Guid NwiId { get; } = new Guid("4589535c-9064-4448-bc01-3b5a00e9410d");

    public const int MAX_LENGTH_TRAINING_TITLE = 50;
    public const int MAX_LENGTH_TRAINING_DESCRIPTION = 1000;
    public const int MAX_LENGTH_TRAINING_GROUP_NAME = 30;
    public const int MAX_LENGTH_TRAINING_TYPE_NAME = 30;
    public const int MAX_LENGTH_MONTH_ITEM_TEXT = 150;
    public const int MAX_LENGTH_DAY_ITEM_TEXT = 50;
    public const int MAX_LENGTH_HOLIDAY_NAME = 50;
    public const int MAX_LENGTH_VEHICLE_NAME = 30;
    public const int MAX_LENGTH_VEHICLE_CODE = 10;
    public const int MAX_LENGTH_ROLE_EXTERNAL_ID = 50;
    public const int MAX_LENGTH_PRE_COM_AVAILABLE_TEXT = 25;
    public const int LENGTH_MAIL_ACTIVATION = 11;
}
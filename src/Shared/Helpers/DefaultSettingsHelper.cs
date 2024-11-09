namespace Drogecode.Knrm.Oefenrooster.Shared.Helpers;

public static class DefaultSettingsHelper
{
    /// <summary>
    /// Current version, set by tag while compiling release code.
    /// </summary>
    public const string CURRENT_VERSION = "#{VERSION_TOKEN}#";

    /// <summary>
    /// Increase to show update button for next update.
    /// </summary>
    public const int UPDATE_VERSION = 5;// v0.4.16

    /// <summary>
    /// From what UPDATE_VERSION should we show the update button.
    /// </summary>
    public const int BUTTON_VERSION = 4;// v0.3.76
    public static Guid KnrmHuizenId { get; } = new Guid("d9754755-b054-4a9c-a77f-da42a4009365");
    public static Guid KompasOefeningId { get; } = new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0");
    public static Guid KompasLeiderId { get; } = new Guid("d23de705-d950-4833-8b94-aa531022d450");
    public static Guid Oefening1op1Id { get; } = new Guid("52260d46-c748-4ffc-b94c-2baecacbfaf4");
    public static Guid IdTaco { get; } = new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"); // "04093c7a-11e5-4887-af51-319ecc59efe0"
    public static Guid SystemUser { get; } =new Guid("b4bcc37b-321a-4027-b02b-30630ad8f75e");
    public static Guid DefaultRoosterTuesday { get; } = new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54");
    public static Guid DefaultRoosterSaturdayMorning { get; } = new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978");
    public static Guid NwiId { get; } =new Guid("4589535c-9064-4448-bc01-3b5a00e9410d");

    public const int MAX_LENGTH_TRAINING_TITLE = 50;
    public const int MAX_LENGTH_TRAINING_DESCRIPTION = 1000;
    public const int MAX_LENGTH_TRAINING_GROUP_NAME = 30;
    public const int MAX_LENGTH_MONTH_ITEM_TEXT = 150;
    public const int MAX_LENGTH_DAY_ITEM_TEXT = 50;
    public const int MAX_LENGTH_HOLIDAY_NAME = 50;
    public const int MAX_LENGTH_VEHICLE_NAME = 30;
    public const int MAX_LENGTH_VEHICLE_CODE = 10;
    public const int LENGTH_MAIL_ACTIVATION = 11;
}

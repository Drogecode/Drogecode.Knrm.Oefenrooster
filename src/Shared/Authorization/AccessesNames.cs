namespace Drogecode.Knrm.Oefenrooster.Shared.Authorization;

// When adding a new role, the role should be added to the model, mapper and the UI
// (Could be automated in the future)
public struct AccessesNames
{
    // Can not be set by configuration


    public const string GROUP_NO_READ = "ignore";
    public const string AUTH_Taco = "isSuperGlobalAdmin";
    public const string AUTH_Hide = "do_not_show";


    public const string GROUP_CONFIGURATION= "configuration";
    public const string AUTH_configure_training_types = "configure_training-types";
    public const string AUTH_configure_user_roles = "configure_user_roles";

    public const string GROUP_SCHEDULER = "scheduler";
    public const string AUTH_scheduler = "scheduler";
    public const string AUTH_scheduler_in_table_view = "scheduler_table";
    public const string AUTH_scheduler_history = "scheduler_history";
    public const string AUTH_scheduler_edit_past = "scheduler_past"; // more than 3 days ago.
    public const string AUTH_scheduler_dayitem = "scheduler_dayitem";
    public const string AUTH_scheduler_monthitem = "scheduler_monthitem";
    public const string AUTH_scheduler_other_user = "scheduler_other";

    public const string GROUP_USER = "user";
    public const string AUTH_users_details = "users_details";
    public const string AUTH_users_counter = "users_counter";
    public const string AUTH_users_settigns = "users_settings";

    public const string GROUP_DASHBOARD = "dashboard";
    public const string AUTH_action_history_full = "full_action_history";
    public const string AUTH_training_history_full = "full_training_history";
    public const string AUTH_dashboard_Statistics = "full_dashboard_statistics2";
    public const string AUTH_dashboard_holidays = "dashboard_hol";
    public const string AUTH_dashboard_qr = "dashboard_qr";

    public const string GROUP_GLOBAL = "global";
    public const string AUTH_show_c_footer = "showCfooter";
}

public struct AccessesSettings
{
    public const int AUTH_scheduler_edit_past_days = -3;
}
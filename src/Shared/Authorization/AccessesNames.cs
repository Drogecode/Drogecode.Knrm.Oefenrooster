// ReSharper disable InconsistentNaming

namespace Drogecode.Knrm.Oefenrooster.Shared.Authorization;

// When adding a new role, the role should be added to the model, mapper and the UI
// After editing this file, run "Drogecode.Knrm.Oefenrooster.CodeGenerator" to update related files.
public struct AccessesNames
{
    // Cannot be set by configuration
    public const string GROUP_NO_READ = "ignore";
    public const string AUTH_super_user = "isSuperGlobalAdmin";
    public const string AUTH_Hide = "do_not_show";
    public const string AUTH_External = "external";

    public const string GROUP_GLOBAL = "global";
    public const string AUTH_basic_access = "basic_access";
    public const string AUTH_show_c_footer = "showCfooter";

    public const string GROUP_CONFIGURATION = "Configuration";
    public const string AUTH_configure_global_all = "configure_global_all";
    public const string AUTH_configure_training_types = "configure_training-types";
    public const string AUTH_configure_user_roles = "configure_user_roles";
    public const string AUTH_configure_user_functions = "configure_user_functions";
    public const string AUTH_configure_default_schedule = "configure_default_schedule";
    public const string AUTH_configure_vehicles = "configure_vehicles";

    public const string GROUP_SCHEDULER = "Scheduler";
    public const string AUTH_scheduler = "scheduler";
    public const string AUTH_scheduler_add = "scheduler_add";
    public const string AUTH_scheduler_delete = "scheduler_delete";
    public const string AUTH_scheduler_delay_sync = "scheduler_delay_sync";
    public const string AUTH_scheduler_in_table_view = "scheduler_table";
    public const string AUTH_scheduler_history = "scheduler_history";
    public const string AUTH_scheduler_edit_past = "scheduler_past"; // more than 3 days ago.
    public const string AUTH_scheduler_dayitem = "scheduler_dayitem";
    public const string AUTH_scheduler_monthitem = "scheduler_monthitem";
    public const string AUTH_scheduler_self = "scheduler_self";
    public const string AUTH_scheduler_other = "scheduler_other";
    public const string AUTH_scheduler_remaining_days_unavailable = "scheduler_remaining_unavailable";
    public const string AUTH_scheduler_description_edit = "scheduler_description_edit";
    public const string AUTH_scheduler_description_read = "scheduler_description_read";
    public const string AUTH_scheduler_target_set = "WIP_scheduler_target_set"; // ToDo: Remove WIP_ when ready
    
    public const string GROUP_TARGET = "Target";
    public const string AUTH_target_read = "WIP_target_read"; // ToDo: Remove WIP_ when ready
    public const string AUTH_target_edit = "WIP_target_edit"; // ToDo: Remove WIP_ when ready
    public const string AUTH_target_user_read = "WIP_target_user_read"; // ToDo: Remove WIP_ when ready
    public const string AUTH_target_user_rate = "WIP_target_user_rate"; // ToDo: Remove WIP_ when ready

    public const string GROUP_USER = "User";
    public const string AUTH_users_details = "users_details";
    public const string AUTH_users_delete = "users_delete";
    public const string AUTH_users_counter = "users_counter";
    public const string AUTH_users_settings = "users_settings";
    public const string AUTH_users_add_role = "users_add_role";
    
    public const string GROUP_DASHBOARD = "Dashboard";
    public const string AUTH_action_history_full = "full_action_history";
    public const string AUTH_training_history_full = "full_training_history";
    public const string AUTH_action_search = "action_search";
    public const string AUTH_action_share = "action_share";
    public const string AUTH_dashboard_Statistics = "full_dashboard_statistics";
    public const string AUTH_dashboard_Statistics_user_tabel = "full_dashboard_statistics_ut";
    public const string AUTH_dashboard_holidays = "dashboard_hol";

    public const string GROUP_MAIL = "Mail";
    public const string AUTH_mail_invite_external = "mail_invite_external";

    public const string GROUP_PRECOM = "PreCom";
    public const string AUTH_precom_problems = "precom_problems";
    public const string AUTH_precom_manual = "precom_manual";
    public const string AUTH_precom_sync_calendar = "precom_sync_calendar";
}

public struct AccessesSettings
{
    public const int AUTH_scheduler_edit_past_days = -3;
}
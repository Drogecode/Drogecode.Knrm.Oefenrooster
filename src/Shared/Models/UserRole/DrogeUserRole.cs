// <auto-generated>
// This code was generated by Drogecode.Knrm.Oefenrooster.CodeGenerator.
// If required, update the tool; not this file.
// 
// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

public class DrogeUserRole
{
    public Guid Id { get; set; }
    public string? ExternalId { get; set; }
    public string? Name { get; set; }


    // Group: GROUP_GLOBAL
    public bool AUTH_basic_access { get; set; }
    public bool AUTH_show_c_footer { get; set; }

    // Group: GROUP_CONFIGURATION
    public bool AUTH_configure_training_types { get; set; }
    public bool AUTH_configure_user_roles { get; set; }
    public bool AUTH_configure_user_functions { get; set; }
    public bool AUTH_configure_default_schedule { get; set; }
    public bool AUTH_configure_vehicles { get; set; }

    // Group: GROUP_SCHEDULER
    public bool AUTH_scheduler { get; set; }
    public bool AUTH_scheduler_in_table_view { get; set; }
    public bool AUTH_scheduler_history { get; set; }
    public bool AUTH_scheduler_edit_past { get; set; }
    public bool AUTH_scheduler_dayitem { get; set; }
    public bool AUTH_scheduler_monthitem { get; set; }
    public bool AUTH_scheduler_other_user { get; set; }
    public bool AUTH_scheduler_description_edit { get; set; }
    public bool AUTH_scheduler_description_read { get; set; }

    // Group: GROUP_USER
    public bool AUTH_users_details { get; set; }
    public bool AUTH_users_counter { get; set; }
    public bool AUTH_users_settigns { get; set; }

    // Group: GROUP_DASHBOARD
    public bool AUTH_action_history_full { get; set; }
    public bool AUTH_training_history_full { get; set; }
    public bool AUTH_action_search { get; set; }
    public bool AUTH_dashboard_Statistics { get; set; }
    public bool AUTH_dashboard_Statistics_user_tabel { get; set; }
    public bool AUTH_dashboard_holidays { get; set; }
    public bool AUTH_dashboard_qr { get; set; }

    // Group: GROUP_MAIL
    public bool AUTH_mail_invite_external { get; set; }

    // Group: GROUP_PRECOM
    public bool AUTH_precom_manual { get; set; }
}

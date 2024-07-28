namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

public class DrogeUserRole
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    
    public bool AUTH_configure_training_types { get; set; }
    
    public bool AUTH_scheduler { get; set; }
    public bool AUTH_scheduler_in_table_view { get; set; }
    public bool AUTH_scheduler_history { get; set; }
    public bool AUTH_scheduler_edit_past { get; set; }
    public bool AUTH_scheduler_dayitem { get; set; }
    public bool AUTH_scheduler_monthitem { get; set; }
    public bool AUTH_scheduler_other_user { get; set; }
    
    public bool AUTH_users_details { get; set; }
    public bool AUTH_users_counter { get; set; }
    public bool AUTH_users_settigns { get; set; }
    
    public bool AUTH_action_history_full { get; set; }
    public bool AUTH_training_history_full { get; set; }
    public bool AUTH_dashboard_Statistics { get; set; }
    public bool AUTH_dashboard_holidays { get; set; }
    public bool AUTH_dashboard_qr { get; set; }
    
    public bool AUTH_show_c_footer { get; set; }
}
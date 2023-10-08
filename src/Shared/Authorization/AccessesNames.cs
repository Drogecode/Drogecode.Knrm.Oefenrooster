using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Authorization;

public static class AccessesNames
{
    public const string AUTH_Taco = "isSuperGlobalAdmin";

    public const string AUTH_configure_training_types = "configure_training-types";

    public const string AUTH_scheduler = "scheduler";
    public const string AUTH_scheduler_in_table_view = "scheduler_table";
    public const string AUTH_scheduler_history = "scheduler_history";

    public const string AUTH_users_details = "users_details";
    public const string AUTH_users_counter = "users_counter";

    public const string AUTH_action_history_full = "full_action_history";
    public const string AUTH_training_history_full = "full_training_history";
}

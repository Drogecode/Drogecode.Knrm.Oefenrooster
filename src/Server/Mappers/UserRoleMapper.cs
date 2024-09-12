// <auto-generated>
// This code was generated by Drogecode.Knrm.Oefenrooster.CodeGenerator.
// If required, update the tool; not this file.
// 
// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>

using System;
using System.Text;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers
{
    internal static class UserRoleMapper
    {
        public static DrogeUserRole ToDrogeUserRole(this DbUserRoles dbUserRoles)
        {
            var drogeUserRole = new DrogeUserRole
            {
                Id = dbUserRoles.Id,
                ExternalId = dbUserRoles.ExternalId,
                Name = dbUserRoles.Name
            };

            var roles = dbUserRoles.Accesses.Split(',');

            foreach (var role in roles)
            {
                switch (role)
                {
                    case AccessesNames.AUTH_configure_training_types:
                        drogeUserRole.AUTH_configure_training_types = true;
                        break;
                    case AccessesNames.AUTH_configure_user_roles:
                        drogeUserRole.AUTH_configure_user_roles = true;
                        break;
                    case AccessesNames.AUTH_configure_default_schedule:
                        drogeUserRole.AUTH_configure_default_schedule = true;
                        break;
                    case AccessesNames.AUTH_scheduler:
                        drogeUserRole.AUTH_scheduler = true;
                        break;
                    case AccessesNames.AUTH_scheduler_in_table_view:
                        drogeUserRole.AUTH_scheduler_in_table_view = true;
                        break;
                    case AccessesNames.AUTH_scheduler_history:
                        drogeUserRole.AUTH_scheduler_history = true;
                        break;
                    case AccessesNames.AUTH_scheduler_edit_past:
                        drogeUserRole.AUTH_scheduler_edit_past = true;
                        break;
                    case AccessesNames.AUTH_scheduler_dayitem:
                        drogeUserRole.AUTH_scheduler_dayitem = true;
                        break;
                    case AccessesNames.AUTH_scheduler_monthitem:
                        drogeUserRole.AUTH_scheduler_monthitem = true;
                        break;
                    case AccessesNames.AUTH_scheduler_other_user:
                        drogeUserRole.AUTH_scheduler_other_user = true;
                        break;
                    case AccessesNames.AUTH_users_details:
                        drogeUserRole.AUTH_users_details = true;
                        break;
                    case AccessesNames.AUTH_users_counter:
                        drogeUserRole.AUTH_users_counter = true;
                        break;
                    case AccessesNames.AUTH_users_settigns:
                        drogeUserRole.AUTH_users_settigns = true;
                        break;
                    case AccessesNames.AUTH_action_history_full:
                        drogeUserRole.AUTH_action_history_full = true;
                        break;
                    case AccessesNames.AUTH_training_history_full:
                        drogeUserRole.AUTH_training_history_full = true;
                        break;
                    case AccessesNames.AUTH_dashboard_Statistics:
                        drogeUserRole.AUTH_dashboard_Statistics = true;
                        break;
                    case AccessesNames.AUTH_dashboard_holidays:
                        drogeUserRole.AUTH_dashboard_holidays = true;
                        break;
                    case AccessesNames.AUTH_dashboard_qr:
                        drogeUserRole.AUTH_dashboard_qr = true;
                        break;
                    case AccessesNames.AUTH_show_c_footer:
                        drogeUserRole.AUTH_show_c_footer = true;
                        break;
                }
            }

            return drogeUserRole;
        }

        public static DbUserRoles ToDb(this DrogeUserRole userRole, Guid customerId)
        {
            var dbUserRole = new DbUserRoles
            {
                Id = userRole.Id,
                ExternalId = userRole.ExternalId,
                CustomerId = customerId,
                Name = userRole.Name
            };

            var sb = new StringBuilder();
            if (userRole.AUTH_configure_training_types)
                sb.Append(AccessesNames.AUTH_configure_training_types).Append(',');
            if (userRole.AUTH_configure_user_roles)
                sb.Append(AccessesNames.AUTH_configure_user_roles).Append(',');
            if (userRole.AUTH_configure_default_schedule)
                sb.Append(AccessesNames.AUTH_configure_default_schedule).Append(',');
            if (userRole.AUTH_scheduler)
                sb.Append(AccessesNames.AUTH_scheduler).Append(',');
            if (userRole.AUTH_scheduler_in_table_view)
                sb.Append(AccessesNames.AUTH_scheduler_in_table_view).Append(',');
            if (userRole.AUTH_scheduler_history)
                sb.Append(AccessesNames.AUTH_scheduler_history).Append(',');
            if (userRole.AUTH_scheduler_edit_past)
                sb.Append(AccessesNames.AUTH_scheduler_edit_past).Append(',');
            if (userRole.AUTH_scheduler_dayitem)
                sb.Append(AccessesNames.AUTH_scheduler_dayitem).Append(',');
            if (userRole.AUTH_scheduler_monthitem)
                sb.Append(AccessesNames.AUTH_scheduler_monthitem).Append(',');
            if (userRole.AUTH_scheduler_other_user)
                sb.Append(AccessesNames.AUTH_scheduler_other_user).Append(',');
            if (userRole.AUTH_users_details)
                sb.Append(AccessesNames.AUTH_users_details).Append(',');
            if (userRole.AUTH_users_counter)
                sb.Append(AccessesNames.AUTH_users_counter).Append(',');
            if (userRole.AUTH_users_settigns)
                sb.Append(AccessesNames.AUTH_users_settigns).Append(',');
            if (userRole.AUTH_action_history_full)
                sb.Append(AccessesNames.AUTH_action_history_full).Append(',');
            if (userRole.AUTH_training_history_full)
                sb.Append(AccessesNames.AUTH_training_history_full).Append(',');
            if (userRole.AUTH_dashboard_Statistics)
                sb.Append(AccessesNames.AUTH_dashboard_Statistics).Append(',');
            if (userRole.AUTH_dashboard_holidays)
                sb.Append(AccessesNames.AUTH_dashboard_holidays).Append(',');
            if (userRole.AUTH_dashboard_qr)
                sb.Append(AccessesNames.AUTH_dashboard_qr).Append(',');
            if (userRole.AUTH_show_c_footer)
                sb.Append(AccessesNames.AUTH_show_c_footer).Append(',');

            dbUserRole.Accesses = sb.ToString().Trim(',');

            return dbUserRole;
        }
    }
}

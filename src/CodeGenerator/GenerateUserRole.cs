﻿using System.Reflection;
using System.Text;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;

namespace Drogecode.Knrm.Oefenrooster.CodeGenerator;

public static class GenerateUserRole
{
    public static void Start()
    {
        // List of cases for access names
        var accessNames = typeof(AccessesNames).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field.DeclaringType == typeof(AccessesNames))
            .ToArray();
        var generatedCode = GenerateUserRoleMapper(accessNames);
        var filePath = "../../../../Server/Mappers/UserRoleMapper.cs";
        SaveToFile(filePath, generatedCode);
        var generatedBlazorPage = GenerateBlazorPage(accessNames);
        filePath = "../../../../Client/Pages/Configuration/UserRolesEdit.razor";
        SaveToFile(filePath, generatedBlazorPage);
        var generatedObject = GenerateObject(accessNames);
        filePath = "../../../../Shared/Models/UserRole/DrogeUserRole.cs";
        SaveToFile(filePath, generatedObject);
    }

    private static string GenerateUserRoleMapper(FieldInfo[] accessNames)
    {
        var sb = new StringBuilder();

        // Warning Comment
        sb.AppendLine("// <auto-generated>");
        sb.AppendLine("// This code was generated by Drogecode.Knrm.Oefenrooster.CodeGenerator.");
        sb.AppendLine("// If required, update the tool; not this file.");
        sb.AppendLine("// ");
        sb.AppendLine("// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.");
        sb.AppendLine("// </auto-generated>");
        sb.AppendLine();

        // Namespace and Usings
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Text;");
        sb.AppendLine("using Drogecode.Knrm.Oefenrooster.Server.Database.Models;");
        sb.AppendLine("using Drogecode.Knrm.Oefenrooster.Shared.Authorization;");
        sb.AppendLine("using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;");
        sb.AppendLine();
        sb.AppendLine("namespace Drogecode.Knrm.Oefenrooster.Server.Mappers");
        sb.AppendLine("{");

        // Internal static class definition
        sb.AppendLine("    internal static class UserRoleMapper");
        sb.AppendLine("    {");

        // ToDrogeUserRole method
        sb.AppendLine("        public static DrogeUserRole ToDrogeUserRole(this DbUserRoles dbUserRoles)");
        sb.AppendLine("        {");
        sb.AppendLine("            var drogeUserRole = new DrogeUserRole");
        sb.AppendLine("            {");
        sb.AppendLine("                Id = dbUserRoles.Id,");
        sb.AppendLine("                ExternalId = dbUserRoles.ExternalId,");
        sb.AppendLine("                Name = dbUserRoles.Name");
        sb.AppendLine("            };");
        sb.AppendLine();
        sb.AppendLine("            var roles = dbUserRoles.Accesses.Split(',');");
        sb.AppendLine();
        sb.AppendLine("            foreach (var role in roles)");
        sb.AppendLine("            {");
        sb.AppendLine("                switch (role)");
        sb.AppendLine("                {");


        var group = string.Empty;
        foreach (var field in accessNames)
        {
            string accessName = field.Name;
            if (accessName.StartsWith("GROUP_"))
            {
                group = accessName;
            }
            else if (!group.Equals("GROUP_NO_READ"))
            {
                sb.AppendLine($"                    case AccessesNames.{accessName}:");
                sb.AppendLine($"                        drogeUserRole.{accessName} = true;");
                sb.AppendLine("                        break;");
            }
        }

        sb.AppendLine("                }");
        sb.AppendLine("            }");
        sb.AppendLine();
        sb.AppendLine("            return drogeUserRole;");
        sb.AppendLine("        }");

        // ToDb method
        sb.AppendLine();
        sb.AppendLine("        public static DbUserRoles ToDb(this DrogeUserRole userRole, Guid customerId)");
        sb.AppendLine("        {");
        sb.AppendLine("            var dbUserRole = new DbUserRoles");
        sb.AppendLine("            {");
        sb.AppendLine("                Id = userRole.Id,");
        sb.AppendLine("                ExternalId = userRole.ExternalId,");
        sb.AppendLine("                CustomerId = customerId,");
        sb.AppendLine("                Name = userRole.Name");
        sb.AppendLine("            };");
        sb.AppendLine();
        sb.AppendLine("            var sb = new StringBuilder();");

        // Adding access checks in the ToDb method
        foreach (var field in accessNames)
        {
            string accessName = field.Name;
            if (accessName.StartsWith("GROUP_"))
            {
                group = accessName;
            }
            else if (!group.Equals("GROUP_NO_READ"))
            {
                sb.AppendLine($"            if (userRole.{accessName})");
                sb.AppendLine($"                sb.Append(AccessesNames.{accessName}).Append(',');");
            }
        }

        sb.AppendLine();
        sb.AppendLine("            dbUserRole.Accesses = sb.ToString().Trim(',');");
        sb.AppendLine();
        sb.AppendLine("            return dbUserRole;");
        sb.AppendLine("        }");

        // End of class and namespace
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GenerateBlazorPage(FieldInfo[] accessNames)
    {
        StringBuilder sb = new StringBuilder();

        // Add a warning message at the top
        sb.AppendLine("@*");
        sb.AppendLine("<auto-generated>");
        sb.AppendLine("This code was generated by Drogecode.Knrm.Oefenrooster.CodeGenerator.");
        sb.AppendLine("If required, update the tool; not this file.");
        sb.AppendLine();
        sb.AppendLine("Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.");
        sb.AppendLine("</auto-generated>");
        sb.AppendLine("*@");
        sb.AppendLine();
        
        sb.AppendLine("@page \"/configuration/user-roles/edit/{Id:guid}\"");
        sb.AppendLine("@page \"/configuration/user-roles/add/\"");
        sb.AppendLine("@if (_userRole?.Role is not null)");
        sb.AppendLine("{");
        sb.AppendLine("    <MudPaper Class=\"pa-4\" MaxWidth=\"800px\">");

        // Render the Role Name TextField
        sb.AppendLine("        @if (!_editName && !string.IsNullOrWhiteSpace(_userRole.Role.Name))");
        sb.AppendLine("        {");
        sb.AppendLine("            <MudText Typo=\"Typo.h5\" Class=\"cursor-pointer\" @onclick=\"() => _editName = true\">@_userRole.Role.Name</MudText>");
        sb.AppendLine("        }");
        sb.AppendLine("        else");
        sb.AppendLine("        {");
        sb.AppendLine("            <MudTextField T=\"string?\" @bind-Value=\"_userRole.Role.Name\" Label=\"@L[\"Name\"]\"/>");
        sb.AppendLine("        }");

        // Render the ExternalId TextField
        sb.AppendLine("        <MudTextField T=\"string?\" @bind-Value=\"_userRole.Role.ExternalId\" Label=\"@L[\"ExternalId\"]\"/>");

        // Generate switches based on the group
        var group = string.Empty;
        foreach (var field in accessNames)
        {
            var accessName = field.Name;
            var fieldValue = (string)field.GetValue(null);
            if (accessName.StartsWith("GROUP_"))
            {
                group = accessName;
                if (group.Equals("GROUP_NO_READ"))
                    continue;
                sb.AppendLine($"        <MudText Typo=\"Typo.h6\">@L[\"{FirstCharToUpper(fieldValue ?? "not found")}\"]</MudText>");
            }
            else if (!group.Equals("GROUP_NO_READ"))
            {
                sb.AppendLine(
                    $"        <MudSwitch Label=\"@L[\"{fieldValue}\"]\" Color=\"Color.Primary\" T=\"bool\" Value=\"@_userRole.Role.{accessName}\" ValueChanged=\"@(isChecked => {{ _userRole.Role.{accessName} = isChecked; _saved = null; }})\"/>");
            }
        }

        // Submit button
        sb.AppendLine("        <MudButton Color=\"Color.Primary\" OnClick=\"Submit\">");
        sb.AppendLine("            @LApp[\"Submit\"]");
        sb.AppendLine("            @switch (_saved)");
        sb.AppendLine("            {");
        sb.AppendLine("                case true:");
        sb.AppendLine("                    <MudIcon Icon=\"@Icons.Material.Filled.Check\"/>");
        sb.AppendLine("                    break;");
        sb.AppendLine("                case false:");
        sb.AppendLine("                    <MudIcon Icon=\"@Icons.Material.Filled.Close\"/>");
        sb.AppendLine("                    break;");
        sb.AppendLine("            }");
        sb.AppendLine("        </MudButton>");

        sb.AppendLine("    </MudPaper>");
        sb.AppendLine("""
                          if (_linkedUsers?.LinkedUsers is not null && _users is not null)
                          {
                              <MudPaper>
                                  <MudList T="string" ReadOnly="true">
                                      <MudText Typo="Typo.h5">@L["Users in this role"]</MudText>
                                      @foreach (var userId in _linkedUsers.LinkedUsers)
                                      {
                                          var userName = _users.FirstOrDefault(x => x.Id == userId)?.Name ?? @LApp["User not found or deleted"];
                                          <MudListItem T="string">@userName</MudListItem>
                                      }
                                  </MudList>
                              </MudPaper>
                          }
                      """);
        sb.AppendLine("}");
        sb.AppendLine("else if (_userRole is null)");
        sb.AppendLine("{");
        sb.AppendLine("    <MudSkeleton/>");
        sb.AppendLine("    <MudSkeleton/>");
        sb.AppendLine("    <MudSkeleton/>");
        sb.AppendLine("    <MudSkeleton/>");
        sb.AppendLine("}");
        return sb.ToString();
    }
    
    private static string GenerateObject(FieldInfo[] accessNames)
    {
        // Use StringBuilder to construct the class content
        StringBuilder sb = new StringBuilder();

        // Warning Comment
        sb.AppendLine("// <auto-generated>");
        sb.AppendLine("// This code was generated by Drogecode.Knrm.Oefenrooster.CodeGenerator.");
        sb.AppendLine("// If required, update the tool; not this file.");
        sb.AppendLine("// ");
        sb.AppendLine("// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.");
        sb.AppendLine("// </auto-generated>");
        sb.AppendLine();

        // Begin the namespace and class definition
        sb.AppendLine("namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;");
        sb.AppendLine();
        sb.AppendLine("public class DrogeUserRole");
        sb.AppendLine("{");

        // Add basic properties (Id, ExternalId, and Name)
        sb.AppendLine("    public Guid Id { get; set; }");
        sb.AppendLine("    public string? ExternalId { get; set; }");
        sb.AppendLine("    public string? Name { get; set; }");
        sb.AppendLine();

        // Add a property for each AUTH constant in AccessesNames
        var group = string.Empty;
        foreach (var field in accessNames)
        {
            var accessName = field.Name;
            if (accessName.StartsWith("GROUP_"))
            {
                group = accessName;
                if (group.Equals("GROUP_NO_READ"))
                    continue;
                sb.AppendLine("");
                sb.AppendLine($"    // Group: {field.Name}");
            }
            else if (!group.Equals("GROUP_NO_READ"))
            {
                sb.AppendLine($"    public bool {field.Name} {{ get; set; }}");
            }
        }

        // End the class definition
        sb.AppendLine("}");

        // Output the generated class definition
        return sb.ToString();
    }

    private static void SaveToFile(string filePath, string content)
    {
        try
        {
            File.WriteAllText(filePath, content);
            Console.WriteLine("File saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex.Message}");
        }
    }

    static string FirstCharToUpper(string input) =>
        string.IsNullOrEmpty(input) ? string.Empty : input[0].ToString().ToUpper() + input.Substring(1);
}
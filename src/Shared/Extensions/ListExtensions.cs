using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System.Text;

namespace Drogecode.Knrm.Oefenrooster.Shared.Extensions;

public static class ListExtensions
{
    public static string ToFancyString(this List<string> list) => string.Join(',', list);

    public static string ToFancyString(this List<DrogeUser> list)
    {
        var result = new StringBuilder();

        for (var i = 0; i < list.Count; i++)
        {
            result.Append(list[i].Name);
            if (list.Count - 1 != i)
            {
                result.Append(", ");
            }
        }

        return result.ToString();
    }
}
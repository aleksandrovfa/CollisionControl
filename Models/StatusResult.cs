using Autodesk.Navisworks.Api.Clash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionControl
{
    public static class StatusResult
    {
        public static string ToString2(this ClashResultStatus me)
        {

            switch (me)
            {
                case ClashResultStatus.New:
                    return "Создать";
                case ClashResultStatus.Active:
                    return "Активный";
                case ClashResultStatus.Reviewed:
                    return "Проанализировано";
                case ClashResultStatus.Approved:
                    return "Подтверждено";
                case ClashResultStatus.Resolved:
                    return "Исправлено";
                default:
                    return "вроде такое невозможно";
            }
        }

        public static ClashResultStatus ToClashResultStatus(this string me)
        {

            switch (me)
            {
                case "Создать":
                    return ClashResultStatus.New;
                case "Активный":
                    return ClashResultStatus.Active;
                case "Проанализировано":
                    return ClashResultStatus.Reviewed;
                case "Подтверждено":
                    return ClashResultStatus.Approved;
                case "Исправлено":
                    return ClashResultStatus.Resolved;
                default:
                    throw new ArgumentException("Неправильное значение: " + me);
            }
        }

    }
}

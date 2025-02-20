using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionControl
{
    public static class StatusComment
    {
        public static string ToString2(this CommentStatus me)
        {

            switch (me)
            {
                case CommentStatus.Active:
                    return "Активный";
                case CommentStatus.Approved:
                    return "Утверждено";
                case CommentStatus.Resolved:
                    return "Исправлено";
                case CommentStatus.New:
                    return "Новый";
                default:
                    return "вроде такое невозможно";
            }
        }

        public static CommentStatus ToCommentStatus(this string me)
        {

            switch (me)
            {
                case "Активный":
                    return CommentStatus.Active;
                case "Утверждено":
                    return CommentStatus.Approved;
                case "Исправлено":
                    return CommentStatus.Resolved;
                case "Новый":
                    return CommentStatus.New;
                default:
                    throw new ArgumentException("Недопустимое значение статуса комментария");
            }
        }

    }
}

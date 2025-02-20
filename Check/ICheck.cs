using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionControl
{
    public interface ICheck
    {
        string Name { get; }
        bool IsSelected { get; set; }

        /// <summary>
        /// метод для фильтрации коллизий
        /// </summary>
        /// <param name="clashes">Коллизии которые нужно проверить</param>
        /// <returns>Коллизии, которые удовлетворяют условиям проверок</returns>
        List<WrapClashResult> GetClashAfterCheck(List<WrapClashResult> clashes);
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace CollisionControl
{
    /// <summary>
    /// Статический класс для работы с настройками
    /// </summary>
    public class PlaginSettingsManager
    {
        /// <summary>
        /// Сериализация настроек в файл
        /// </summary>
        /// <param name="settings">Настройки для сериализации</param>
        /// <param name="filename">Путь</param>
        public static void SaveSettings(PlaginSettings settings, string filename)
        {
            Debug.WriteLine($"Cериализация настроек по адресу {filename}");
            XmlSerializer serializer = new XmlSerializer(typeof(PlaginSettings));
            using (StreamWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, settings);
            }
        }
        /// <summary>
        /// Восстановление настроек из файла.
        /// При ошибки диссериализации перезаписывает файл пустыми настройками
        /// </summary>
        /// <param name="filename">Путь</param>
        /// <returns></returns>
        public static PlaginSettings LoadSettings(string filename)
        {
            try
            {
                Debug.WriteLine($"Диссериализация настроек по адресу {filename}");
                PlaginSettings settings = DeserilizeSettings(filename);
                return settings;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                PlaginSettings settings = new PlaginSettings();
                if (File.Exists(filename))
                {
                    SaveSettings(new PlaginSettings(), filename);
                    settings = DeserilizeSettings(filename);
                }
                else
                {
                    settings = new PlaginSettings();
                }
               
                return settings;
            }

        }
        /// <summary>
        /// Приватный метод диссериализации
        /// </summary>
        /// <param name="filename">Путь</param>
        /// <returns></returns>
        private static PlaginSettings DeserilizeSettings(string filename)
        {
           
            XmlSerializer serializer = new XmlSerializer(typeof(PlaginSettings));
            using (StreamReader reader = new StreamReader(filename))
            {
                return (PlaginSettings)serializer.Deserialize(reader);
            }
        }
        /// <summary>
        /// Применяет настройки к принятым элементам.
        /// </summary>
        /// <param name="sets1">Поисковые наборы ЭЛЕМЕНТА1</param>
        /// <param name="sets2">Поисковые наборы ЭЛЕМЕНТА2</param>
        /// <param name="tests">ПРОВЕРКИ</param>
        /// <param name="SelectedProfile">Профиль с настройками</param>
        public static void ApplySettings(List<WrapSavedItem> sets1, List<WrapSavedItem> sets2, List<WrapClashTest> tests ,  Profile SelectedProfile)
        {
            Debug.WriteLine($"Начало применения настроек по профилю {SelectedProfile.Name}") ;
            sets1.ForEach(x => x.IsSelected = false);
            sets2.ForEach(x => x.IsSelected = false);
            tests.ForEach(x => x.IsSelected = false);

            List<string> listError = new List<string>();
            foreach (Set set in SelectedProfile.Sets1)
            {
                WrapSavedItem setPlagin = sets1.Where(x => x.Set?.Guid == set.Guid).FirstOrDefault();
                if (setPlagin == null)
                {
                    setPlagin = sets1.Where(x => x.Set?.DisplayName == set.Name).FirstOrDefault();
                }
                if (setPlagin != null)
                {
                    setPlagin.IsSelected = true;
                }
                else
                {
                    string error = $"Набор {set.Name} для ЭЛЕМЕНТ1 не найден в проекте";
                    listError.Add(error);
                }
            }
            foreach (Set set in SelectedProfile.Sets2)
            {
                WrapSavedItem setPlagin = sets2.Where(x => x.Set?.Guid == set.Guid).FirstOrDefault();
                if (setPlagin == null)
                {
                    setPlagin = sets2.Where(x => x.Set?.DisplayName == set.Name).FirstOrDefault();
                }
                if (setPlagin != null)
                {
                    setPlagin.IsSelected = true;
                }
                else
                {
                    string error = $"Набор {set.Name} для ЭЛЕМЕНТ2 не найден в проекте";
                    listError.Add(error);
                }
            }
            foreach (Test rule in SelectedProfile.Tests)
            {

                WrapClashTest rulePlagin = tests.Where(x => x.DisplayName == rule.Name).FirstOrDefault();

                if (rulePlagin != null)
                {
                    rulePlagin.IsSelected = true;
                }
                else
                {
                    string error = $"ПРОВЕРКА {rule.Name} не найдена в проекте";
                    listError.Add(error);
                }
            }
            if (listError.Count == 0)
            {
                MessageBox.Show($"Настройки {SelectedProfile.Name} применены");
                Debug.WriteLine($"Настройки {SelectedProfile.Name} применены");
            }
            else
            {
                string messageError = String.Join("\n", listError);
                MessageBox.Show(messageError, "Ошибка восстановление настроек");
                Debug.WriteLine($"Ошибка восстановление настроек");
                Debug.WriteLine(messageError);
            }
        }

    }
}

using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using System.Collections.Generic;
using System.Linq;

namespace CollisionControl
{
    public class CheckElementInside : ICheck
    {
        public string Name { get { return "Проверка элемента внутри себя"; } }
        public bool IsSelected { get; set; }
        public CheckElementInside() { }
        public List<WrapClashResult> GetClashAfterCheck(List<WrapClashResult> clashes)
        {
            List<WrapClashResult> result = new List<WrapClashResult>();

            int progress = 0;

            Progress pbar = Application.BeginProgress("Контроль коллизий", "Поиск коллизий внутри элементов");
            

            foreach (var clashWrapper in clashes)
            {
                progress++;
                double procent = (double)progress / clashes.Count();
                if (pbar.IsCanceled)
                {
                    Application.EndProgress();
                    return result;
                }
                pbar.Update(procent);

                ClashResult clash = clashWrapper.Result;
                if (clash.Item1 != null && clash.Item2 != null)
                {
                    ModelItem item1 = GetItem("LcRevitInstance", clash.Item1);
                    ModelItem item2 = GetItem("LcRevitInstance", clash.Item2);
                    if (item1 != null && item2 != null)
                    {
                        if (item1.ClassName == "LcRevitInstance" && item2.ClassName == "LcRevitInstance")
                        {
                            //if (item1.InstanceGuid != System.Guid.Empty && item2.InstanceGuid != System.Guid.Empty)
                            //{
                            //    if (item1.InstanceGuid == item2.InstanceGuid)
                            //    {
                            //        clashesNew.Add(clashWrapper);
                            //    }
                            //}
                            //else
                            //{
                            string id1 = "";
                            string id2 = "";
                            string nameFamily = "";
                            foreach (PropertyCategory prop in item1.PropertyCategories)
                            {
                                if (prop.Name == "LcRevitId")
                                {
                                    id1 = prop.Properties[0].Value.ToString();
                                }
                            }

                            foreach (PropertyCategory prop in item2.PropertyCategories)
                            {
                                if (prop.Name == "LcRevitId")
                                {
                                    id2 = prop.Properties[0].Value.ToString();
                                }
                            }

                            if (id1 == id2)
                            {
                                clashWrapper.CheckItem1 = item1.DisplayName;
                                clashWrapper.CheckItem2 = item2.DisplayName;
                                result.Add(clashWrapper);
                            }
                            //}
                        }
                    }
                }
            }

            pbar.Update(1);
            Application.EndProgress();

            return result;
        }
        private ModelItem GetItem(string className, ModelItem item)
        {

            while (item.ClassName != className && item.Parent != null)
            {
                item = item.Parent;
            }
            return item;
        }
    }
}
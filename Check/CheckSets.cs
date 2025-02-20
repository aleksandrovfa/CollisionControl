using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionControl
{
    internal class CheckSets : ICheck
    {
        
        private List<WrapSavedItem> Sets1 { get; set; } = new List<WrapSavedItem>();
        private List<WrapSavedItem> Sets2 { get; set; } = new List<WrapSavedItem>();
        public string Name => throw new NotImplementedException();

        public bool IsSelected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public CheckSets( List<WrapSavedItem> sets1, List<WrapSavedItem> sets2)
        {
            Sets1 = sets1;
            Sets2 = sets2;
        }

        public List<WrapClashResult> GetClashAfterCheck(List<WrapClashResult> clashes)
        {
            List<WrapClashResult> result = new List<WrapClashResult>();


            Sets1.ForEach(x => x.Collection = x.Set.Search.FindAll(true));
            Sets2.ForEach(x => x.Collection = x.Set.Search.FindAll(true));

            WrapSavedItem ssw1 = null;
            WrapSavedItem ssw2 = null;
            int progress = 0;

            if (Sets1.Count > 0 && Sets2.Count > 0)
            {
                Progress pbar = Application.BeginProgress("Контроль коллизий", "Поиск коллизий в проверках по наборам в ЭЛЕМЕНТ1 и ЭЛЕМЕНТ2");
                progress = 0;
                foreach (var clash in clashes)
                {
                    progress++;
                    double procent = (double)progress / clashes.Count();
                    if (pbar.IsCanceled)
                    {
                        Application.EndProgress();
                        result.ForEach(x => x.ListCheckInCheck());
                        return result;
                    }
                    pbar.Update(procent);

                    ssw1 = CheckInListSet(clash, Sets1, 1);
                    if (ssw1 != null)
                    {
                        ssw2 = CheckInListSet(clash, Sets2, 2);
                        if (ssw2 != null)
                        {
                            if (!result.Contains(clash))
                            {
                                result.Add(clash);
                            }
                            clash.ListCheckItem1.Add(ssw1.DisplayName);
                            clash.ListCheckItem2.Add(ssw2.DisplayName);
                        }
                    }

                    ssw1 = CheckInListSet(clash, Sets1, 2);
                    if (ssw1 != null)
                    {
                        ssw2 = CheckInListSet(clash, Sets2, 1);
                        if (ssw2 != null)
                        {
                            if (!result.Contains(clash))
                            {
                                result.Add(clash);
                            }
                            clash.ListCheckItem1.Add(ssw2.DisplayName);
                            clash.ListCheckItem2.Add(ssw1.DisplayName);
                        }
                    }
                }
                pbar.Update(1);
                Application.EndProgress();
            }

            if (Sets1.Count == 0)
            {
                Progress pbar = Application.BeginProgress("Контроль коллизий", "Поиск коллизий в проверках по наборам в ЭЛЕМЕНТ2");
                progress = 0;
                foreach (var clash in clashes)
                {
                    progress++;
                    double procent = (double)progress / clashes.Count();
                    if (pbar.IsCanceled)
                    {
                        Application.EndProgress();
                        result.ForEach(x => x.ListCheckInCheck());
                        return result;
                    }
                    pbar.Update(procent);

                    ssw1 = CheckInListSet(clash, Sets2, 1);
                    ssw2 = CheckInListSet(clash, Sets2, 2);
                    if (ssw1 != null || ssw2 != null)
                    {
                        if (!result.Contains(clash))
                        {
                            result.Add(clash);
                        }
                        clash.ListCheckItem1.Add(ssw1?.DisplayName);
                        clash.ListCheckItem2.Add(ssw2?.DisplayName);
                    }
                }
                pbar.Update(1);
                Application.EndProgress();
            }

            if (Sets2.Count == 0)
            {
                Progress pbar = Application.BeginProgress("Контроль коллизий", "Поиск коллизий в проверках по наборам в ЭЛЕМЕНТ1");
                progress = 0;

                foreach (var clash in clashes)
                {
                    progress++;
                    double procent = (double)progress / clashes.Count();
                    if (pbar.IsCanceled)
                    {
                        Application.EndProgress();
                        result.ForEach(x => x.ListCheckInCheck());
                        return result;
                    }
                    pbar.Update(procent);

                    ssw1 = CheckInListSet(clash, Sets1, 1);
                    ssw2 = CheckInListSet(clash, Sets1, 2);
                    if (ssw1 != null || ssw2 != null)
                    {
                        if (!result.Contains(clash))
                        {
                            result.Add(clash);
                        }
                        clash.ListCheckItem1.Add(ssw1?.DisplayName);
                        clash.ListCheckItem2.Add(ssw2?.DisplayName);
                    }
                }
                pbar.Update(1);
                Application.EndProgress();
            }
            result.ForEach(x => x.ListCheckInCheck());
            return result;
        }

        private WrapSavedItem CheckInListSet(WrapClashResult clash, List<WrapSavedItem> set1Selected, int indexItem)
        {
            WrapSavedItem result = null;

            foreach (var set in set1Selected)
            {
                if (indexItem == 1)
                {
                    ModelItem item = clash.Result.Item1;
                    if (item != null)
                    {
                        if (set.Collection.IsContained(clash.Result.Item1))
                        {
                            result = set;
                            break;
                        }
                    }

                }

                if (indexItem == 2)
                {
                    ModelItem item = clash.Result.Item2;
                    if (item != null)
                    {
                        if (set.Collection.IsContained(clash.Result.Item2))
                        {
                            result = set;
                            break;
                        }
                    }
                }

            }
            return result;
        }
    }
}

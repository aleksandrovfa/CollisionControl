using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CollisionControl
{
    public class WrapClashResult : INotifyPropertyChanged
    {
        private ClashResult _result;

        private ClashTest _clashTest;

        public event PropertyChangedEventHandler PropertyChanged;

        public ClashResult Result { get { return _result; } }
        public ClashTest ClashTest { get { return _clashTest; } }

        public int Number { get { return Convert.ToInt32(_result.DisplayName.Replace("Конфликт", "")); } }
        public string NameClashResult { get { return _result.DisplayName; } }
        public string NameClashTest { get { return _clashTest.DisplayName; } }
        public string Status { get { return _result.Status.ToString2(); } }
       

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; 
                NotifyPropertyChanged(nameof(IsSelected)); }
        }
        public List<string> ListCheckItem1 { get; set; } = new List<string>();
        public List<string> ListCheckItem2 { get; set; } = new List<string>();
        public string CheckItem1 { get; set; }
        public string CheckItem2 { get; set; }
        public int Comments { get { return _result.Comments.Count(); } }
        public WrapClashResult(ClashResult result)
        {
            _result = result;

            GroupItem item = result.Parent;
            while (!(item is ClashTest))
            {
                item = item.Parent;
            }
            _clashTest = (ClashTest)item;

            IsSelected = false;
        }

        public void ListCheckInCheck()
        {
            CheckItem1 = String.Join(",", ListCheckItem1.Distinct().ToList());
            CheckItem2 = String.Join(",", ListCheckItem2.Distinct().ToList());
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void SetView()
        {
            try
            {
                Viewpoint viewpoint = Data.DocActive.CurrentViewpoint.Value;

                List<ModelItem> items = new List<ModelItem>();
                items.Add(_result.Item1);
                items.Add(_result.Item2);
                Data.DocActive.CurrentSelection.Clear();
                Data.DocActive.CurrentSelection.CopyFrom(items);
                Data.DocActive.ActiveView.FocusOnCurrentSelection();
                Data.DocActive.Models.ResetAllHidden();

                // Hide everything except for the 2 clashing elements
                ModelItemCollection modelItemCollection1 = new ModelItemCollection();
                ModelItemCollection modelItemCollection2 = new ModelItemCollection();

                using (IEnumerator<ModelItem> enumerator = Data.DocActive.CurrentSelection.SelectedItems.GetEnumerator())
                {
                    while (((IEnumerator)enumerator).MoveNext())
                    {
                        ModelItem current = enumerator.Current;

                        if (current.AncestorsAndSelf != null)
                            modelItemCollection2.AddRange((IEnumerable<ModelItem>)current.AncestorsAndSelf);

                        if (current.Descendants != null)
                            modelItemCollection2.AddRange((IEnumerable<ModelItem>)current.Descendants);
                    }
                }

                using (IEnumerator<ModelItem> enumerator = modelItemCollection2.GetEnumerator())
                {
                    while (((IEnumerator)enumerator).MoveNext())
                    {
                        ModelItem current = enumerator.Current;

                        if (!NativeHandle.ReferenceEquals((NativeHandle)current.Parent, (NativeHandle)null))
                            modelItemCollection1.AddRange((IEnumerable<ModelItem>)current.Parent.Children);
                    }
                }

                using (IEnumerator<ModelItem> enumerator = modelItemCollection2.GetEnumerator())
                {
                    while (((IEnumerator)enumerator).MoveNext())
                    {
                        ModelItem current = enumerator.Current;
                        modelItemCollection1.Remove(current);
                    }
                }

                Data.DocActive.Models.SetHidden((IEnumerable<ModelItem>)modelItemCollection1, true);
                Data.DocActive.Models.SetHidden(
                    ((IEnumerable<Model>)Data.DocActive.Models)
                    .SelectMany<Model, ModelItem>(
                        (Func<Model, IEnumerable<ModelItem>>)(c => (IEnumerable<ModelItem>)c.RootItem.Children))
                    .Except<ModelItem>((IEnumerable<ModelItem>)modelItemCollection1)
                    .Except<ModelItem>((IEnumerable<ModelItem>)modelItemCollection2)
                    , true
                );

                Data.DocActive.CurrentSelection.Clear();

                // Adjust the camera, lighting, and paint the clashing elements in Red and Green respectively
                Viewpoint copy = viewpoint.CreateCopy();
                copy.Lighting = (ViewpointLighting)0;

                Data.DocActive.Models.ResetAllPermanentMaterials();
                Data.DocActive.CurrentViewpoint.CopyFrom(copy);

                Autodesk.Navisworks.Api.Color RED = Autodesk.Navisworks.Api.Color.Red;
                Autodesk.Navisworks.Api.Color GREEN = Autodesk.Navisworks.Api.Color.Green;

                if (!NativeHandle.ReferenceEquals((NativeHandle)items.ElementAtOrDefault<ModelItem>(0), (NativeHandle)null))
                    Data.DocActive.Models.OverridePermanentColor((IEnumerable<ModelItem>)new ModelItem[1] { items.ElementAtOrDefault<ModelItem>(0) }, RED);

                if (!NativeHandle.ReferenceEquals((NativeHandle)items.ElementAtOrDefault<ModelItem>(1), (NativeHandle)null))
                    Data.DocActive.Models.OverridePermanentColor((IEnumerable<ModelItem>)new ModelItem[1] { items.ElementAtOrDefault<ModelItem>(1) }, GREEN);

                Data.DocActive.ActiveView.LookFromFrontRightTop();
                Data.DocActive.ActiveView.RequestDelayedRedraw((ViewRedrawRequests)3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

    }



   

}

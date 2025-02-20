using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api.DocumentParts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Application = Autodesk.Navisworks.Api.Application;
using GroupItem = Autodesk.Navisworks.Api.GroupItem;
using MessageBox = System.Windows.MessageBox;

namespace CollisionControl
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        #region Свойства

        /// <summary>
        /// Деревовидная струтура папок и поисковых наборов для ЭЛЕМЕНТ2
        /// Используется только для отображения в окне
        /// </summary>
        public ObservableCollection<WrapSavedItem> TreeSavedItem1 { get; set; } = new ObservableCollection<WrapSavedItem>();

        /// <summary>
        /// Деревовидная струтура папок и поисковых наборов для ЭЛЕМЕНТ2
        /// Используется только для отображения в окне
        /// </summary>
        public ObservableCollection<WrapSavedItem> TreeSavedItem2 { get; set; } = new ObservableCollection<WrapSavedItem>();

        /// <summary>
        /// Поисковые наборы для ЭЛЕМЕНТ1
        /// </summary>
        public ObservableCollection<WrapSavedItem> Sets1 { get; set; } = new ObservableCollection<WrapSavedItem>();

        /// <summary>
        /// Поисковые наборы для ЭЛЕМЕНТ2
        /// </summary>
        public ObservableCollection<WrapSavedItem> Sets2 { get; set; } = new ObservableCollection<WrapSavedItem>();

        /// <summary>
        /// ПРОВЕРКИ
        /// </summary>
        public ObservableCollection<WrapClashTest> ClashTests { get; set; } = new ObservableCollection<WrapClashTest>();

        /// <summary>
        /// Результаты в ОКНЕ ВЫВОДА
        /// </summary>
        public ObservableCollection<WrapClashResult> ClashResults { get; set; } = new ObservableCollection<WrapClashResult>();

        /// <summary>
        /// Проверки для поиска коллизий во вкладке Другое
        /// </summary>
        public ObservableCollection<ICheck> Checks { get; set; } = new ObservableCollection<ICheck>();

        /// <summary>
        /// Номер открытой вкладки (выбор способа проверок)
        /// </summary>
        public int SelectedTabIndex { get; set; } = 0;

        /// <summary>
        /// Список всех статусов для коллизий
        /// </summary>
        public List<string> ClashResultStatusList
        {
            get
            {
                var list = new List<string>();
                foreach (var en in Enum.GetValues(typeof(ClashResultStatus)))
                    list.Add(((ClashResultStatus)en).ToString2());
                return list;
            }
        }

        private ClashResultStatus _clashStatus = ClashResultStatus.Reviewed;

        /// <summary>
        /// Выбранный статус для коллизий
        /// </summary>
        public string StatusClash
        {
            get { return _clashStatus.ToString2(); }
            set { _clashStatus = value.ToClashResultStatus(); }
        }

        public List<string> CommentStatusList
        {
            get
            {
                var list = new List<string>();
                foreach (var en in Enum.GetValues(typeof(CommentStatus)))
                    list.Add(((CommentStatus)en).ToString2());
                return list;
            }
        }

        private CommentStatus _commentStatus = CommentStatus.New;

        /// <summary>
        /// Выбранный статус для комментариев
        /// </summary>
        public string StatusComment
        {
            get { return _commentStatus.ToString2(); }
            set { _commentStatus = value.ToCommentStatus(); }
        }
        public string Comment { get; set; } = "Текст комментария";

        /// <summary>
        /// Настройки с профилями
        /// </summary>
        public PlaginSettings Settings { get; private set; }

        /// <summary>
        /// Список профилей для отображения
        /// </summary>
        public ObservableCollection<Profile> Profiles { get { return Settings.Profiles; } }

        /// <summary>
        /// Выбранный профиль
        /// </summary>
        public Profile SelectedProfile { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Счетчики
        public int CountClashs { get { return ClashTests.Count; } }

        private int _countClashsSelected;
        public int CountClashsSelected
        {
            get { return _countClashsSelected; }
            set { _countClashsSelected = value; NotifyPropertyChanged(nameof(CountClashsSelected)); }
        }

        private int _countSet1Selected;
        public int CountSet1Selected
        {
            get { return _countSet1Selected; }
            set { _countSet1Selected = value; NotifyPropertyChanged(nameof(CountSet1Selected)); }
        }

        private int _countSet2Selected;
        public int CountSet2Selected
        {
            get { return _countSet2Selected; }
            set { _countSet2Selected = value; NotifyPropertyChanged(nameof(CountSet2Selected)); }
        }

        private int _countClashResults;
        public int CountClashResults
        {
            get { return _countClashResults; }
            set { _countClashResults = value; NotifyPropertyChanged(nameof(CountClashResults)); }
        }

        #endregion

        #region Команды
        /// <summary>
        /// Поиск коллизий
        /// </summary>
        private RelayCommand searchCommand;
        public RelayCommand SearchCommand
        {
            get { return searchCommand ?? (searchCommand = new RelayCommand(obj => Search())); }
        }
        /// <summary>
        /// Обновить проверки и наборы
        /// </summary>
        private RelayCommand reloadCommand;
        public RelayCommand ReloadCommand
        {
            get { return reloadCommand ?? (reloadCommand = new RelayCommand(obj => Load())); }
        }

        /// <summary>
        /// Выделить все проверки
        /// </summary>
        private RelayCommand clashTests_SelectAllCommand;

        public RelayCommand ClashTests_SelectAllCommand
        {
            get { return clashTests_SelectAllCommand ?? (clashTests_SelectAllCommand = new RelayCommand(obj => ClashTests_SelectAll())); }
        }

        /// <summary>
        /// Изменить статус у выделенных коллизий
        /// </summary>
        private RelayCommand clashResults_changeStatusCommand;
        public RelayCommand ClashResults_ChangeStatusCommand
        {
            get { return clashResults_changeStatusCommand ?? (clashResults_changeStatusCommand = new RelayCommand(obj => ClashResults_ChangeStatus())); ; }
        }

        /// <summary>
        /// Добавить комментарий у выделенных коллизий
        /// </summary>
        private RelayCommand clashResults_addCommentCommand;
        public RelayCommand ClashResults_AddCommentCommand
        {
            get { return clashResults_addCommentCommand ?? (clashResults_addCommentCommand = new RelayCommand(obj => ClashResults_AddComment())); ; }
        }

        /// <summary>
        /// Выделить все найденные коллизии
        /// </summary>
        private RelayCommand clashResults_SelectAllCommand;
        public RelayCommand ClashResults_SelectAllCommand
        {
            get { return clashResults_SelectAllCommand ?? (clashResults_SelectAllCommand = new RelayCommand(obj => ClashResults_SelectAll())); }
        }


        /// <summary>
        /// Снять выделения у всех найденных коллизий
        /// </summary>
        private RelayCommand clashResults_UnSelectAllCommand;

        public RelayCommand ClashResults_UnSelectAllCommand
        {
            get { return clashResults_UnSelectAllCommand ?? (clashResults_UnSelectAllCommand = new RelayCommand(obj => ClashResults_UnSelectAll())); }
        }



        /// <summary>
        /// Двойное нажатие на коллизию
        /// </summary>
        private RelayCommand clashResults_DoubleClickCommand;
        public RelayCommand ClashResults_DoubleClickCommand
        {
            get { return clashResults_DoubleClickCommand ?? (clashResults_DoubleClickCommand = new RelayCommand(obj => ClashResults_SetView(obj))); }
        }


        /// <summary>
        /// Редактирование профилей
        /// </summary>
        private RelayCommand settings_EditCommand;
        public RelayCommand Settings_EditCommand
        {
            get { return settings_EditCommand ?? (settings_EditCommand = new RelayCommand(obj => Settings_Edit())); }
        }
        /// <summary>
        /// Применить выбранный профиль
        /// </summary>
        private RelayCommand profile_SetCommand;
        public RelayCommand Profile_SetCommand
        {
            get { return profile_SetCommand ?? (profile_SetCommand = new RelayCommand(obj => Profile_Set())); }
        }


        /// <summary>
        /// Добавить профиль
        /// </summary>
        private RelayCommand profile_AddCommand;
        public RelayCommand Profile_AddCommand
        {
            get { return profile_AddCommand ?? (profile_AddCommand = new RelayCommand(obj => Profile_Add())); }
        }


        /// <summary>
        /// Клик по комментарию (изменения текста)
        /// </summary>
        private RelayCommand textBlock_clickCommand;
        public RelayCommand TextBlock_ClickCommand
        {
            get { return textBlock_clickCommand ?? (textBlock_clickCommand = new RelayCommand(obj => TextBlock_Click())); }
        }

        /// <summary>
        /// Экспорт в ексель
        /// </summary>
        private RelayCommand сlashResults_exportCommand;
        public RelayCommand ClashResults_ExportCommand
        {
            get { return сlashResults_exportCommand ?? (сlashResults_exportCommand = new RelayCommand(obj => СlashResults_Export())); }
        }

        /// <summary>
        /// Команда по закрытию окна
        /// </summary>
        private RelayCommand window_closeCommand;
        public RelayCommand Window_CloseCommand
        {
            get { return window_closeCommand ?? (window_closeCommand = new RelayCommand(obj => Window_Close())); }
        }
        #endregion

        #endregion
        public MainViewModel()
        {
            Load();
        }
        #region Методы

        #region Подписки
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void ClashResults_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CountClashResults = ClashResults.Count;
        }
        private void UpdateCountSelectSet1(object sender, PropertyChangedEventArgs e)
        {
            CountSet1Selected = Sets1.Where(c => c.IsSelected).Count();
        }
        private void UpdateCountSelectSet2(object sender, PropertyChangedEventArgs e)
        {
            CountSet2Selected = Sets2.Where(c => c.IsSelected).Count();
        }
        private void UpdateCountSelectClashWrapper(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CountClashsSelected = ClashTests.Where(c => c.IsSelected).Count();
        }
        #endregion
        private void Search()
        {
            ClashResults.Clear();
            try
            {
                Debug.WriteLine("Начало поиска");
                List<WrapClashResult> allClash = new List<WrapClashResult>();
                foreach (var clashTest in ClashTests.Where(x => x.IsSelected))
                {
                    foreach (var rt in clashTest.ClashTest.Children)
                    {
                        if (rt is ClashResult)
                        {
                            WrapClashResult test = new WrapClashResult((ClashResult)rt);
                            allClash.Add(test);
                        }
                        if (rt is ClashResultGroup)
                        {
                            ClashResultGroup test = (ClashResultGroup)rt;
                            foreach (var item in test.Children)
                            {
                                if (item is ClashResult)
                                {
                                    WrapClashResult t = new WrapClashResult((ClashResult)item);
                                    allClash.Add(t);
                                }
                            }
                        }
                    }
                }
                List<WrapSavedItem> set1Selected = Sets1.Where(x => x.IsSelected).ToList();
                List<WrapSavedItem> set2Selected = Sets2.Where(x => x.IsSelected).ToList();


                if (SelectedTabIndex == 0)
                {

                    if ((set1Selected.Count + set2Selected.Count) > 0)
                    {
                        CheckSets checkSets = new CheckSets(set1Selected, set2Selected);
                        List<WrapClashResult> clashAfterCheck = checkSets.GetClashAfterCheck(allClash);
                        clashAfterCheck.ForEach(x => ClashResults.Add(x));
                    }
                    else
                    {
                        allClash.ForEach(x => ClashResults.Add(x));
                    }
                }

                if (SelectedTabIndex == 1)
                {
                    foreach (var check in Checks)
                    {
                        check.GetClashAfterCheck(allClash).ForEach(x => ClashResults.Add(x));
                    }
                }
                Debug.WriteLine("Поиск закончен");
                MessageBox.Show("Поиск закончен");
            }
            catch (ObjectDisposedException ex)
            {
                Debug.WriteLine(ex.ToString());
                string message = "Проверки в плагине устарели. Пожалуйста обновите";
                Debug.WriteLine(message);
                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }
        private void Load()
        {
            Debug.WriteLine("Загрузка всех коллекций");
            Data.DocMain = Application.MainDocument;
            try
            {
                ///Обнуление коллекций и счетчиков
                ClashTests.Clear();
                Sets1.Clear();
                Sets2.Clear();
                TreeSavedItem1.Clear();
                TreeSavedItem2.Clear();
                Checks.Clear();
                CountSet1Selected = 0;
                CountSet2Selected = 0;
                CountClashsSelected = 0;

                ///Заполнение коллекций
                SetClashTests();
                SetTreesAndSets();

                ///Подписки у коллекций
                Sets1.ToList().ForEach(x => x.PropertyChanged += UpdateCountSelectSet1);
                Sets2.ToList().ForEach(x => x.PropertyChanged += UpdateCountSelectSet2);
                ClashResults.CollectionChanged += ClashResults_CollectionChanged;

                ///Добавление кастомных проверок во вкладке другое
                Checks.Add(new CheckElementInside());

                //Заполнение настроек и профилей
                Settings = PlaginSettingsManager.LoadSettings(Properties.Settings.Default.SettingPath);
                //Settings = PlaginSettingsManager.LoadSettings("O:\\Etalon Project\\KPO\\СФП\\alexey.razumov\\CollisionControl\\Настройки временно создаются здесь\\Настройки.xml");
                SelectedProfile = Profiles.FirstOrDefault();
                Debug.WriteLine("Конец загрузки всех коллекций");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Debug.WriteLine(ex.ToString());
            }
            

        }
        private void SetClashTests()
        {
            foreach (ClashTest clashTest in Data.ClashTests)
            {
                WrapClashTest clashWrapper = new WrapClashTest(clashTest);
                clashWrapper.PropertyChanged += UpdateCountSelectClashWrapper;
                ClashTests.Add(clashWrapper);
            }
        }
        private void SetTreesAndSets()
        {

            var searchSets = Data.DocActive.SelectionSets.Value;

            foreach (var item in searchSets)
            {
                TreeSavedItem1.Add((new WrapSavedItem(item)));
                TreeSavedItem2.Add((new WrapSavedItem(item)));
            }

            foreach (var item in TreeSavedItem1)
            {
                if (item.IsGroup)
                {
                    foreach (var node in item.GetNodes())
                    {
                        Sets1.Add(node);
                    }
                }
                else
                {
                    Sets1.Add(item);
                }
            }

            foreach (var item in TreeSavedItem2)
            {
                if (item.IsGroup)
                {
                    foreach (var node in item.GetNodes())
                    {
                        Sets2.Add(node);
                    }
                }
                else
                {
                    Sets2.Add(item);
                }
            }



        }
        private void ClashTests_SelectAll()
        {
            ClashTests.ToList().ForEach(result => result.IsSelected = true);
        }
        private void Profile_Set()
        {

            if (SelectedProfile != null)
            {
                Debug.WriteLine($"Применения настроек {SelectedProfile.Name}");
                PlaginSettingsManager.ApplySettings(Sets1.ToList(), Sets2.ToList(), ClashTests.ToList(), SelectedProfile);
            }
        }
        private void Profile_Add()
        {
            SettingViewModel settingViewModel = new SettingViewModel(Sets1.ToList(), Sets2.ToList(), ClashTests.ToList(), Settings);
            settingViewModel.Add();
            settingViewModel.SaveAndExit();
        }
        private void Settings_Edit()
        {
            try
            {
                Debug.WriteLine($"Редактирование настроек");
                SettingsView settingsView = new SettingsView(Sets1.ToList(), Sets2.ToList(), ClashTests.ToList(), Settings);
                settingsView.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void ClashResults_ChangeStatus()
        {
            Debug.WriteLine($"Изменение статуса у {ClashResults.Count} коллизий");
            Transaction transaction = new Transaction(Data.DocMain, "Изменение статуса");
            foreach (var clash in ClashResults)
            {
                if (clash.IsSelected)
                {
                    Data.DocClashTests.TestsEditResultStatus(clash.Result, _clashStatus);
                    clash.NotifyPropertyChanged(nameof(WrapClashResult.Status));
                }
            }
            transaction.Commit();
            Debug.WriteLine($"Конец изменения статуса");

        }
        private void ClashResults_AddComment()
        {
            try
            {
                
                Debug.WriteLine($"Добавления комментария у {ClashResults.Count} коллизий");
                Transaction transaction = new Transaction(Data.DocMain, "Добавления комментария");
                foreach (var clash in ClashResults)
                {
                    if (clash.IsSelected)
                    {
                        CommentCollection comments = new CommentCollection(clash.Result.Comments);
                        comments.Add(new Comment(Comment, _commentStatus));
                        Data.DocClashTests.TestsEditResultComments(clash.Result, comments);
                        clash.NotifyPropertyChanged(nameof(WrapClashResult.Comments));
                    }
                }
                transaction.Commit();
                Debug.WriteLine($"Конец добавление комментария");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Debug.WriteLine(ex.ToString());
            }
            
        }
        private void ClashResults_SetView(object obj)
        {
            WrapClashResult row = (WrapClashResult)obj;
            Debug.WriteLine($"Начало показа вида у {row.NameClashTest} {row.NameClashResult}");
            row.SetView();
            Debug.WriteLine($"Вид показан");
        }
        private void ClashResults_SelectAll()
        {
            ClashResults.ToList().ForEach(result => result.IsSelected = true);
        }
        private void ClashResults_UnSelectAll()
        {
            ClashResults.ToList().ForEach(result => result.IsSelected = false);
        }
        private void СlashResults_Export()
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (Properties.Settings.Default.ExportPath != null)
                {
                    dialog.SelectedPath = Properties.Settings.Default.ExportPath;
                }
                Debug.WriteLine("Выбор папки");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Debug.WriteLine($"Папка выбрана {dialog.SelectedPath}");
                    Properties.Settings.Default.ExportPath = dialog.SelectedPath;
                    Properties.Settings.Default.Save();
                    Debug.WriteLine("Начало экспорта");


                    string filenameShort;

                    if (this.SelectedProfile != null)
                        filenameShort = this.SelectedProfile.Name;
                    else
                        filenameShort = Data.DocMain.Title;

                    string filename = dialog.SelectedPath +
                        @"\"+ DateTime.Now.ToString("yyyyMMddHHmmss") 
                        +"_" +filenameShort + ".xlsx";

                    DataTable data = ExportManager.ToDataTable(ClashResults.ToList());
                    try
                    {
                        ExportManager.ToExcelFile2(data, filename);
                        //ExportManager.ToTxTFile(data, filename.Replace(".xlsx", ".txt"));
                    }
                    catch (Exception ex )
                    {
                        MessageBox.Show(ex.ToString());
                        MessageBox.Show("Ошибка при экспорте в Excel. Будет сделан экспорт в txt");
                        ExportManager.ToTxTFile(data, filename.Replace(".xlsx", ".txt"));
                       
                    }
                    Debug.WriteLine("Конец экспорта");
                    MessageBox.Show("Экспорт завершен");
                }
                else
                {
                    Debug.WriteLine($"Отмена экспорта");
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Debug.WriteLine(ex.ToString());
            }

           
        }
        private void TextBlock_Click()
        {
            SettingNameView view = new SettingNameView(Comment,"Введите текст комментария");
            if (view.ShowDialog() == true)
            {
                Comment = view.NameProfile.Text;
                NotifyPropertyChanged(nameof(Comment));
            }
        }
        private void Window_Close()
        {
            Data.DocActive.Models.ResetAllHidden();
        }

        #endregion
    }
}
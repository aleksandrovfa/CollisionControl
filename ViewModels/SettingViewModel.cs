using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace CollisionControl
{
    public class SettingViewModel : INotifyPropertyChanged
    {
        #region Свойства

        /// <summary>
        /// Поисковые наборы для ЭЛЕМЕНТА1
        /// </summary>
        public List<WrapSavedItem> Sets1 { get; }

        /// <summary>
        /// Поисковые наборы для ЭЛЕМЕНТА2
        /// </summary>
        public List<WrapSavedItem> Sets2 { get; }

        /// <summary>
        /// ПРОВЕРКИ
        /// </summary>
        public List<WrapClashTest> ClashTests { get; }

        /// <summary>
        /// Настройки
        /// </summary>
        public PlaginSettings Settings { get; set; }
        /// <summary>
        /// Выбранный профиль
        /// </summary>
        public Profile SelectedProfile { get; set; }

        /// <summary>
        /// Профили
        /// </summary>
        public ObservableCollection<Profile> Profiles { get { return Settings.Profiles; } }

        #region Команды

        private RelayCommand changePathCommand;
        public RelayCommand ChangePathCommand
        {
            get { return changePathCommand ?? (changePathCommand = new RelayCommand(obj => ChangePathSetting())); }
        }

        private RelayCommand changeNameCommand;
        public RelayCommand ChangeNameCommand
        {
            get { return changeNameCommand ?? (changeNameCommand = new RelayCommand(obj => ChangeNameProfile())); }
        }

        private RelayCommand saveAndExitCommand;
        public RelayCommand SaveAndExitCommand
        {
            get { return saveAndExitCommand ?? (saveAndExitCommand = new RelayCommand(obj => SaveAndExit())); }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get { return deleteCommand ?? (deleteCommand = new RelayCommand(obj => Delete())); }
        }

        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get { return addCommand ?? (addCommand = new RelayCommand(obj => Add())); }
        }

        private RelayCommand upCommand;
        public RelayCommand UpCommand
        {
            get { return upCommand ?? (upCommand = new RelayCommand(obj => UpItemsCommand())); }
        }

        private RelayCommand downCommand;
        public RelayCommand DownCommand
        {
            get { return downCommand ?? (downCommand = new RelayCommand(obj => DownItemsCommand())); }
        }

        #endregion

        #region События
        public event EventHandler CloseRequest;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #endregion

        /// <summary>
        /// Конструктор для работы с настройками
        /// </summary>
        /// <param name="sets1">Наборы поиска ЭЛЕМЕНТ1</param>
        /// <param name="sets2">Наборы поиска ЭЛЕМЕНТ2</param>
        /// <param name="tests">Проверки</param>
        /// <param name="settings">Настройки</param>
        public SettingViewModel(List<WrapSavedItem> sets1, List<WrapSavedItem> sets2, List<WrapClashTest> tests, PlaginSettings settings)
        {
            Sets1 = sets1;
            Sets2 = sets2;
            ClashTests = tests;
            Settings = settings;
        }

        #region Методы
        private void ChangePathSetting()
        {

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = Path.GetDirectoryName(Properties.Settings.Default.SettingPath);
            openFile.Filter = "XML Files (*.xml)|*.xml";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PlaginSettingsManager.SaveSettings(Settings, Properties.Settings.Default.SettingPath);
                    PlaginSettings sett = PlaginSettingsManager.LoadSettings(openFile.FileName);
                    
                    Profiles.Clear();
                    sett.Profiles.ToList().ForEach(profile => Profiles.Add(profile));
                    NotifyPropertyChanged(nameof(Profiles));
                    Properties.Settings.Default.SettingPath = openFile.FileName;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    Debug.WriteLine(ex.ToString());
                }
            }
        }
        public void Add()
        {
            string name;
            SettingNameView settingNameView = new SettingNameView();
            settingNameView.ShowDialog();
            if (settingNameView.DialogResult == true)
            {
                name = settingNameView.Name;
            }
            else
            {
                return;
            }
            Profile profile = new Profile(name, Sets1, Sets2,ClashTests);
            Profiles.Add(profile);
        }
        private void Delete()
        {
            Profiles.Remove(SelectedProfile);
        }
        private void ChangeNameProfile()
        {
            if (SelectedProfile != null)
            {
                SettingNameView settingNameView = new SettingNameView(SelectedProfile.Name);
                settingNameView.ShowDialog();
                if (settingNameView.DialogResult == true)
                {
                    SelectedProfile.Name = settingNameView.Name;
                }
            }
        }
        public void SaveAndExit()
        {
            Settings.Profiles = Profiles;
            PlaginSettingsManager.SaveSettings(Settings, Properties.Settings.Default.SettingPath);
            RaiseCloseRequest();
        }
        private void UpItemsCommand()
        {
            int i = Profiles.IndexOf(SelectedProfile);
            if (i > 0)
            {
                Profiles.Move(i, i - 1);
            }
        }

        private void DownItemsCommand()
        {
            int i = Profiles.IndexOf(SelectedProfile);
            if (i < Profiles.Count - 1)
            {
                Profiles.Move(i, i + 1);
            }
        }
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);

        }
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
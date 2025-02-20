using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application = Autodesk.Navisworks.Api.Application;


namespace CollisionControl
{
    /// <summary>
    /// Класс для формирование основных объектов ревита, для удобства обращения к ним из любого места
    /// </summary>
    public static class Data
    {
        private static Application _app = null;
        private static Document _docMain = null;
        private static Document _docActive = null;
        private static DocumentClash _docClash = null;
        private static DocumentClashTests _docClashTests = null;
        private static List<ClashTest> _clashTests = null;


        public static Document DocMain
        {
            get
            {
                return _docMain;
            }
            set
            {
                _docMain = value;
                _docActive = Application.ActiveDocument;
                _docClash = _docMain.GetClash();
                _docClashTests = _docClash.TestsData;
                _clashTests = _docClashTests.Tests.Select(x => x as ClashTest).ToList();
            }
        }
        public static Document DocActive { get { return _docActive; } }
        public static DocumentClashTests DocClashTests { get { return _docClashTests; } }
        public static DocumentClash DocClash { get { return _docClash; } }
        public static List<ClashTest> ClashTests { get { return _clashTests; } }
        //public static string SettingsPath {get; set; } = @"O:\Etalon Project\KPO\Revit\!_БИБЛИОТЕКА РЕСУРСОВ\06_АУДИТ\01_Navisworks\CollisionControl_Settings_ЭтиНеТрогай.xml";


    }
   

}

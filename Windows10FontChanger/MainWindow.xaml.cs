using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System;

namespace Windows10FontChanger
{
    public partial class MainWindow : Window
    {
        #region Data
        RegistryKey _fontsKey = 
            Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts");
        RegistryKey _fontSubstitutesKey = 
            Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\FontSubstitutes");

        Dictionary<string, string> _fontsToChange = new Dictionary<string, string>
        {
            { "Segoe UI (TrueType)", "segoeui.ttf" },
            { "Segoe UI Black (TrueType)", "seguibl.ttf" },
            { "Segoe UI Black Italic (TrueType)", "seguibli.ttf"},
            { "Segoe UI Bold (TrueType)", "segoeuib.ttf"},
            { "Segoe UI Bold Italic (TrueType)", "segoeuiz.ttf" },
            { "Segoe UI Italic (TrueType)", "segoeuii.ttf" },
            { "Segoe UI Light (TrueType)", "segoeuil.ttf" },
            { "Segoe UI Light Italic (TrueType)", "seguili.ttf" },
            { "Segoe UI Semibold (TrueType)", "seguisb.ttf" },
            { "Segoe UI Semibold Italic (TrueType)", "seguisbi.ttf" },
            { "Segoe UI Semilight (TrueType)", "segoeuisl.ttf" },
            { "Segoe UI Semilight Italic (TrueType)", "seguisli.ttf" },
            { "Segoe Print (TrueType)", "segoepr.ttf" },
            { "Segoe Print Bold (TrueType)", "segoeprb.ttf" },
            { "Segoe Script (TrueType)", "segoesc.ttf" },
            { "Segoe Script Bold (TrueType)", "segoescb.ttf" }
        };

        string _selectedFont = "";

        public string SelectedFont
        {
            get { return _selectedFont; }
            set { _selectedFont = value; OnPropertyChanged(); }
        }

        List<string> _fonts = new List<string>();
        public List<string> FontList
        {
            get { return _fonts; }
            set { _fonts = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler ?PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string ?propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region UI
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            FontList.Clear();
            FontList = Fonts.SystemFontFamilies.Select(x => x.Source).ToList();
            FontList.Sort();
            comBoxFonts.SelectedValue = _fontSubstitutesKey.GetValue("Segoe UI");
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void comBoxFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedFont = (string)comBoxFonts.SelectedValue;
            UpdateUIFonts();
        }

        private void btnSelectFont_Click(object sender, RoutedEventArgs e)
        {
            SelectFont();
        }

        private void btnResetFont_Click(object sender, RoutedEventArgs e)
        {
            ResetFont();
        }
        #endregion

        #region Logic
        void UpdateUIFonts()
        {
            btnExit.FontFamily =
            btnMinimize.FontFamily =
            lblExample1.FontFamily =
            lblExample2.FontFamily =
            lblExample3.FontFamily =
            comBoxFonts.FontFamily =
            tbFontTesting.FontFamily =
            btnApplyFont.FontFamily =
            btnResetFont.FontFamily =
            FontFamily = new FontFamily(_selectedFont);
        }

        void SelectFont()
        {
            foreach (var font in _fontsToChange)
            {
                _fontsKey.SetValue(font.Key, "");
            }
            _fontSubstitutesKey.SetValue("Segoe UI", _selectedFont);
            if (RestartDialog()) Restart();
        }

        void ResetFont()
        {
            foreach (var font in _fontsToChange)
            {
                _fontsKey.SetValue(font.Key, font.Value);
            }
            _fontSubstitutesKey.SetValue("Segoe UI", "Segoe UI");
            comBoxFonts.SelectedValue = _fontSubstitutesKey.GetValue("Segoe UI");
            if (RestartDialog()) Restart();
        }

        bool RestartDialog()
        {
            return
                MessageBox.Show("In order to complete the process, you must sign in again.\nThis will result in all your applications being closed and unsaved progress being lost.\nDo you want to do that now?", "Sign out", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
                == MessageBoxResult.Yes ?
                true : false;
        }

        void Restart()
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.CreateNoWindow = true;
            proc.FileName = "cmd";
            proc.Arguments = "/C shutdown -l";
            Process.Start(proc);
        }
        #endregion

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}

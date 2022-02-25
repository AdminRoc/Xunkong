﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.NumberFormatting;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Xunkong.Desktop.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {


        private string AppVersion => XunkongEnvironment.AppVersion.ToString(4);

        private SettingViewModel vm => (DataContext as SettingViewModel)!;





        public SettingPage()
        {
            this.InitializeComponent();
            DataContext = ActivatorUtilities.CreateInstance<SettingViewModel>(App.Current.Services);
            Loaded += async (_, _) => await vm.InitializeDataAsync();
            SetNumberBoxNumberFormatter();
        }



        private void _Expander_WebTool_Collapsed(Expander sender, ExpanderCollapsedEventArgs args)
        {
            vm.SelectedWebToolItem = null;
        }


        private void SetNumberBoxNumberFormatter()
        {
            IncrementNumberRounder rounder = new IncrementNumberRounder();
            rounder.Increment = 0.01;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundUp;

            DecimalFormatter formatter = new DecimalFormatter();
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 2;
            formatter.NumberRounder = rounder;
            _NumberBox_HomeCoinThreshold.NumberFormatter = formatter;
        }




    }
}

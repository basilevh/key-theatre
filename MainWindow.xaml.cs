// Started 04-11-2016, Basile Van Hoorick

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using KeyDecorator.Decorators;

namespace KeyDecorator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Load settings
            loadSettings();
            this.loadingCtrls = false;

            // Auto-initialize
            init();
            update();

            // Subscribe to user control events
            ucDecoPressPlus.ParameterChanged += ucDecoPressPlus_ParameterChanged;
        }

        private bool loadingCtrls = true;
        private bool sdkTitleAdded = false;
        private Decorator decorator;
        private System.Drawing.Color backClr;

        private void init()
        {
            // Init SDK
            LogitechGSDK.LogiLedInit();
            LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_PERKEY_RGB); // only G810, G910, ...
            LogitechGSDK.LogiLedSaveCurrentLighting();

            if (!sdkTitleAdded)
            {
                // Show SDK version
                int major = -1, minor = -1, build = -1;
                LogitechGSDK.LogiLedGetSdkVersion(ref major, ref minor, ref build);
                this.Title += " (SDK version " + major + "." + minor + "." + build + ")";
                this.sdkTitleAdded = true;
            }

            // Update controls
            btnInit.IsEnabled = false;
            btnClear.IsEnabled = true;
        }

        private void clear()
        {
            // Stop decorator
            if (decorator != null)
            {
                decorator.Stop();
                this.decorator = null;
            }

            // Shutdown SDK
            LogitechGSDK.LogiLedStopEffects();
            LogitechGSDK.LogiLedRestoreLighting();
            LogitechGSDK.LogiLedShutdown();

            // Update controls
            btnInit.IsEnabled = true;
            btnClear.IsEnabled = false;
        }

        private void update()
        {
            if (loadingCtrls)
                return;

            // Stop decorator
            if (decorator != null && decorator.IsRunning)
                decorator.Stop();
            
            // Start decorator
            if (radPress.IsChecked == true)
                this.decorator = new PressSimple(backClr);
            else if (radPressPlus.IsChecked == true)
                this.decorator = new PressPlus(backClr, ucDecoPressPlus.Mode, ucDecoPressPlus.Distance);
            else
                this.decorator = new FullPulse();
            decorator.Start();

            // Update control visibility
            ucDecoPressPlus.Visibility = (radPressPlus.IsChecked == true ? Visibility.Visible : Visibility.Collapsed);

            // Store settings
            storeSettings();
        }

        private void loadSettings()
        {
            // General
            this.backClr = Properties.Settings.Default.BackColor;
            updateBackClrBtn();
            var match = myGrid.Children.OfType<System.Windows.Controls.RadioButton>()
                .FirstOrDefault(r => (string)r.Content == Properties.Settings.Default.Decorator);
            if (match != null)
                match.IsChecked = true;

            // Key Press Plus
            match = ucDecoPressPlus.myGrid.Children.OfType<System.Windows.Controls.RadioButton>()
                .FirstOrDefault(r => (string)r.Content == Properties.Settings.Default.PressPlusMode);
            if (match != null)
                match.IsChecked = true;
            ucDecoPressPlus.numDistance.Value = Properties.Settings.Default.PressPlusDistance;
        }

        private void storeSettings()
        {
            // General
            Properties.Settings.Default.BackColor = backClr;
            var match = myGrid.Children.OfType<System.Windows.Controls.RadioButton>()
                .FirstOrDefault(r => r.IsChecked == true);
            if (match != null)
                Properties.Settings.Default.Decorator = (string)match.Content;

            // Key Press Plus
            Properties.Settings.Default.PressPlusMode = ucDecoPressPlus.Mode.ToString();
            Properties.Settings.Default.PressPlusDistance = ucDecoPressPlus.Distance;

            Properties.Settings.Default.Save();
        }

        #region Controls

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            clear();
        }

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {
            init();
            update();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var cd = new System.Windows.Forms.ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.backClr = cd.Color;
                updateBackClrBtn();
                update();
            }
        }

        private void updateBackClrBtn()
        {
            btnBack.Background = new SolidColorBrush(
                Color.FromArgb(255, backClr.R, backClr.G, backClr.B));
        }

        private void radPress_Checked(object sender, RoutedEventArgs e)
        {
            update();
        }

        private void radPressPlus_Checked(object sender, RoutedEventArgs e)
        {
            update();
        }

        private void radFullPulse_Checked(object sender, RoutedEventArgs e)
        {
            update();
        }

        private void ucDecoPressPlus_ParameterChanged()
        {
            update();
        }

        #endregion
    }
}

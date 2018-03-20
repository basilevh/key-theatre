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
            this.backClr = Properties.Settings.Default.BackColor;
            updateBackClrBtn();
            var match = myGrid.Children.OfType<System.Windows.Controls.RadioButton>()
                .FirstOrDefault(r => (string)r.Content == Properties.Settings.Default.Decorator);
            if (match != null)
                match.IsChecked = true;
            this.loadingCtrls = false;

            // Auto-initialize
            init();
            update();
        }

        private bool loadingCtrls = true;
        private bool sdkTitleAdded = false;
        private Decorator decorator;
        private System.Drawing.Color backClr;

        private void init()
        {
            // Init SDK
            LogitechGSDK.LogiLedInit();
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

            // Store settings
            Properties.Settings.Default.BackColor = backClr;
            Properties.Settings.Default.Save();
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
            // Stop decorator
            if (decorator != null && decorator.IsRunning)
                decorator.Stop();

            if (!loadingCtrls)
            {
                // Start decorator
                if (radPress.IsChecked == true)
                    this.decorator = new PressSimple(backClr);
                else if (radPressPlus.IsChecked == true)
                    this.decorator = new PressPlus(backClr, PressPlus.Mode.Radial);
                else
                    this.decorator = new FullPulse();
                decorator.Start();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            clear();
        }

        #region Controls

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
            ucDecoPressPlus.Visibility = (radPressPlus.IsChecked == true ? Visibility.Visible : Visibility.Collapsed);
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

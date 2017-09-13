// Started 04-11-2016, Basile Van Hoorick

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            this.loadingCtrls = false;

            // Auto-init
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
                    this.decorator = new PressPlus(backClr, PressPlus.Mode.Circle);
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
            var cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.backClr = cd.Color;
                btnBack.Background = new SolidColorBrush(
                    Color.FromArgb(255, backClr.R, backClr.G, backClr.B));
                update();
            }
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

        #endregion
    }
}

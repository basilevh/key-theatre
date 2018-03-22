using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyDecorator
{
    /// <summary>
    /// Interaction logic for ucPressPlus.xaml
    /// </summary>
    public partial class ucPressPlus : UserControl
    {
        public ucPressPlus()
        {
            InitializeComponent();
        }
        
        public delegate void ParameterChangedDelegate();

        /// <summary>
        /// Called whenever a parameter, such as a mode or a number value, has changed.
        /// </summary>
        public event ParameterChangedDelegate ParameterChanged;

        /// <summary>
        /// Returns the selected decorator mode.
        /// </summary>
        public Decorators.PressPlus.Mode Mode
        {
            get
            {
                if (radRadial.IsChecked == true)
                    return Decorators.PressPlus.Mode.Radial;
                else
                    // Default case
                    return Decorators.PressPlus.Mode.Horizontal;
            }
        }

        /// <summary>
        /// Returns the selected path distance.
        /// </summary>
        public int Distance => numDistance.Value.Value;

        private void radHorizontal_Checked(object sender, RoutedEventArgs e)
        {
            ParameterChanged?.Invoke();
        }

        private void radRadial_Checked(object sender, RoutedEventArgs e)
        {
            ParameterChanged?.Invoke();
        }

        private void numDistance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ParameterChanged?.Invoke();
        }
    }
}

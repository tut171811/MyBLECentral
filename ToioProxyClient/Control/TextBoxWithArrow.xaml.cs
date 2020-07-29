using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace ToioProxyClient.Control
{
    public sealed partial class TextBoxWithArrow : UserControl
    {
        public string Text
        {
            set
            {
                this.Box1.Text = value;
            }
            get
            {
                return this.Box1.Text;
            }
        }

        private bool isUpperChecked = true;
        public bool IsUpperChecked
        {
            set
            {
                this.isUpperChecked = value;
                Toggle(value);
            }
            get
            {
                return this.isUpperChecked;
            }
        }

        public TextBoxWithArrow()
        {
            this.InitializeComponent();
            this.UpperButton.Checked += UpperButton_Checked;
            this.LowerButton.Checked += LowerButton_Checked;
            this.UpperButton.IsChecked = true;
        }

        private void Toggle(bool isUpper)
        {
            this.UpperButton.IsChecked = isUpper;
            this.LowerButton.IsChecked = !isUpper;
            //this.Text = this.IsUpperChecked.ToString();
        }

        private void UpperButton_Checked(object sender, RoutedEventArgs e)
        {
            this.IsUpperChecked = true;
        }

        private void LowerButton_Checked(object sender, RoutedEventArgs e)
        {
            this.IsUpperChecked = false;
        }
    }
}

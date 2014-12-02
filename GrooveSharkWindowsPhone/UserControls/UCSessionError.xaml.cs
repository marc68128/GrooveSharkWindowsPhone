using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GrooveSharkWindowsPhone.UserControls
{
    public sealed partial class UCSessionError : UserControl
    {
        public event EventHandler ReloadTap;
        public UCSessionError()
        {
            this.InitializeComponent();
        }

        private void TryAgainTap(object sender, TappedRoutedEventArgs e)
        {
            if (ReloadTap != null)
                ReloadTap.Invoke(this, null);
            e.Handled = true;
        }
    }
}

using System.Windows;

namespace hardwareToggle {
    public partial class ErrorMessageBox : Window {
        public ErrorMessageBox(string message) {
            InitializeComponent();
            MessageTextBlock.Text = message;
            ShowDialog();
        }

        private void OKButton_Clicked(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}

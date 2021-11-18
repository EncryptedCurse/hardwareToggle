using System.Windows;

namespace hardwareToggle {
    public partial class ErrorMsgBox : Window {
        public string Message1 { get; private set; }
        public string Message2 { get; private set; }

        public ErrorMsgBox(string message1, string message2 = null) {
            Message1 = message1;
            Message2 = message2;
            InitializeComponent();
            ShowDialog();
        }
    }
}

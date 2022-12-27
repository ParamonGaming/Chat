using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace Client.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для BindPasswordBox.xaml
    /// </summary>
    public partial class BindPasswordBox : UserControl
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(BindPasswordBox));
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        public BindPasswordBox()
        {
            InitializeComponent();
            txtPassword.PasswordChanged += OnPasswordChanged;
        }
        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = txtPassword.Password;
        }
    }
}

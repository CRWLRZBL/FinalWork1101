using ServicesLibrary;
using ServicesLibrary.Services;
using System.Windows;
using System.Windows.Controls;

namespace FinalWPF.Pages
{
    public partial class AuthorizationPage : Page
    {
        public static readonly ExamUserService _userService = new();

        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private async void AuthorizeButton_Click(object sender, RoutedEventArgs e)
        {
            var user = await _userService.GetUserByLoginAndPasswordAsync(loginTextBox.Text, passPasswordBox.Password);
            if (user != null)
            {
                RunUser.IsGuest = false;
                RunUser.UserID = user.UserId;
                RunUser.RoleID = user.RoleId;
                RunUser.UserSurname = user.UserSurname;
                RunUser.UserName = user.UserName;
                RunUser.UserPatronymic = user.UserPatronymic;
                RunUser.UserLogin = user.UserLogin;
                RunUser.UserPassword = user.UserPassword;
                App.CurrentFrame.Navigate(new ShopPage());
            }
            else
                MessageBox.Show("Login or password are incorrect");
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            RunUser.IsGuest = true;
            RunUser.RoleID = 2;
            App.CurrentFrame.Navigate(new ShopPage());
        }
    }
}

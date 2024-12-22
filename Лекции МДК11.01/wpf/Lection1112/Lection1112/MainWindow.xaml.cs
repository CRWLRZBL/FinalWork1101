using Lection1112.Models;
using Lection1112.Services;
using System.Windows;
using System.Windows.Controls;

namespace Lection1112
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GameService _service = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task LoadGames()
        {
            try
            {
                GamesDataGrid.ItemsSource = await _service.GetGamesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }         
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {          
            var selectedGames = GamesDataGrid.SelectedItems.Cast<Game>();
            foreach (Game game in selectedGames)
                await _service.DeleteGameAsync(game);
            await LoadGames();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadGames();
        }
    }
}
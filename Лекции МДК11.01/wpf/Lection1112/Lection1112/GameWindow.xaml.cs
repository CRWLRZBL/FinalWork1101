using Lection1112.Models;
using Lection1112.Services;
using System.Text;
using System.Windows;

namespace Lection1112
{
    /// <summary>
    /// Логика взаимодействия для GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private readonly GameService _service = new();
        private Game _game;
        public GameWindow(Game? game = null)
        {
            InitializeComponent();

            _game = game ?? new();
            DataContext = _game;
            LoadCategories();
        }

        private async Task LoadCategories()
        {
            // add exceptions while working with other data (SSMS) (koro4e ne c dannymi VS)
            CategoryComboBox.ItemsSource = await _service.GetCategoriesAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new();
            if (CategoryComboBox.SelectedItem == null)
                errors.AppendLine("Нужно выбрать категорию");
            if (String.IsNullOrEmpty(_game.Name))
                errors.AppendLine("Нужно указать имя");
            if (_game.Price < 0 || _game.Price > 5000)
                errors.AppendLine("Цена должна быть в диапозоне от 0 до 5000");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            try
            {
                if (_game.GameId == 0)
                    await _service.AddGameAsync(_game);
                else
                    await _service.UpdateGameAsync(_game);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
        }
    }
}

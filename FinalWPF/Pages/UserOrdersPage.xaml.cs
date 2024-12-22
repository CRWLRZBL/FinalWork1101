using Microsoft.Win32;
using ServicesLibrary;
using ServicesLibrary.Models;
using ServicesLibrary.Services;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using Label = System.Windows.Controls.Label;
using Orientation = System.Windows.Controls.Orientation;

namespace FinalWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class UserOrdersPage : Page
    {
        public static List<ExamOrder> createdByGuestOrdersList = new();
        public static List<ExamOrder> examCreatedOrdersList = new();
        public static readonly ExamOrderService _orderService = new();
        public static readonly ExamOrderProductService _orderProductService = new();
        public static readonly ExamProductService _productService = new();

        public UserOrdersPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //гость? => заполняется createdByGuest
            examCreatedOrdersList = RunUser.IsGuest ? createdByGuestOrdersList : await _orderService.GetOrdersByUserIdAsync(RunUser.UserID);

            GetUserOrderList();

            if (RunUser.IsGuest)
                RunUserLabel.Content = "Guest";
            else
                RunUserLabel.Content = $"{RunUser.UserName.Substring(0, 1)}.{RunUser.UserPatronymic.Substring(0, 1)}. {RunUser.UserSurname}";

            if (RunUser.RoleID != 2)//доступ к кнопке для менеджера и администратора
                GoToAllOrdersButton.Visibility = Visibility.Visible;
            else
                GoToAllOrdersButton.Visibility = Visibility.Collapsed;
        }

        private async void GetUserOrderList()
        {
            UserOrdersStackPanel.Children.Clear();
            int productsCount = examCreatedOrdersList.Count;
            for (int i = 0; i < productsCount; i++)//создание элементов для хранения данных о товарах
            {
                List<ExamOrderProduct> examOrderProducts = await _orderProductService.GetProductsInOrder(examCreatedOrdersList[i].OrderId);

                Border orderBorder = new();
                orderBorder.Width = 600;
                orderBorder.Margin = new Thickness(80, 5, 0, 5);
                orderBorder.BorderThickness = new(3);

                StackPanel orderPanel = new();
                orderPanel.Tag = i;

                Label orderIdLabel = new();
                orderIdLabel.Content = $"Order №{examCreatedOrdersList[i].OrderId}";
                orderPanel.Children.Add(orderIdLabel);

                Label orderDateLabel = new Label();
                orderDateLabel.Content = $"Date: {examCreatedOrdersList[i].OrderDate}";
                orderPanel.Children.Add(orderDateLabel);

                StackPanel orderContentsPanel = new();
                orderContentsPanel.Orientation = Orientation.Horizontal;
                Label orderContentsLabel = new();
                string orderContents = "";
                for (int j = 0; j < examOrderProducts.Count; j++)
                {
                    orderContents += $"\n{await _productService.GetProductNameByArticleAsync(examOrderProducts[j].ProductArticleNumber)}" +
                        $"({await _orderProductService.GetProductAmountInOrderWithArticle(examCreatedOrdersList[i].OrderId, examOrderProducts[j].ProductArticleNumber)})";
                }
                orderContentsLabel.Content = $"Contents:{orderContents}";
                orderContentsPanel.Children.Add(orderContentsLabel);
                orderPanel.Children.Add(orderContentsPanel);

                Label orderSumLabel = new();
                decimal? orderSum = await _orderProductService.GetSumOrder(examCreatedOrdersList[i].OrderId);
                string orderSumStr = orderSum.HasValue ? orderSum.Value.ToString("F2") : "0.00";
                orderSumLabel.Content = $"Total price: " + orderSumStr;
                orderPanel.Children.Add(orderSumLabel);

                Label orderDiscountLabel = new();
                decimal? orderDiscount = await _orderProductService.GetDiscountOrder(examCreatedOrdersList[i].OrderId);
                string orderDiscontStr = orderDiscount.HasValue ? orderDiscount.Value.ToString("F2") : "0.00";
                orderDiscountLabel.Content = $"Total discount: " + orderDiscontStr;
                orderPanel.Children.Add(orderDiscountLabel);

                Label orderPickupPointLabel = new();
                orderPickupPointLabel.Content = $"Pick up point: {examCreatedOrdersList[i].OrderPickupPoint}";
                orderPanel.Children.Add(orderPickupPointLabel);

                DockPanel dockPanel = new();
                Label orderPickupCodeLabel = new();
                orderPickupCodeLabel.Content = $"Code: {examCreatedOrdersList[i].OrderPickupCode}";
                Button saveOrderButton = new();
                saveOrderButton.Content = "Save";
                saveOrderButton.Margin = new(5);
                saveOrderButton.BorderThickness = new Thickness(4);
                saveOrderButton.Click += SaveOrderButton_Click;
                saveOrderButton.Tag =
                    $"Order №{examCreatedOrdersList[i].OrderId}\n" +
                    $"Date: {examCreatedOrdersList[i].OrderDate}\n" +
                    $"Contents:{orderContents}\n" +
                    $"Total price: {(orderSum.HasValue ? orderSum.Value.ToString("F2") : "0.00")}\n" +
                    $"Total discount: {(orderDiscount.HasValue ? orderDiscount.Value.ToString("F2") : "0.00")}\n" +
                    $"Pick up point: {examCreatedOrdersList[i].OrderPickupPoint}\n" +
                    $"Code: {examCreatedOrdersList[i].OrderPickupCode}";
                DockPanel.SetDock(saveOrderButton, Dock.Right);
                dockPanel.Children.Add(saveOrderButton);
                dockPanel.Children.Add(orderPickupCodeLabel);
                orderPanel.Children.Add(dockPanel);

                orderBorder.Child = orderPanel;
                UserOrdersStackPanel.Children.Add(orderBorder);
            }
        }

        private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
        {
            Button saveOrderButton = (Button)sender;
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";

            if (saveFileDialog.ShowDialog() == true)
            {
                string textToSave = saveOrderButton.Tag.ToString();
                File.WriteAllText(saveFileDialog.FileName, textToSave);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) => App.CurrentFrame.Navigate(new ShopPage());

        private void GoToAllOrdersButton_Click(object sender, RoutedEventArgs e) => App.CurrentFrame.Navigate(new AllOrdersPage());
    }
}

using ServicesLibrary;
using ServicesLibrary.Models;
using ServicesLibrary.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FinalWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для AllOrdersPage.xaml
    /// </summary>
    public partial class AllOrdersPage : Page
    {
        public static List<ExamOrder> examOrders = new();
        public static readonly ExamOrderService _orderService = new();
        public static readonly ExamOrderProductService _orderProductService = new();
        public static readonly ExamProductService _productService = new();
        public static readonly ExamUserService _userService = new();

        public AllOrdersPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetOrderList();
            RunUserLabel.Content = $"{RunUser.UserName.Substring(0, 1)}.{RunUser.UserPatronymic.Substring(0, 1)}. {RunUser.UserSurname}";
        }

        private async void GetOrderList()
        {
            AllOrdersStackPanel.Children.Clear();
            examOrders = await _orderService.GetOrdersWithTotalCost();
            int productsCount = examOrders.Count;
            for (int i = 0; i < productsCount; i++)
            {
                List<ExamOrderProduct> examOrderProducts = await _orderProductService.GetProductsInOrder(examOrders[i].OrderId);

                Border orderBorder = new();
                orderBorder.Tag = examOrders[i].OrderId.ToString();
                orderBorder.Width = 600;
                orderBorder.Margin = new Thickness(80, 5, 0, 5);
                orderBorder.BorderThickness = new(3);

                StackPanel orderPanel = new();
                DockPanel statusPanel = new();
                Label orderIdLabel = new();
                orderIdLabel.Content = $"Order {examOrders[i].OrderId}";
                Label orderStatus = new();
                DockPanel.SetDock(orderStatus, Dock.Right);
                orderStatus.Content = examOrders[i].OrderStatus;
                statusPanel.Children.Add(orderStatus);
                statusPanel.Children.Add(orderIdLabel);
                orderPanel.Children.Add(statusPanel);

                StackPanel orderCompositionPanel = new();
                orderCompositionPanel.Orientation = Orientation.Horizontal;
                Label orderCompositionLabel = new();
                string orderComposition = "";
                for (int j = 0; j < examOrderProducts.Count; j++)
                {
                    var product = await _productService.GetProductNameByArticleAsync(examOrderProducts[j].ProductArticleNumber);
                    orderComposition += $"\n{product}({await _orderProductService.GetProductAmountInOrderWithArticle(examOrders[i].OrderId, examOrderProducts[j].ProductArticleNumber)})";
                }
                orderCompositionLabel.Content = $"Contents:{orderComposition}";
                orderCompositionPanel.Children.Add(orderCompositionLabel);
                orderPanel.Children.Add(orderCompositionPanel);

                Label orderSumLabel = new();
                decimal? orderSum = await _orderProductService.GetSumOrder(examOrders[i].OrderId);
                string orderSumStr = orderSum.HasValue ? orderSum.Value.ToString("F2") : "0.00";
                orderSumLabel.Content = $"Total: " + orderSumStr;
                orderPanel.Children.Add(orderSumLabel);

                Label orderDiscountLabel = new();
                decimal? orderDiscount = await _orderProductService.GetDiscountOrder(examOrders[i].OrderId);
                string orderDiscontStr = orderDiscount.HasValue ? orderDiscount.Value.ToString("F2") : "0.00";
                orderDiscountLabel.Content = $"Total discount: " + orderDiscontStr;
                orderPanel.Children.Add(orderDiscountLabel);

                if (examOrders[i].UserId != 0)
                {
                    Label orderPickupPointLabel = new();
                    string? fullName = await _userService.GetUserFullNameWithOrderIdAsync(examOrders[i].OrderId);
                    orderPickupPointLabel.Content = !string.IsNullOrWhiteSpace(fullName) ? $"Full name: {fullName}" : "Unauthorized";
                    orderPanel.Children.Add(orderPickupPointLabel);
                }

                Label orderDateLabel = new();
                orderDateLabel.Content = $"Date: {examOrders[i].OrderDate}";
                orderPanel.Children.Add(orderDateLabel);

                StackPanel DeliveryStackPanel = new();
                DeliveryStackPanel.Orientation = Orientation.Horizontal;
                Label orderDeliveryDateLabel = new();
                orderDeliveryDateLabel.Content = $"Delivery date:\n{examOrders[i].OrderDeliveryDate:yyyy-MM-dd}";
                StackPanel ChangeDeliveryDateStackPanel = new();
                Label changeDeliveryDateLabel = new();
                changeDeliveryDateLabel.Content = "Change delivery date:";
                TextBox changeDeliveryDateTextBox = new();
                changeDeliveryDateTextBox.Name = "changeTextBox";
                Button changeButton = new();
                changeButton.Tag = examOrders[i].OrderId;
                changeButton.Content = "Change";
                changeButton.Click += ChangeButton_Click;
                StackPanel StatusStackPanel = new();
                Label changeStatusLabel = new();
                changeStatusLabel.Content = "Status:";
                ComboBox changeStatusComboBox = new();
                changeStatusComboBox.Tag = examOrders[i].OrderId;
                changeStatusComboBox.Items.Add("New");
                changeStatusComboBox.Items.Add("Completed");
                changeStatusComboBox.SelectionChanged += ChangeStatusComboBox_SelectionChanged;
                StatusStackPanel.Children.Add(changeStatusLabel);
                StatusStackPanel.Children.Add(changeStatusComboBox);
                ChangeDeliveryDateStackPanel.Children.Add(changeDeliveryDateLabel);
                ChangeDeliveryDateStackPanel.Children.Add(changeDeliveryDateTextBox);
                ChangeDeliveryDateStackPanel.Children.Add(changeButton);
                DeliveryStackPanel.Children.Add(orderDeliveryDateLabel);
                DeliveryStackPanel.Children.Add(ChangeDeliveryDateStackPanel);
                DeliveryStackPanel.Children.Add(StatusStackPanel);
                orderPanel.Children.Add(DeliveryStackPanel);

                orderBorder.Child = orderPanel;
                AllOrdersStackPanel.Children.Add(orderBorder);
            }
        }

        private async void ChangeStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedIndex == 0)
                await _orderService.UpdateExamOrderStatus("New", Convert.ToInt32(comboBox.Tag));
            else
                await _orderService.UpdateExamOrderStatus("Completed", Convert.ToInt32(comboBox.Tag));
            GetOrderList();
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            Button changeButton = sender as Button;
            StackPanel stackPanel = changeButton.Parent as StackPanel;
            TextBox newDeliveryDateTextBox = stackPanel.Children.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "changeTextBox");
            try
            {
                _orderService.UpdateExamOrderDeliveryDate(Convert.ToDateTime(newDeliveryDateTextBox.Text), Convert.ToInt32(changeButton.Tag));
                MessageBox.Show("Delivery date has been changed");
            }
            catch
            {
                MessageBox.Show("Incorrect format");
            }
            GetOrderList();
        }

        private void SearchByIdButton_Click(object sender, RoutedEventArgs e)
        {
            var targetBorder = AllOrdersStackPanel.Children
                                    .OfType<Border>()
                                    .FirstOrDefault(b => b.Tag.ToString() == IdTextBox.Text);

            if (targetBorder != null)
            {
                var scrollViewer = FindVisualParent<ScrollViewer>(AllOrdersStackPanel);
                if (scrollViewer != null)
                {
                    var position = targetBorder.TransformToAncestor(AllOrdersStackPanel)
                                                .Transform(new Point(0, 0));
                    scrollViewer.ScrollToVerticalOffset(position.Y);
                }
            }
            else
            {
                MessageBox.Show("Order code not found");
            }
        }

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            return parent != null ? parent : FindVisualParent<T>(parentObject);
        }

        private void ToStartButton_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = FindVisualParent<ScrollViewer>(AllOrdersStackPanel);
            scrollViewer.ScrollToVerticalOffset(0);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) => App.CurrentFrame.GoBack();
    }
}

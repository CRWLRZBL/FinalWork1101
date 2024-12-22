using ServicesLibrary;
using ServicesLibrary.DTOs;
using ServicesLibrary.Models;
using ServicesLibrary.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FinalWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrderPage.xaml
    /// </summary>
    public partial class MakeOrderPage : Page
    {
        public static List<ProductDTO> ExamOrderList = new();
        public static int orderInProductsCount;
        public static decimal? totalCost;
        public static decimal? totalDiscount;
        public static List<ExamPickupPoint> examPickupPoints = new();
        public static List<int> existingPickupCodes = new();
        public static readonly ExamPickupPointService _pickupPointService = new();
        public static readonly ExamOrderService _orderService = new();
        public static readonly ExamOrderProductService _orderProductService = new();

        public MakeOrderPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateOrderList();

            examPickupPoints = await _pickupPointService.GetPickupPointsAsync();
            existingPickupCodes = await _orderService.GetExistingPickupCodesAsync();

            if (RunUser.IsGuest)
                CurrentUserLabel.Content = "Guest";
            else
                CurrentUserLabel.Content = $"{RunUser.UserName.Substring(0, 1)}.{RunUser.UserPatronymic.Substring(0, 1)}. {RunUser.UserSurname}";

            PickupPointsComboBox.ItemsSource = examPickupPoints;

            if (RunUser.RoleID != 2)//доступ к кнопке для менеджера и администратора
                GoToAllOrdersButton.Visibility = Visibility.Visible;
            else
                GoToAllOrdersButton.Visibility = Visibility.Collapsed;
        }

        private void CreateOrderList()
        {
            productsInOrderStackPanel.Children.Clear();
            int productsCount = ExamOrderList.Count;
            for (int i = 0; i < productsCount; i++)//создание элементов данных о товарах в корзине
            {
                Border productBorder = new();
                productBorder.Width = 600;
                productBorder.Margin = new Thickness(80, 5, 0, 5);
                productBorder.BorderThickness = new(3);

                StackPanel productPanel = new();
                productPanel.Tag = i;

                StackPanel articleNumberPanel = new();
                articleNumberPanel.Orientation = Orientation.Horizontal;
                Label articleNumberLabel = new();
                articleNumberLabel.Content = "Article:";
                Label articleDataLabel = new();
                articleDataLabel.Content = ExamOrderList[i].ProductArticleNumber;
                articleNumberPanel.Children.Add(articleNumberLabel);
                articleNumberPanel.Children.Add(articleDataLabel);
                productPanel.Children.Add(articleNumberPanel);

                Label nameDataLabel = new();
                nameDataLabel.Content = ExamOrderList[i].ProductName;
                productPanel.Children.Add(nameDataLabel);

                Label desciptionDataLabel = new();
                desciptionDataLabel.Content = ExamOrderList[i].ProductDescription;
                productPanel.Children.Add(desciptionDataLabel);

                StackPanel categoryPanel = new();
                categoryPanel.Orientation = Orientation.Horizontal;
                Label categoryLabel = new();
                categoryLabel.Content = "Category:";
                Label categoryDataLabel = new();
                categoryDataLabel.Content = ExamOrderList[i].ProductCategory;
                categoryPanel.Children.Add(categoryLabel);
                categoryPanel.Children.Add(categoryDataLabel);
                productPanel.Children.Add(categoryPanel);

                StackPanel manufacturerPanel = new StackPanel();
                manufacturerPanel.Orientation = Orientation.Horizontal;
                Label manufacturerLabel = new();
                manufacturerLabel.Content = "Manufacturer:";
                Label manufacturerDataLabel = new();
                manufacturerDataLabel.Content = ExamOrderList[i].ProductManufacturer;
                manufacturerPanel.Children.Add(manufacturerLabel);
                manufacturerPanel.Children.Add(manufacturerDataLabel);
                productPanel.Children.Add(manufacturerPanel);

                DockPanel costDockPanel = new DockPanel();
                Label costLabel = new();
                costLabel.Content = "Price:";
                TextBlock costDataTextBlock = new TextBlock();//для зачеркнутости текста
                costDataTextBlock.Text = ExamOrderList[i].ProductCost.ToString();
                Label discountLabel = new();
                discountLabel.Content = $"Discount:";
                discountLabel.FontSize = 12;
                Label discountDataLabel = new();
                discountDataLabel.FontSize = 12;
                discountDataLabel.Content = ExamOrderList[i].ProductDiscountAmount;
                costDockPanel.Children.Add(costLabel);
                costDockPanel.Children.Add(discountDataLabel);
                DockPanel.SetDock(discountDataLabel, Dock.Right);
                costDockPanel.Children.Add(discountLabel);
                DockPanel.SetDock(discountLabel, Dock.Right);
                if (ExamOrderList[i].ProductDiscountAmount > 0)
                {
                    costDataTextBlock.TextDecorations = TextDecorations.Strikethrough;
                    costDataTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
                    Label costWithDiscountDataLabel = new();
                    decimal resultCost = (decimal)Convert.ToDouble(costDataTextBlock.Text) * (100 - Convert.ToInt32(discountDataLabel.Content)) / 100;
                    costWithDiscountDataLabel.Content = resultCost;
                    costDockPanel.Children.Add(costWithDiscountDataLabel);
                }
                costDockPanel.Children.Add(costDataTextBlock);
                productPanel.Children.Add(costDockPanel);

                StackPanel productStatusPanel = new();
                productStatusPanel.Orientation = Orientation.Horizontal;
                Label productStatusLabel = new();
                productStatusLabel.Content = "Status:";
                Label productStatusDataLabel = new();
                productStatusDataLabel.Content = ExamOrderList[i].ProductStatus;
                productStatusPanel.Children.Add(productStatusLabel);
                productStatusPanel.Children.Add(productStatusDataLabel);
                productPanel.Children.Add(productStatusPanel);

                StackPanel productQuantityInStockPanel = new();
                productQuantityInStockPanel.Orientation = Orientation.Horizontal;
                Label productQuantityInStockLabel = new();
                productQuantityInStockLabel.Content = "Amount:";
                Label productQuantityInStockDataLabel = new();
                productQuantityInStockDataLabel.Content = ExamOrderList[i].ProductQuantityInStock;
                productQuantityInStockPanel.Children.Add(productQuantityInStockLabel);
                productQuantityInStockPanel.Children.Add(productQuantityInStockDataLabel);
                productPanel.Children.Add(productQuantityInStockPanel);

                DockPanel countPanel = new();
                CountControl countControl = new();
                countControl.Tag = i;
                countControl.HorizontalAlignment = HorizontalAlignment.Left;
                countControl.Value = ExamOrderList[i].ProductCountInOrder;
                countControl.MaxValue = ExamOrderList[i].ProductQuantityInStock;
                countControl.countTextBox.Text = ExamOrderList[i].ProductCountInOrder.ToString();
                countControl.ValueChanged += CountControl_ValueChanged;
                Button deleteButton = new();
                deleteButton.Click += DeleteButton_Click;
                Image deleteImage = new();
                deleteButton.Width = 50;
                deleteButton.HorizontalAlignment = HorizontalAlignment.Right;
                deleteImage.Source = new BitmapImage(new Uri("C:\\FinalWorkJiEs\\FinalWeb-API\\FinalWPF\\Images\\delete.png"));
                deleteButton.Content = deleteImage;
                DockPanel.SetDock(deleteButton, Dock.Right);
                countPanel.Children.Add(deleteButton);
                countPanel.Children.Add(countControl);
                productPanel.Children.Add(countPanel);

                productBorder.Child = productPanel;
                productsInOrderStackPanel.Children.Add(productBorder);
            }
            UpdateProductsCount();
            UpdateDiscount();
            UpdateCost();
        }

        private void UpdateProductsCount()
        {
            int productsCount = ExamOrderList.Count;
            orderInProductsCount = 0;
            for (int i = 0; i < productsCount; i++)
            {
                orderInProductsCount += ExamOrderList[i].ProductCountInOrder;
                CountProductsInOrderLabel.Content = orderInProductsCount.ToString();
            }
            if (ExamOrderList.Count == 0)
                CountProductsInOrderLabel.Content = 0;
        }

        private void UpdateDiscount()
        {
            int productsCount = ExamOrderList.Count;
            totalDiscount = 0;
            for (int i = 0; i < productsCount; i++)
            {
                totalDiscount += (ExamOrderList[i].ProductCost - ExamOrderList[i].TotalCost) * ExamOrderList[i].ProductCountInOrder;
                string totalDiscountStr = totalDiscount.HasValue ? totalDiscount.Value.ToString("F2") : "0.00";
                OrderDiscountLabel.Content = totalDiscountStr + " rub.";
            }
            if (ExamOrderList.Count == 0)
                OrderDiscountLabel.Content = 0;
        }

        private void UpdateCost()
        {
            int productsCount = ExamOrderList.Count;
            totalCost = 0;
            for (int i = 0; i < productsCount; i++)
            {
                totalCost += ExamOrderList[i].TotalCost * ExamOrderList[i].ProductCountInOrder;
                string totalCostStr = totalCost.HasValue ? totalCost.Value.ToString("F2") : "0.00";
                OrderCostLabel.Content = totalCostStr + " rub.";
            }
            if (ExamOrderList.Count == 0)
                OrderCostLabel.Content = 0;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;
            DockPanel countPanel = deleteButton.Parent as DockPanel;
            StackPanel productPanel = countPanel.Parent as StackPanel;
            StackPanel productsInOrderStackPanel = productPanel.Parent as StackPanel;
            ExamOrderList.RemoveAt((int)productPanel.Tag);
            if (productsInOrderStackPanel != null)
                productsInOrderStackPanel.Children.Remove(productPanel);
            CreateOrderList();
        }

        private void CountControl_ValueChanged(object sender, RoutedEventArgs e)
        {
            CountControl countControl = sender as CountControl;
            ExamOrderList[Convert.ToInt32(countControl.Tag)].ProductCountInOrder = countControl.Value;
            UpdateProductsCount();
            UpdateCost();
            UpdateDiscount();
            if (countControl.Value == 0)
            {
                DockPanel countPanel = countControl.Parent as DockPanel;
                StackPanel productPanel = countPanel.Parent as StackPanel;
                StackPanel productsInOrderStackPanel = productPanel.Parent as StackPanel;
                ExamOrderList.RemoveAt((int)productPanel.Tag);
                productsInOrderStackPanel?.Children.Remove(productPanel);
                CreateOrderList();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) => App.CurrentFrame.GoBack();

        private async void MakeOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExamOrderList.Count != 0)
            {
                if (PickupPointsComboBox.SelectedItem != null)
                {
                    Random rnd = new Random();
                    int travelDays = rnd.Next(3, 8);
                    DateTime currentDate = DateTime.Now;
                    DateTime deliveryDate = currentDate.AddDays(travelDays);

                    int pickupCode;
                    do
                    {
                        pickupCode = rnd.Next(100, 1000);
                    }
                    while (existingPickupCodes.Contains(pickupCode));

                    await _orderService.AddExamOrderAsync(RunUser.UserID, "New", currentDate, deliveryDate, examPickupPoints[PickupPointsComboBox.SelectedIndex].PickupPointId, pickupCode);
                    var newOrder = await _orderService.GetLastOrderAsync();

                    for (int i = 0; i < ExamOrderList.Count; i++)
                    {
                        await _orderProductService.AddOrderProductAsync(newOrder.OrderId, ExamOrderList[i].ProductArticleNumber, ExamOrderList[i].ProductCountInOrder);
                    }

                    //запись для гостя
                    if (RunUser.IsGuest)
                    {
                        UserOrdersPage.createdByGuestOrdersList.Add(newOrder);
                    }

                    ExamOrderList.Clear();
                    productsInOrderStackPanel.Children.Clear();
                    App.CurrentFrame.Navigate(new UserOrdersPage());

                }
                else
                {
                    MessageBox.Show("Choose pick up point");
                    WarnLabel.Content = "Error";
                }
            }
            else
            {
                MessageBox.Show("Order is empty");
                WarnLabel.Content = "Error";
            }
        }

        private void PickupPointsComboBox_GotFocus(object sender, RoutedEventArgs e) => WarnLabel.Content = string.Empty;

        private void GoToYourOrders_Click(object sender, RoutedEventArgs e) => App.CurrentFrame.Navigate(new UserOrdersPage());

        private void GoToAllOrdersButton_Click(object sender, RoutedEventArgs e) => App.CurrentFrame.Navigate(new AllOrdersPage());
    }
}

﻿using ServicesLibrary;
using ServicesLibrary.DTOs;
using ServicesLibrary.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FinalWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для ShopPage.xaml
    /// </summary>
    public partial class ShopPage : Page
    {
        public decimal MinCost { get; set; } = 0;
        public decimal? MaxCost { get; set; } = null;
        public string SortMethod { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string SearchedText { get; set; } = "";

        public static bool _isProgr = false;
        public bool searchTextBoxIsFill = false;
        public static List<ProductDTO> examProducts = new();
        public static readonly ExamProductService _productService = new();

        public ShopPage()
        {
            InitializeComponent();
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ManufacturerFilterComboBox.ItemsSource = await _productService.GetManufacturersAsync();
            CreateProductsList(SearchedText, SortMethod, MinCost, MaxCost, Manufacturer);
            searchTextBox.Foreground = new SolidColorBrush(Colors.Gray);

            if (RunUser.IsGuest)
                CurrentUserLabel.Content = "Guest";
            else
                CurrentUserLabel.Content = $"{RunUser.UserName.Substring(0, 1)}.{RunUser.UserPatronymic.Substring(0, 1)}. {RunUser.UserSurname}";

            if (MakeOrderPage.ExamOrderList.Count == 0)
                orderButton.Visibility = Visibility.Hidden;

            if (RunUser.RoleID != 2)
                GoToAllOrdersButton.Visibility = Visibility.Visible;
            else
                GoToAllOrdersButton.Visibility = Visibility.Collapsed;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isProgr)
            {
                if (searchTextBox.Text != string.Empty)
                {
                    searchTextBoxIsFill = true;
                }
                else
                {
                    SearchedText = searchTextBox.Text;
                    CreateProductsList(SearchedText, SortMethod, MinCost, MaxCost, Manufacturer);
                    searchTextBoxIsFill = false;
                }
                if (searchedProductNameLabel != null)
                {
                    searchedProductNameLabel.Content = searchTextBox.Text;
                    searchedProductNameLabel.Content = searchTextBox.Text.Length > 7 ? searchTextBox.Text.Substring(0, 7) + "..." : searchTextBox.Text;
                }
                if (searchTextBoxIsFill)
                {
                    SearchedText = searchTextBox.Text;
                    CreateProductsList(SearchedText, SortMethod, MinCost, MaxCost, Manufacturer);
                }
            }
        }

        private void Page_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!searchTextBox.IsFocused && searchTextBox.Text == string.Empty)
            {
                searchTextBox.TextChanged -= SearchTextBox_TextChanged;
                searchTextBox.Foreground = new SolidColorBrush(Colors.Gray);
                searchTextBox.Text = "Search on Fragrance World";
                searchTextBox.TextChanged += SearchTextBox_TextChanged;
                searchedProductNameLabel.Content = string.Empty;
                searchTextBoxIsFill = false;
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBoxIsFill == false)
            {
                searchTextBox.Text = string.Empty;
                searchTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private async void CreateProductsList(string subs, string sortMethod, decimal min, decimal? max, string manufacturer)
        {
            examProducts.Clear();
            productsStackPanel.Children.Clear();
            examProducts = await _productService.GetProductsAsync(subs, sortMethod, min, max, manufacturer);
            int productsCount = examProducts.Count;
            for (int i = 0; i < productsCount; i++)
            {
                Border productBorder = new();
                productBorder.Width = 600;
                productBorder.Margin = new Thickness(80, 5, 0, 5);
                if (examProducts[i].ProductQuantityInStock == 0)
                {
                    productBorder.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                }
                productBorder.BorderThickness = new(3);

                StackPanel productPanel = new();

                if (examProducts[i].ProductQuantityInStock == 0)
                {
                    productPanel.Background = new SolidColorBrush(Colors.Gray);
                }

                Label nameDataLabel = new();
                nameDataLabel.Content = examProducts[i].ProductName;
                if (examProducts[i].ProductQuantityInStock == 0)
                {
                    nameDataLabel.Content = examProducts[i].ProductName + " (Out of stock)";
                }
                productPanel.Children.Add(nameDataLabel);

                Label desciptionDataLabel = new();
                desciptionDataLabel.Content = examProducts[i].ProductDescription;
                productPanel.Children.Add(desciptionDataLabel);

                StackPanel manufacturerPanel = new();
                manufacturerPanel.Orientation = Orientation.Horizontal;
                Label manufacturerLabel = new();
                manufacturerLabel.Content = "Manufacturer:";
                Label manufacturerDataLabel = new();
                manufacturerDataLabel.Content = examProducts[i].ProductManufacturer;
                manufacturerPanel.Children.Add(manufacturerLabel);
                manufacturerPanel.Children.Add(manufacturerDataLabel);
                productPanel.Children.Add(manufacturerPanel);

                DockPanel costDockPanel = new();
                Label costLabel = new();
                costLabel.Content = "Cost:";
                TextBlock costDataTextBlock = new();
                costDataTextBlock.Text = examProducts[i].ProductCost.ToString();
                Button addButton = new();
                addButton.Click += AddProduct_Click;
                addButton.Content = $"Offer";
                addButton.FontSize = 12;
                costDockPanel.Children.Add(costLabel);
                costDockPanel.Children.Add(addButton);
                DockPanel.SetDock(addButton, Dock.Right);
                costDockPanel.Children.Add(costDataTextBlock);
                if (examProducts[i].ProductDiscountAmount > 0)
                {
                    costDataTextBlock.TextDecorations = TextDecorations.Strikethrough;
                    costDataTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
                    Label costWithDiscountDataLabel = new();
                    decimal resultCost = (decimal)Convert.ToDouble(costDataTextBlock.Text) * (100 - Convert.ToInt32(examProducts[i].ProductDiscountAmount)) / 100;
                    costWithDiscountDataLabel.Content = resultCost;
                    costDockPanel.Children.Add(costWithDiscountDataLabel);
                }
                productPanel.Children.Add(costDockPanel);

                if (examProducts[i].ProductQuantityInStock == 0)
                    addButton.IsEnabled = false;
                productPanel.Tag = i.ToString();

                productBorder.Child = productPanel;
                productsStackPanel.Children.Add(productBorder);
                searchedProductsCount.Content = productsStackPanel.Children.Count + " / " + await _productService.GetProductsCountAsync();
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            Button addButton = sender as Button;
            if (addButton != null)
            {
                DockPanel costDockPanel = addButton.Parent as DockPanel;
                if (costDockPanel != null)
                {
                    StackPanel productPanel = costDockPanel.Parent as StackPanel;
                    if (productPanel != null && productPanel.Tag != null)
                    {
                        var existingProduct = MakeOrderPage.ExamOrderList.FirstOrDefault(p => p.ProductName == examProducts[Convert.ToInt32(productPanel.Tag)].ProductName);
                        if (existingProduct != null)
                        {
                            existingProduct.ProductCountInOrder += 1;
                        }
                        else
                        {
                            examProducts[Convert.ToInt32(productPanel.Tag)].ProductCountInOrder += 1;
                            MakeOrderPage.ExamOrderList.Add(examProducts[Convert.ToInt32(productPanel.Tag)]);
                            orderButton.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        private void ManufactorerFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ManufacturerFilterComboBox.SelectedIndex != -1)
            {
                GetProductsWithFilter();
            }
        }

        private void GetProductsWithFilter()
        {
            if (SortComboBox.SelectedIndex == 0)
                SortMethod = "asc";
            else if (SortComboBox.SelectedIndex == 1)
                SortMethod = "desc";

            Manufacturer = ManufacturerFilterComboBox.SelectedIndex != 0 ? $"{ManufacturerFilterComboBox.SelectedItem}" : "";
            CreateProductsList(SearchedText, SortMethod, MinCost, MaxCost, Manufacturer);
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox.SelectedIndex != -1)
            {
                GetProductsWithFilter();
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e) => App.CurrentFrame.Navigate(new MakeOrderPage());

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentFrame.Navigate(new AuthorizationPage());
            MakeOrderPage.ExamOrderList.Clear();
            UserOrdersPage.examCreatedOrdersList.Clear();
        }

        private void GoToUserOrders_Click(object sender, RoutedEventArgs e) => App.CurrentFrame.Navigate(new UserOrdersPage());

        private void GoToAllOrders_Click(object sender, RoutedEventArgs e) => App.CurrentFrame.Navigate(new AllOrdersPage());

        private void FilterOff_Click(object sender, RoutedEventArgs e)
        {
            _isProgr = true;
            searchTextBox.Text = string.Empty;
            SearchedText = string.Empty;
            searchedProductNameLabel.Content = string.Empty;
            SortComboBox.SelectedIndex = -1;
            ManufacturerFilterComboBox.SelectedIndex = -1;
            MinCostTextBox.Text = string.Empty;
            MaxCostTextBox.Text = string.Empty;
            MinCost = 0;
            MaxCost = null;
            SortMethod = "";
            Manufacturer = "";
            _isProgr = false;
            CreateProductsList(SearchedText, SortMethod, MinCost, MaxCost, Manufacturer);
        }

        private void Cost_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isProgr)
            {
                if (int.TryParse(MinCostTextBox.Text, out int minCost) || MinCostTextBox.Text == "")
                {
                    if (int.TryParse(MaxCostTextBox.Text, out int maxCost) || MaxCostTextBox.Text == "")
                    {
                        MinCost = MinCostTextBox.Text == "" ? 0 : minCost;
                        MaxCost = MaxCostTextBox.Text == "" ? null : maxCost;
                        CreateProductsList(SearchedText, SortMethod, MinCost, MaxCost, Manufacturer);
                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace task
{
    /// <summary>
    /// Логика взаимодействия для PartnerEditWindow.xaml
    /// </summary>
    public partial class PartnerEditWindow : Window
    {
        private PracticeContext _dbContext;
        private Productsale _sale;  
        private bool _isNewSale;

        public PartnerEditWindow(PracticeContext dbContext, Productsale sale = null)
        {
            InitializeComponent();
            _dbContext = dbContext;
            LoadAgentsAndProducts();  


            if (sale == null)
            {
                _isNewSale = true;
                _sale = new Productsale(); 
            }
            else
            {
                _isNewSale = false;
                _sale = sale;
                LoadSaleData();
            }
        }

        private void LoadAgentsAndProducts()
        {
            AgentComboBox.ItemsSource = _dbContext.Agents.ToList();
            AgentComboBox.DisplayMemberPath = "Title";

            ProductComboBox.ItemsSource = _dbContext.Products.ToList();
            ProductComboBox.DisplayMemberPath = "Title";
        }
        private void LoadSaleData()
        {
            var agent = _dbContext.Agents.Find(_sale.AgentId);
            var product = _dbContext.Products.Find(_sale.ProductId);

            AgentComboBox.SelectedItem = _dbContext.Agents.FirstOrDefault(a => a.Id == _sale.AgentId);
            ProductComboBox.SelectedItem = _dbContext.Products.FirstOrDefault(p => p.Id == _sale.ProductId);

            QuantityTextBox.Text = _sale.ProductCount.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (AgentComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите агента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ProductComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите продукт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(QuantityTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите количество.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                MessageBox.Show("Пожалуйста, введите целое число в поле 'Количество'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (quantity <= 0)
            {
                MessageBox.Show("Пожалуйста, введите положительное число в поле 'Количество'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _sale.AgentId = ((Agent)AgentComboBox.SelectedItem).Id;  
            _sale.ProductId = ((Product)ProductComboBox.SelectedItem).Id; 
            _sale.ProductCount = quantity;


            try
            {
                if (_isNewSale)
                {
                    _dbContext.Productsales.Add(_sale);
                }
                else
                {
                    _dbContext.Productsales.Update(_sale);
                }

                _dbContext.SaveChanges();
                DialogResult = true; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
            Close();
        }
    }
}
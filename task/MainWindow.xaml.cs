using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace task
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Agent> _agents = new List<Agent>(); // Теперь работаем с сущностями Agent

        public MainWindow()
        {
            InitializeComponent();

            // Загрузка данных из базы данных (замените на ваш код)
            LoadAgentsFromDatabase();

            // Отображение агентов в UI
            DisplayAgents();
        }

        private void LoadAgentsFromDatabase()
        {
            using (var db = new PracticeContext())
            {
                // Загружаем агентов и связанные данные (ProductsSold)
                _agents = db.Agents.Include(a => a.AgentType).ToList();  // Жадная загрузка для AgentType
            }
        }

        private decimal CalculateDiscount(Agent agent)
        {
            using (var db = new PracticeContext())
            {
                // Рассчитываем общее количество проданных продуктов для агента
                int totalProductsSold = db.Productsales
                    .Where(ps => ps.AgentId == agent.Id)
                    .Sum(ps => ps.ProductCount);

                if (totalProductsSold < 10000)
                {
                    return 0;
                }
                else if (totalProductsSold >= 10000 && totalProductsSold < 50000)
                {
                    return 5;
                }
                else if (totalProductsSold >= 50000 && totalProductsSold < 300000)
                {
                    return 10;
                }
                else
                {
                    return 15;
                }
            }
        }

        private void DisplayAgents()
        {
            foreach (var agent in _agents)
            {
                // Рассчитываем скидку для каждого агента
                decimal discount = CalculateDiscount(agent);

                // Создаем Border для каждого агента
                Border border = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(5),
                    Padding = new Thickness(10)
                };

                // Создаем Grid для колоночного размещения данных и скидки
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) }); // Для скидки

                // Создаем StackPanel для текстовых полей
                StackPanel stackPanel = new StackPanel();

                // Добавляем текстовые поля
                TextBlock typeNameBlock = new TextBlock { Text = $"{agent.AgentType?.Title ?? "Не указано"} | {agent.Title}", FontWeight = FontWeights.Bold }; // Используем AgentType
                TextBlock directorBlock = new TextBlock { Text = $"Директор: {agent.DirectorName}" };
                TextBlock phoneBlock = new TextBlock { Text = $"Телефон: {agent.Phone}" };
                TextBlock priorityBlock = new TextBlock { Text = $"Приоритет: {agent.Priority}" }; // Вместо Rating

                stackPanel.Children.Add(typeNameBlock);
                stackPanel.Children.Add(directorBlock);
                stackPanel.Children.Add(phoneBlock);
                stackPanel.Children.Add(priorityBlock);

                // Создаем TextBlock для скидки
                TextBlock discountBlock = new TextBlock
                {
                    Text = $"{discount}%",
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(5),
                    FontSize = 16
                };
                Grid.SetColumn(discountBlock, 1);

                // Добавляем StackPanel и TextBlock в Grid
                grid.Children.Add(stackPanel);
                grid.Children.Add(discountBlock);

                // Добавляем Grid в Border
                border.Child = grid;

                // Добавляем Border в StackPanel (главный контейнер)
                PartnersPanel.Children.Add(border);
            }
        }
    }
}
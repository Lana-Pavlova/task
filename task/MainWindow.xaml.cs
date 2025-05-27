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
        private List<Agent> _agents = new List<Agent>();

        public MainWindow()
        {
            InitializeComponent();
            LoadAgentsFromDatabase();
            DisplayAgents();
        }

        private void LoadAgentsFromDatabase()
        {
            using (var db = new PracticeContext())
            {
                _agents = db.Agents.Include(a => a.AgentType).ToList();
            }
        }

        private decimal CalculateOrderCost(Agent agent)
        {
            using (var db = new PracticeContext())
            {
                decimal totalCost = db.Productsales
                    .Where(ps => ps.AgentId == agent.Id)
                    .Join(db.Products,
                          ps => ps.ProductId,
                          p => p.Id,
                          (ps, p) => new { ps, p })
                    .Sum(x => (decimal)(x.ps.ProductCount * x.p.MinCostForAgent));

                return totalCost;
            }
        }

        private void DisplayAgents()
        {
            using (var db = new PracticeContext())
            {
                _agents = db.Agents.Include(a => a.AgentType).ToList(); // Обновляем список агентов из базы данных

                var agentData = _agents.Select(agent => new
                {
                    TypeName = $"{agent.AgentType?.Title ?? "Не указано"} | {agent.Title}",
                    Director = $"Директор: {agent.DirectorName}",
                    Phone = $"Телефон: {agent.Phone}",
                    Priority = $"Приоритет: {agent.Priority}",
                    OrderCost = $"{CalculateOrderCost(agent):C}" // Format as currency
                }).ToList();

                AgentsListBox.ItemsSource = agentData;
            }
        }


        private void AddPartnerButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new PracticeContext())
            {
                PartnerEditWindow editWindow = new PartnerEditWindow(db);
                bool? result = editWindow.ShowDialog();
                if (result == true)
                {
                    DisplayAgents(); // Обновляем список партнеров после добавления
                }
            }
        }

        private void AgentsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Получаем выбранный элемент из ListBox
            var selectedItem = AgentsListBox.SelectedItem;

            if (selectedItem != null)
            {
                // Получаем объект Agent, соответствующий выбранному элементу
                var agentData = (dynamic)selectedItem;
                var agent = _agents.FirstOrDefault(a => $"{a.AgentType?.Title ?? "Не указано"} | {a.Title}" == agentData.TypeName);

                if (agent != null)
                {
                    using (var db = new PracticeContext())
                    {
                        PartnerEditWindow editWindow = new PartnerEditWindow(db, agent);
                        bool? result = editWindow.ShowDialog();
                        if (result == true)
                        {
                            DisplayAgents(); // Обновляем список партнеров после редактирования
                        }
                    }
                }
            }
        }
    }
}
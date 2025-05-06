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

        private decimal CalculateDiscount(Agent agent)
        {
            using (var db = new PracticeContext())
            {
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
            var agentData = _agents.Select(agent => new
            {
                TypeName = $"{agent.AgentType?.Title ?? "Не указано"} | {agent.Title}",
                Director = $"Директор: {agent.DirectorName}",
                Phone = $"Телефон: {agent.Phone}",
                Priority = $"Приоритет: {agent.Priority}",
                Discount = $"{CalculateDiscount(agent)}%"
            }).ToList();

            AgentsItemsControl.ItemsSource = agentData;
        }
    }
}
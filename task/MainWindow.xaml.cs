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
            LoadAgentsFromDatabase(); 

            using (var db = new PracticeContext())
            {

                var agentData = _agents.Select(agent => new
                {
                    TypeName = $"{agent.AgentType?.Title ?? "Не указано"} | {agent.Title}",
                    Phone = $"Телефон: {agent.Phone}",
                    Priority = $"Приоритет: {agent.Priority}",
                    OrderCost = $"{CalculateOrderCost(agent):C}" 
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
                    DisplayAgents(); 
                }
            }
        }

        private void AgentsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = AgentsListBox.SelectedItem;

            if (selectedItem != null)
            {
                var agentData = (dynamic)selectedItem;
                var agent = _agents.FirstOrDefault(a => $"{a.AgentType?.Title ?? "Не указано"} | {a.Title}" == agentData.TypeName);

                if (agent != null)
                {
                    using (var db = new PracticeContext())
                    {
                        var sales = db.Productsales.Where(s => s.AgentId == agent.Id).ToList();

                        if (sales.Count > 0)
                        {

                            PartnerEditWindow partnerEditWindow = new PartnerEditWindow(db, sales.First());
                            bool? result = partnerEditWindow.ShowDialog();
                            if (result == true)
                            {
                                DisplayAgents(); 
                            }
                        }
                        else
                        {
                            MessageBox.Show("У данного агента нет заявок.");
                        }
                    }
                }
            }
        }
    }
} 
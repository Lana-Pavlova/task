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
        private Agent _agent;
        private bool _isNewAgent;

        public PartnerEditWindow(PracticeContext dbContext, Agent agent = null)
        {
            InitializeComponent();
            _dbContext = dbContext;

            // Загрузка типов агентов в ComboBox
            AgentTypeComboBox.ItemsSource = _dbContext.Agenttypes.ToList();
            AgentTypeComboBox.DisplayMemberPath = "Title";

            if (agent == null)
            {
                // Добавление нового партнера
                _isNewAgent = true;
                _agent = new Agent();
            }
            else
            {
                // Редактирование существующего партнера
                _isNewAgent = false;
                _agent = agent;
                LoadAgentData();
            }
        }

        private void LoadAgentData()
        {
            TitleTextBox.Text = _agent.Title;
            AgentTypeComboBox.SelectedItem = _agent.AgentType;
            PriorityTextBox.Text = _agent.Priority.ToString();
            AddressTextBox.Text = _agent.Address;
            DirectorNameTextBox.Text = _agent.DirectorName;
            PhoneTextBox.Text = _agent.Phone;
            EmailTextBox.Text = _agent.Email;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация ввода
            if (string.IsNullOrEmpty(TitleTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите наименование партнера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (AgentTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите тип партнера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(PriorityTextBox.Text, out int priority) || priority < 0)
            {
                MessageBox.Show("Пожалуйста, введите корректный неотрицательный рейтинг.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Заполнение данных агента из формы
            _agent.Title = TitleTextBox.Text;
            _agent.AgentType = (Agenttype)AgentTypeComboBox.SelectedItem;
            _agent.AgentTypeId = ((Agenttype)AgentTypeComboBox.SelectedItem).Id;
            _agent.Priority = priority;
            _agent.Address = AddressTextBox.Text;
            _agent.DirectorName = DirectorNameTextBox.Text;
            _agent.Phone = PhoneTextBox.Text;
            _agent.Email = EmailTextBox.Text;

            try
            {
                if (_isNewAgent)
                {
                    // Добавление нового агента в базу данных
                    _dbContext.Agents.Add(_agent);
                }
                else
                {
                    // Обновление существующего агента
                    _dbContext.Agents.Update(_agent);
                }

                _dbContext.SaveChanges();
                DialogResult = true; // Закрываем окно с результатом OK
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Закрываем окно с результатом Cancel
            Close();
        }
    }
}
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

namespace DairyPlant
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private dairyplantEntities db = new dairyplantEntities();
        public MainWindow(string role)
        {
            InitializeComponent();
            UpdateTable();
            // Скрытие админ-панели для обычного пользователя
            if (role != "admin")
            {
                BtnCreate.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                dgUsers.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            EditWindow editWindow = new EditWindow(null);
            editWindow.ShowDialog();
            UpdateTable();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if(dgUsers.SelectedItem != null)
            {
                Users selectedUser = dgUsers.SelectedItem as Users;
                EditWindow editWindow = new EditWindow(selectedUser);
                editWindow.ShowDialog();
                UpdateTable();
            }
            else
            {
                MessageBox.Show("Выберите пользователя");

            }
        }

        /// <summary>
        /// Обновляет подключение к базе данных и загружает список пользователей в таблицу.
        /// </summary>
        private void UpdateTable()
        {
            db = new dairyplantEntities();
            dgUsers.ItemsSource = db.Users.ToList();
        }

        /// <summary>
        /// Сохраняет изменения в базе данных после редактирования ячейки таблицы.
        /// </summary>
        private void DgUsers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Сохранение изменённого флажка
            if (e.EditAction == DataGridEditAction.Commit)
            {
                db.SaveChanges();
            }
        }

        private void BtnValidation_Click(object sender, RoutedEventArgs e)
        {
            ValidationWindow validationWindow = new ValidationWindow();
            validationWindow.ShowDialog();
        }
    }
}

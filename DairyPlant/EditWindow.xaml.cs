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
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private Users currentUser;
        private dairyplantEntities db = new dairyplantEntities();
        public EditWindow(Users currentUser)
        {
            this.currentUser = currentUser;
            InitializeComponent();
            if(currentUser == null)
            {
                this.Title = "Добавление пользователя";
            }
            else
            {
                NameBox.Text = currentUser.name;
                LoginBox.Text = currentUser.login;
                PasswordBox.Password = currentUser.password;
                if(currentUser.role == "admin")
                    RoleBox.SelectedIndex = 0;
                else
                    RoleBox.SelectedIndex = 1;
                BlockBox.IsChecked = currentUser.is_blocked;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(LoginBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                RoleBox.SelectedIndex == -1)
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            if (NameBox.Text.Length > 150 || LoginBox.Text.Length > 50 || PasswordBox.Password.Length > 50)
            {
                MessageBox.Show("Превышена максимальная длина поля (50 символов).");
                return;
            }

            if (NameBox.Text.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c) && c != '-'))
            {
                MessageBox.Show("Имя может содержать только буквы, пробелы и дефис.");
                return;
            }

            if (LoginBox.Text.Contains(" ") || PasswordBox.Password.Contains(" "))
            {
                MessageBox.Show("В логине и пароле не должно быть пробелов.");
                return;
            }

            if (currentUser == null)
            {
                var user = db.Users.FirstOrDefault(u => u.login == LoginBox.Text);
                if (user != null) 
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.");
                    return;
                }
                else
                {
                    Users newuser = new Users();
                    EditUser(newuser);
                    db.Users.Add(newuser);
                }
            }
            else
            {
                var user = db.Users.FirstOrDefault(u => u.id_user == currentUser.id_user);
                EditUser(user);
            }
            db.SaveChanges();
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Переносит данные из полей формы в объект пользователя.
        /// </summary>
        private void EditUser(Users user)
        {
            user.name = NameBox.Text;
            user.login = LoginBox.Text;
            user.password = PasswordBox.Password;
            if (RoleBox.SelectedIndex == 0)
                user.role = "admin";
            else
                user.role = "user";
            user.is_blocked = BlockBox.IsChecked == true; 
        }
    }
}

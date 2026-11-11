using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DairyPlant
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
            string[] paths = {
                Puzzle1.Source.ToString(),
                Puzzle2.Source.ToString(),
                Puzzle3.Source.ToString(),
                Puzzle4.Source.ToString()
            };
            Random rnd = new Random();
            paths = paths.OrderBy(x => rnd.Next()).ToArray();
            Puzzle1.Source = new BitmapImage(new Uri(paths[0]));
            Puzzle2.Source = new BitmapImage(new Uri(paths[1]));
            Puzzle3.Source = new BitmapImage(new Uri(paths[2]));
            Puzzle4.Source = new BitmapImage(new Uri(paths[3]));
        }

        private dairyplantEntities db = new dairyplantEntities();
        private Image draggedImage;
        private int failedAttempts = 0;
        private string lastlog = "";
        private bool capcha = false;

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = db.Users.FirstOrDefault(u => u.login == LoginBox.Text);
            if ((user != null && user.is_blocked == true) || failedAttempts >= 3)
            {
                MessageBox.Show("Вы заблокированы. Обратитесь к администратору.");
                return;
            }
            else if (user != null)
            {
                // Сброс попыток при смене логина
                if (lastlog != LoginBox.Text)
                {
                    lastlog = user.login;
                    failedAttempts = 0;
                }
                if (capcha)
                {
                    if (user.password == PasswordBox.Password)
                    {
                        MessageBox.Show("Вы успешно авторизовались");
                        failedAttempts = 0;
                        MainWindow main = new MainWindow(user.role);
                        main.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные.");
                        failedAttempts++;
                        if (failedAttempts == 3)
                        {
                            user.is_blocked = true;
                            db.SaveChanges();
                            MessageBox.Show("Вы заблокированы. Обратитесь к администратору.");
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Вы не прошли капчу!");
                    failedAttempts++;
                    if (failedAttempts == 3)
                    {
                        user.is_blocked = true;
                        db.SaveChanges();
                        MessageBox.Show("Вы заблокированы. Обратитесь к администратору.");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные.");
            }
        }

        /// <summary>
        /// Обработчик нажатия мыши на фрагменте капчи. Начинает перетаскивание.
        /// </summary>
        private void Puzzle_MouseLeftBtnDown(object sender, MouseButtonEventArgs e)
        {
            // Начало перетаскивания фрагмента пазла
            draggedImage = sender as Image;
            DragDrop.DoDragDrop(draggedImage, draggedImage.Source, DragDropEffects.Move);
        }

        /// <summary>
        /// Обработчик отпускания фрагмента. Меняет местами и проверяет сборку пазла.
        /// </summary>
        private void Puzzle_Drop(object sender, DragEventArgs e)
        {
            Image targetImage = sender as Image;
            // Обмен фрагментов пазла местами
            if (draggedImage != null && targetImage != null && draggedImage != targetImage)
            {
                var tempSource = draggedImage.Source;
                draggedImage.Source = targetImage.Source;
                targetImage.Source = tempSource;
            }

            // Проверка правильности сборки пазла
            if (Puzzle1.Source.ToString().Contains("img1.png") &&
                Puzzle2.Source.ToString().Contains("img2.png") &&
                Puzzle3.Source.ToString().Contains("img3.png") &&
                Puzzle4.Source.ToString().Contains("img4.png"))
            {
                MessageBox.Show("Вы прошли капчу!");
                capcha = true;
            }
        }
    }
}

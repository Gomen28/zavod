using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Логика взаимодействия для ValidationWindow.xaml
    /// </summary>
    public partial class ValidationWindow : Window
    {
        public ValidationWindow()
        {
            InitializeComponent();
        }

        private async void BtnGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создаем HTTP-клиент для отправки запроса к эмулятору
                using (HttpClient client = new HttpClient())
                {
                    string url = "http://localhost:4444/TransferSimulator/fullName";
                    string response = await client.GetStringAsync(url);
                    // Разбираем ответ сервера в формате JSON
                    JObject json = JObject.Parse(response);
                    // Извлекаем значение поля "value"
                    NameDisplay.Text = json["value"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных: " + ex.Message);
            }
        }

        private void BtnSendResult_Click(object sender, RoutedEventArgs e)
        {
            string name = NameDisplay.Text;
            // Проверяем: содержит ли строка хотя бы один символ, который не является буквой, пробелом или дефисом
            bool hasDigits = name.Any(char.IsDigit);
            bool hasForbidden = name.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c) && c != '-');

            if (hasForbidden)
            {
                ResultDisplay.Text = "ФИО содержит запрещенные символы";
            }
            else
            {
                ResultDisplay.Text = "ФИО корректно";
            }
        }
    }
}

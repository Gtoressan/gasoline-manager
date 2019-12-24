using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Gasoline
{
    class Exam
    {

        /// <summary>
        /// Базовый экзамен для числовых строк, является ли строка числом
        /// </summary>
        /// <param name="content">числовая строка</param>
        /// <returns></returns>
        public static bool NumericValidation(string content)
        {
            if (!float.TryParse(content, out float f) || content == null || content == "")
                return false;

            else if (content.Length > 1 && content.Last() == ',')
                return false;

            return true;
        }

        /// <summary>
        /// Базовый экзамен для строк, на проверка пустоты (-1) и содержание запрещенных символов (0)
        /// </summary>
        /// <param name="content">строка</param>
        /// <param name="prohibitedChar">запрещенный символ</param>
        /// <param name="vanishText">псевдо-анимационный текст</param>
        /// <returns></returns>
        public static int WordValidation(string content, char prohibitedChar, string vanishText)
        {
            if (content == null || content == "" || content == vanishText)
                return -1;

            if (content.Contains(prohibitedChar))
                return 0;

            return 1;
        }



        /// <summary>
        /// Меняет фон TextBox'а при неправильном вводе числа
        /// </summary>
        /// <param name="textBox">текстовое поле, в котором нужно поменять цвет</param>
        public static void LigthTheBackgroundOfNumericTextBox(TextBox textBox)
        {
            string text = textBox.Text;

            if (!float.TryParse(text.Replace(" ", string.Empty), out float f) && text != null && text != "")
                textBox.Background = Brushes.LightYellow;
            else
                textBox.Background = Brushes.White;
        }

        /// <summary>
        /// Меняет фон TextBox'a при вводе запрещенного символа
        /// </summary>
        /// <param name="textBox">текстовое поле, в котором нужно поменять цвет</param>
        /// <param name="symbol">запрещенный символ, при вводе которого должен поменяться цвет</param>
        public static void LigthTheBackgroundOfNamedTextBox(TextBox textBox, char symbol)
        {
            string text = textBox.Text;

            if (WordValidation(text, symbol, "null") == 0)
                textBox.Background = Brushes.LightYellow;
            else
                textBox.Background = Brushes.White;
        }



        /// <summary>
        /// Проверка поля наименования
        /// </summary>
        /// <param name="text">основной текст</param>
        /// <param name="option">наименование параметра, отображаемого в сообщении</param>
        /// <param name="vanishText">исчезающий текст (псевдо-анимация)</param>
        /// <returns></returns>
        public static bool NamedExam(string text, string option, string vanishText)
        {
            if (WordValidation(text, '|', vanishText) == -1)
            {
                MessageBox.Show($"Вы не указали {option}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            else if (WordValidation(text, '|', vanishText) == 0)
            {
                MessageBox.Show("Знак \"|\" является недопустимым, попробуйте его заменить другим", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            else if (option == "имя учёта" && WriteAndRead.GetListFda(0).Contains(text))
            {
                MessageBox.Show($"Такое имя {option} занято, придумайте другое", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
 
        /// <summary>
        /// Проверка числового поля
        /// </summary>
        /// <param name="text">основной текст</param>
        /// <param name="option">наименование параметра, отображаемого в сообщении</param>
        /// <param name="vanishText">исчезающий текст (псевдо-анимация)</param>
        /// <returns></returns>
        public static bool NumericExam(string text, string option, string vanishText)
        {
            if (!NumericValidation(text))
            {
                MessageBox.Show($"Вы не указали {option}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            else if (!NumericValidation(text))
            {
                MessageBox.Show($"Вы не верно указали {option}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}

/// имя_учета(0), дата_создания(1), средний_расход_топлива(2)
/// дата(0), АЗС(1), Тип(2), Объем(3), Стоимость(4), Средний_расход(5), Стоимость_литра(6), Ожидаемая_длина_поездки(7)
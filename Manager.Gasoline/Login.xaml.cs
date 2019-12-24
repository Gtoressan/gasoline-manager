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

namespace Manager.Gasoline
{ 
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();

            //перемещение окна за Header или зоголовок (Rectangle на окне)
            void layoutRoot(object sender, MouseButtonEventArgs e) => DragMove();
            Header.MouseLeftButtonDown += new MouseButtonEventHandler(layoutRoot);
            HeadLable.MouseLeftButtonDown += new MouseButtonEventHandler(layoutRoot);

            //Создание необходимых файлов
            WriteAndRead.CreateFileAndFirectory();

            //заполнение списка учетов
            Actions.AccountsFill(lbAccounts);
        }

        #region Сворачивание/разворачивание окна

        //закрытие окна
        private void bExit_Click(object sender, RoutedEventArgs e) => Close();
        //сворачивание окна
        private void bMinimized_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        //перетаскивание окна в полноэкранном режиме
        private void Header_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Normal;
        //создание нового учета 
        private void bCreateNewAcc_Click(object sender, RoutedEventArgs e) => Actions.AppendNewAcc(tbName, lbAccounts);

        //об этой программе
        private void bAbout_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Учёт расхода топлива\nРазработчик: hse.perm\nВерсия: 1.0\nСайт: а сайта нет", "Об этой программе", MessageBoxButton.OK, MessageBoxImage.Information);

        #endregion

        #region Псевдо-анимация

        //псевдо-анимация для текстового поля
        private void tbName_GotFocus(object sender, RoutedEventArgs e) => Actions.VanishText(tbName, "Укажите имя здесь", false);
        private void tbName_LostFocus(object sender, RoutedEventArgs e) => Actions.VanishText(tbName, "Укажите имя здесь", true);
        private void tbName_KeyUp(object sender, KeyEventArgs e) => Exam.LigthTheBackgroundOfNamedTextBox(tbName, '|');

        #endregion

        //активация окна Login
        private void Window_Activated(object sender, EventArgs e)
        {
            string check = lbAccounts.Items[0].ToString();
            string defolt1 = "Вы еще не завели ни одного учета";

            if (check != defolt1)
            {
                //заполнение ListBox'a Accounts
                Actions.AccountsFill(lbAccounts);
            }
        }

        //выбор элемента из списка учетов
        private void tbAccounts_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => Actions.ChangeItem(lbAccounts, this);
    }
}

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
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();

            //перемещение окна за Header или зоголовок (Rectangle на окне)
            void layoutRoot(object sender, MouseButtonEventArgs e) => DragMove();
            Header.MouseLeftButtonDown += new MouseButtonEventHandler(layoutRoot);
            lHead.MouseLeftButtonDown += new MouseButtonEventHandler(layoutRoot);
        }

        #region Сворачивание/разворачивание окна

        //закрытие окна
        private void bExit_Click(object sender, RoutedEventArgs e) => Close();

        //сворачивание окна
        private void bMinimized_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        //перетаскивание окна в полноэкрамном режиме
        private void Header_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Normal;
        //разворачивание окна
        private void bMaximized_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                bMaximized.ToolTip = "Cвернуть в окно";
            }

            else if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                bMaximized.ToolTip = "Развернуть";
            }
        }

        #endregion

        #region Псевдо-анимация

        //псевдо-анимация для объема топлива
        private void tbVolume_GotFocus(object sender, RoutedEventArgs e) => Actions.VanishText(tbVolume, tbVolume.DataContext.ToString(), false);
        private void tbVolume_LostFocus(object sender, RoutedEventArgs e) => Actions.VanishText(tbVolume, tbVolume.DataContext.ToString(), true);
        private void tbVolume_KeyUp(object sender, KeyEventArgs e) => Exam.LigthTheBackgroundOfNumericTextBox(tbVolume);

        //псевдо-анимация для затрат на топливо
        private void tbCost_GotFocus(object sender, RoutedEventArgs e) => Actions.VanishText(tbCost, tbCost.DataContext.ToString(), false);
        private void tbCost_LostFocus(object sender, RoutedEventArgs e) => Actions.VanishText(tbCost, tbCost.DataContext.ToString(), true);
        private void tbCost_KeyUp(object sender, KeyEventArgs e) => Exam.LigthTheBackgroundOfNumericTextBox(tbCost);

        //псевдо-анимация для среднего расхода топлива
        private void tbConsumption_GotFocus(object sender, RoutedEventArgs e) => Actions.VanishText(tbConsumption, tbConsumption.DataContext.ToString(), false);
        private void tbConsumption_LostFocus(object sender, RoutedEventArgs e) => Actions.VanishText(tbConsumption, tbConsumption.DataContext.ToString(), true);
        private void tbConsumption_KeyUp(object sender, KeyEventArgs e) => Exam.LigthTheBackgroundOfNumericTextBox(tbConsumption);

        //псевдо анимация для наименования АЗС
        private void tbGS_GotFocus(object sender, RoutedEventArgs e) => Actions.VanishText(tbGS, tbGS.DataContext.ToString(), false);
        private void tbGS_LostFocus(object sender, RoutedEventArgs e) => Actions.VanishText(tbGS, tbGS.DataContext.ToString(), true);
        private void tbGS_KeyUp(object sender, KeyEventArgs e) => Exam.LigthTheBackgroundOfNamedTextBox(tbGS, '|');

        //псевдо-анимация для поля переименования учета
        private void tbRename_GotFocus(object sender, RoutedEventArgs e) => Actions.VanishText(tbRename, tbRename.DataContext.ToString(), false);
        private void tbRename_LostFocus(object sender, RoutedEventArgs e) => Actions.VanishText(tbRename, tbRename.DataContext.ToString(), true);
        private void tbRename_KeyUp(object sender, KeyEventArgs e) => Exam.LigthTheBackgroundOfNamedTextBox(tbRename, '|');

        //изменение ширины окна при миграции между вкладками "Главная", "Таблица" и "Аналитика"
        private void tiTable_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => Actions.ResizeWindowWidth(this, 28, true);
        private void tiMainTab_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => Actions.ResizeWindowWidth(this, 28, false);
        private void tiStatistics_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => Actions.ResizeWindowWidth(this, 28, false);

        #endregion

        //запоминание среднего расхода топлива при двойном клике                              
        private void tbConsumption_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Actions.SavedConsumption(Title, tbConsumption);
            Actions.DataUpdate(this);
            UnfocusElement.Focus();
        }

        //занесение данных учета в .dll файл
        private void bAppendData_Click(object sender, RoutedEventArgs e) => Actions.AppendData(this);

        //переименование учета
        private void bRename_Click(object sender, RoutedEventArgs e) => Actions.Renaming(tbRename.Text, this);

        //удадение данных учета
        private void bDataClear_Click(object sender, RoutedEventArgs e) => Actions.DataClearing(Title, this);

        //удаление учета
        private void bDelete_Click(object sender, RoutedEventArgs e) => Actions.Deleting(this);

        //обновление данных в разделе "Аналитика"
        private void bUpdate_Click(object sender, RoutedEventArgs e)
        {
            Calculate.InfoUpdate(this);
            MessageBox.Show("Данные успешно обновлены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //изменение представления данных в chart1
        private void bChart1Reversed_Click(object sender, RoutedEventArgs e) => Actions.DataUpdate(this);

        //изменение представления данных в chat2
        private void bChart2Reversed_Click(object sender, RoutedEventArgs e) => Actions.DataUpdate(this);

        //правка сохраненных данных
        private void ToCorrect_Click(object sender, RoutedEventArgs e) => Actions.OpenCustomizeWindow(Title, this, sender);
    }
}

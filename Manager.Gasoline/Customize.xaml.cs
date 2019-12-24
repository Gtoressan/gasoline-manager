using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Manager.Gasoline
{
    /// <summary>
    /// Логика взаимодействия для Customize.xaml
    /// </summary>
    public partial class Customize : Window
    {
        public Customize()
        {
            InitializeComponent();

            //перемещение окна за Header или зоголовок (Rectangle на окне)
            void layoutRoot(object sender, MouseButtonEventArgs e) => DragMove();
            Header.MouseLeftButtonDown += new MouseButtonEventHandler(layoutRoot);
            lHead.MouseLeftButtonDown += new MouseButtonEventHandler(layoutRoot);
        }

        //закрытие окна
        private void bExit_Click(object sender, RoutedEventArgs e) => Close();

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

        #endregion

        //запоминание среднего расхода топлива при двойном клике                              
        private void tbConsumption_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Actions.SavedConsumption(Title, tbConsumption);
            Actions.DataUpdate((Main)Owner);
            UnfocusElement.Focus();
        }

        //сохранение измененных данных
        private void bSaveData_Click(object sender, RoutedEventArgs e) => Actions.ToCorrectOfData(Title, (Main)Owner, this, (int)bSaveData.DataContext);

        private void bCancel_Click(object sender, RoutedEventArgs e) => Actions.ToDeliteObject(Title, (Main)Owner, this, (int)bSaveData.DataContext);
    }
}

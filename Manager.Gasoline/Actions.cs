using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Manager.Gasoline
{
    class Actions
    {

        /// <summary>
        /// Обновление данных в окне
        /// </summary>
        /// <param name="main">окно</param>
        public static void DataUpdate(Main main)
        {
            //заголовок окна
            main.lHead.Content = main.Title;

            //значение среднего расхода топлива
            string consumption = WriteAndRead.GetListFda(2)[WriteAndRead.GetListFda(0).IndexOf(main.Title)];

            if (consumption != "Не указано")
            {
                main.tbConsumption.Text = consumption;
                main.tbConsumption.Background = Brushes.LightGreen;
                main.tbConsumption.Foreground = Brushes.Black;
                main.tbConsumption.ToolTip = $"Кликните 2 раза, чтобы запомнить.\nСохранённое значение: {consumption}";
            }
            else
                main.tbConsumption.ToolTip = "Кликните 2 раза, чтобы запомнить";

            //заметка о дате заведения учета
            main.lAboutAccCreating.Content = "Учёт был заведён " + WriteAndRead.GetListFda(1)[WriteAndRead.GetListFda(0).IndexOf(main.Title)];

            //перезаполняем таблицу
            main.dgTable.ItemsSource = WriteAndRead.ReadForTable(main.Title);

            //обновляем первую диаграмму
            Chart1Update();

            //обновляем вторую диаграмму
            Chart2Update();

            //перезаполняем круговую диаграмму
            main.chPieChart.ItemsSource = WriteAndRead.ReadForPieChart(main.Title);

            //переписываем списки дней в разделе "Аналитика"
            FillDates(main.Title, main.cbDate, main.cbDate2);

            void Chart1Update()
            {
                //перезаполняем первую диаграмму
                if (main.bChart1Reversed.ToolTip.ToString() == "Изменить по дням")
                {
                    main.chart1.ItemsSource = WriteAndRead.ReadForFirstChart(main.Title, 1);
                    main.bChart1Reversed.ToolTip = "Изменить по записям";
                }
                else
                {
                    main.chart1.ItemsSource = WriteAndRead.ReadForFirstChart(main.Title, 0);
                    main.bChart1Reversed.ToolTip = "Изменить по дням";
                }
            }
            void Chart2Update()
            {
                //перезаполняем вторую диаграмму
                if (main.bChart2Reversed.ToolTip.ToString() == "Изменить по дням")
                {
                    main.chart2.ItemsSource = WriteAndRead.ReadForSecondChart(main.Title, 1);
                    main.bChart2Reversed.ToolTip = "Изменить по записям";
                }
                else
                {
                    main.chart2.ItemsSource = WriteAndRead.ReadForSecondChart(main.Title, 0);
                    main.bChart2Reversed.ToolTip = "Изменить по дням";
                }
            }
        }

        //заведение нового учета
        public static void AppendNewAcc(TextBox textBox, ListBox listBox)
        {
            string name = textBox.Text;

            if (Exam.NamedExam(name, "имя учета", Resources.Get(Resources.Names.textForName)))
            {
                //инициализация пустого массива
                string[] array = { };

                //создание, заполнение файла пустым массивом
                File.AppendAllLines($"{Resources.Get(Resources.Names.Dir)}\\{name}.dll", array);

                //добавляем учет в acountList.ini
                WriteAndRead.WriteInFda(name, 1);

                //заполняем listBox
                AccountsFill(listBox);

                //очищаем текстовой поле
                textBox.Text = null;

                MessageBox.Show($"Новый учет \"{name}\" успешно заведен", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //заполнение Accounts
        public static void AccountsFill(ListBox listBox)
        {
            List<string> list = WriteAndRead.GetListFda(0);

            //если список пустой
            if (list.Count == 0)
            {
                listBox.IsEnabled = false;
                listBox.ItemsSource = null;
                listBox.Items.Add(Resources.Get(Resources.Names.defoltItem));
            }

            //если список содержит наименования учетов
            else
            {
                //удаление стандартного элемента 
                if (listBox.Items.Contains(Resources.Get(Resources.Names.defoltItem)))
                    listBox.Items.Clear();

                listBox.IsEnabled = true;
                listBox.ItemsSource = list;
            }
        }

        //исчезающий текст для элемента TextBox
        public static void VanishText(TextBox textBox, string content, bool action)
        {
            //пишет текст
            if ((textBox.Text == "" || textBox.Text == null) && action)
            {
                textBox.Text = content;
                textBox.Foreground = Brushes.DarkGray;
            }

            //стирает текст
            else if ((textBox.Text == content) && !action)
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        //открытие Main окна для выбранного элемента
        public static void ChangeItem(ListBox Accounts, Login loginWindow)
        {
            if (Accounts.SelectedItem != null)
            {
                //инициализация нового окна Main
                Main mainWindow = new Main();

                //инициализация массива данных учета по выделенному элементу
                string[][] data = WriteAndRead.GetArrayDll(Accounts.SelectedValue.ToString());

                //открываем окно Main и называем его выделенным элементом
                mainWindow.Title = Accounts.SelectedItem.ToString();
                mainWindow.lHead.Content = Accounts.SelectedItem.ToString();
                mainWindow.Show();

                //сворачиваем окно Login и снимаем выделение с Accounts
                loginWindow.WindowState = WindowState.Minimized;
                Accounts.SelectedItem = null;

                //обновление данных в окне
                DataUpdate(mainWindow);

                //обновление дней в комбобоксах, раздел "Аналитика" 
                Calculate.InfoUpdate(mainWindow);
            }
        }

        //сохранение среднего расхода топлива
        public static void SavedConsumption(string name, TextBox textBox)
        {
            //инициализируем значение среднего расхода топлива
            string consumption = textBox.Text;

            //сообщение об ошибке
            if (!Exam.NumericValidation(consumption))
                MessageBox.Show("Вы попытались запомнить невозножное значение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

            //сохранение среднего расхода топлива
            else
            {
                //если пользователь указал 0 или меньшее число
                //сохроняем средний расход, как неуказанный
                float reset = float.Parse(consumption);

                if (reset <= 0)
                {
                    consumption = "Не указано";
                    textBox.Text = null;
                    VanishText(textBox, "Укажите здесь средний расход", true);
                }

                //если число больше 0
                else
                {
                    textBox.Text = consumption;
                    textBox.Background = Brushes.LightGreen;
                    textBox.Foreground = Brushes.Black;
                }

                //записываем новое значение
                WriteAndRead.ChangeFdaPoint(name, consumption, 2);

                //сообщения по окончанию добавления
                if (consumption != "Не указано")
                    MessageBox.Show($"Значение \"{consumption}\" успешно сохранено.\nВ следующий раз вам не придется вводить его заного", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show($"Сохраненное значение удалено", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //меняем состояние кнопки соответственно среднему расходу
        static void ChangesAgregateConsumptionButton(string name, TextBox textBox)
        {
            string consumption = WriteAndRead.GetListFda(2)[WriteAndRead.GetListFda(0).IndexOf(name)];

            if (consumption != "Не указано")
            {
                textBox.Background = Brushes.GreenYellow;
                textBox.Text = consumption;
                textBox.Foreground = Brushes.Black;
            }
        }

        //добавляем данные в .dll файл
        public static void AppendData(Main main)
        {
            //инициализация данных
            string GS = main.tbGS.Text;
            string type = main.cbType.Text;
            string volume = main.tbVolume.Text;
            string cost = main.tbCost.Text;
            string consumption = main.tbConsumption.Text;
            string costLitr = null;
            string transit = null;

            if (GS == "" || GS == null || GS == Resources.Get(Resources.Names.textGS)) GS = "Не указано";

            if (type == "Выберите тип используемого бензина")
                MessageBox.Show("Вы не выбрали тип топлива", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

            else if (!Exam.NumericExam(volume, "объем топлива", main.tbVolume.DataContext.ToString())) ;
            else if (!Exam.NumericExam(cost, "стоимость заправки", main.tbCost.DataContext.ToString())) ;
            else if (!Exam.NumericExam(consumption, "средний расход топлива", main.tbConsumption.DataContext.ToString())) ;
            else if (!Exam.NamedExam(GS, "наименование АЗС", main.tbGS.DataContext.ToString())) ;

            else
            {
                costLitr = Calculate.GetCostLitr(volume, cost);
                transit = Calculate.GetTransit(consumption, volume);

                //инициализируем массив данных
                string[] array = { GS.Trim(' '), type, Formate(volume), Formate(cost, 2), Formate(consumption), Formate(costLitr), Formate(transit) };

                //запись данных
                WriteAndRead.WriteDll(main.Title, array);

                DataUpdate(main);
                ResetTextBoxes(main);

                MessageBox.Show("Данные успешно занесены в учет", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //меняем имя учета 
        public static void Renaming(string name, Main main)
        {
            if (Exam.NamedExam(name, "имя учёта", Resources.Get(Resources.Names.textForName)))
            {
                //поменяли имя .dll файла
                File.Move($"{Resources.Get(Resources.Names.Dir)}\\{main.Title}.dll", $"{Resources.Get(Resources.Names.Dir)}\\{name}.dll");

                //поменяли имя в acountList.ini
                WriteAndRead.ChangeFdaPoint(main.Title, name, 0);

                //меняем заголовок окна Main
                main.Title = name;

                DataUpdate(main);

                MessageBox.Show($"Имя учета было изменено на \"{name}\"", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //стираем данные учета
        public static void DataClearing(string name, Main main)
        {
            MessageBoxResult result = MessageBox.Show("Данные учета будут безвозвратно уничтожены, продолжить?", "Сообщение", 
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

            if (result == MessageBoxResult.Yes)
            {
                File.Delete($"{Resources.Get(Resources.Names.Dir)}\\{name}.dll");
                File.Create($"{Resources.Get(Resources.Names.Dir)}\\{name}.dll").Close();

                DataUpdate(main);

                MessageBox.Show("Данные были безвозвратно уничтожены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //удаляем учет
        public static void Deleting(Main main)
        {
            MessageBoxResult result = MessageBox.Show($"Учет \"{main.Title}\" будет безвозвратно удалён, продолжить?", "Сообщение", 
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

            if (result == MessageBoxResult.Yes)
            {
                //инициализируем индекс позиции имени учета
                int index = WriteAndRead.GetListFda(0).IndexOf(main.Title);

                //инициализируем список из строк accountList.ini
                List<string> list = File.ReadAllLines(Resources.Get(Resources.Names.Ini)).ToList();

                //удаляем строчку по индексу
                list.RemoveAt(index);

                //записываем новый список в accountList.ini
                File.WriteAllLines(Resources.Get(Resources.Names.Ini), list);

                //удаляем .dll файл
                File.Delete($"{Resources.Get(Resources.Names.Dir)}\\{main.Title}.dll");

                MessageBox.Show($"Учет \"{main.Title}\" был безвозвратно удалён", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);

                main.Close();
            }
        }

        //возвращение полей к исходному состоянию
        static void ResetTextBoxes(Main main)
        {
            main.tbVolume.Text = null;
            main.tbCost.Text = null;
            main.tbGS.Text = null;
            main.tbRename.Text = null;

            VanishText(main.tbVolume, Resources.Get(Resources.Names.textVol), true);
            VanishText(main.tbCost, Resources.Get(Resources.Names.textCost), true);
            VanishText(main.tbGS, Resources.Get(Resources.Names.textGS), true);
            VanishText(main.tbRename, Resources.Get(Resources.Names.textForName), true);

            if (WriteAndRead.GetListFda(2)[WriteAndRead.GetListFda(0).IndexOf(main.Title)] == "Не указано")
            {
                main.tbConsumption.Text = null;
                VanishText(main.tbConsumption, Resources.Get(Resources.Names.textMidConsumption), true);
            }

        }

        /// <summary>
        /// Формотирования числовой строчки и округление до 2-х знаков после запятой
        /// </summary>
        /// <param name="content">числовая строчка</param>
        /// <returns></returns>
        public static string Formate(string content)
        {
            content = content.Replace(" ", string.Empty);

            //кол-во знаков после запятой
            int number = 0;

            float.TryParse(content, out float f);
            int index = content.IndexOf(',');

            for (int i = 0; i < content.Length; i++)
                if (i > index && index != -1)
                    number++;

            if (number > 2)
                number = 2;

            //разделяем разряды пробелами
            content = f.ToString($"N{number}");

            return content;
        }

        /// <summary>
        /// Строгое форматирование числа с 'e' знаками после запятой
        /// </summary>
        /// <param name="content">числовая сторчка</param>
        /// <param name="e">количество знаков после запятой</param>
        /// <returns></returns>
        public static string Formate(string content, int e)
        {
            content = content.Replace(" ", string.Empty);
            float.TryParse(content, out float f);

            //разделяем разряды пробелами
            content = f.ToString($"N{e}");

            return content;
        }

        /// <summary>
        /// Заполняет два списка дат
        /// </summary>
        /// <param name="name">имя учета</param>
        /// <param name="cbDate">первый список</param>
        /// <param name="cbDate2">второй список</param>
        public static void FillDates(string name, ComboBox cbDate, ComboBox cbDate2)
        {
            //список неповторяющихся отформатированых дней, кол-во дней
            List<string> days = Calculate.GetOriginalDays(name).Select(d => d.ToShortDateString()).ToList();
            int count = days.Count;

            switch (count)
            {
                case 0:
                    cbDate.ItemsSource = null;
                    cbDate2.ItemsSource = null;

                    cbDate.Items.Add("День 1");
                    cbDate2.Items.Add("День 2");

                    cbDate.SelectedIndex = 0;
                    cbDate2.SelectedIndex = 0;
                    break;

                default:
                    if (cbDate.Items.Contains("День 1"))
                    {
                        cbDate.Items.Clear();
                        cbDate2.Items.Clear();
                    }

                    cbDate.ItemsSource = days;
                    cbDate2.ItemsSource = days;

                    cbDate.SelectedIndex = 0;
                    cbDate2.SelectedIndex = count - 1;
                    break;
            }
        }

        /// <summary>
        /// Меняет ширину окна
        /// </summary>
        /// <param name="main">окно</param>
        /// <param name="e">кооффицент изменения ширины</param>
        /// <param name="action">увелеичение, уменьшение размера</param>
        public static void ResizeWindowWidth(Main main, int e, bool action)
        {
            if (action && main.WindowState != WindowState.Maximized && main.Width <= 700)
                main.Width = 986;

            if (!action && main.WindowState != WindowState.Maximized && main.Width <= 990 && main.Width >= 980)
                main.Width = 688;
        }

        /// <summary>
        /// Открытие окна настроек сохраненных данных
        /// </summary>
        /// <param name="name">имя файла</param>
        /// <param name="main">окно <see cref="Main"/></param>
        /// <param name="sender">объект инициализируемый при нажатии на кнопку в <see cref="Table"/></param>
        public static void OpenCustomizeWindow(string name, Main main, object sender)
        {
            //инициализируем кнопку и массив
            Button button = sender as Button;
            string[][] desposed = WriteAndRead.GetArrayDll(name);

            //получаем индекс строчки и снимаев выделение
            int index = Int32.Parse(button.Tag.ToString());
            main.dgTable.SelectedIndex = -1;

            //настройки содержимого окна корректировки данных
            Customize customize = new Customize { Owner = main };

            customize.lHead.Content = $"Правка данных ({index + 1})";   //заголовок окна
            customize.Title = name;                                     //имя окна, по умолчанию используется для получения имени учета
            customize.tbVolume.Text = desposed[index][3];               //объем заправки
            customize.tbVolume.DataContext = desposed[index][3];
            customize.tbCost.Text = desposed[index][4];                 //стоимость
            customize.tbCost.DataContext = desposed[index][4];
            customize.cbType.Text = desposed[index][2];                 //тип топлива
            customize.cbType.DataContext = desposed[index][2];
            customize.tbConsumption.Text = desposed[index][5];          //средний расход топлива
            customize.tbConsumption.DataContext = desposed[index][5];

            if (desposed[index][1] != "Не указано")
            {
                customize.tbGS.Text = desposed[index][1];               //наименование АЗС
                customize.tbGS.DataContext = desposed[index][1];
            }

            customize.dpChooiceDate.Text = DateTime.Parse
                (desposed[index][0]).ToShortDateString();               //дата
            customize.dpChooiceDate.DataContext = desposed[index][0];

            customize.bSaveData.DataContext = index;                    //индекс элемента в desposed[][]

            customize.ShowDialog();
        }

        //сохроняет измененные данные
        public static void ToCorrectOfData(string name, Main main, Customize customize, int e)
        {
            //инициализация данных
            string[][] desposed = WriteAndRead.GetArrayDll(name);
            string GS = customize.tbGS.Text;
            string type = customize.cbType.Text;
            string volume = customize.tbVolume.Text;
            string cost = customize.tbCost.Text;
            string consumption = customize.tbConsumption.Text;
            string date = customize.dpChooiceDate.DataContext.ToString();
            if (customize.dpChooiceDate.Text != DateTime.Parse(customize.dpChooiceDate.DataContext.ToString()).ToShortDateString())
                date = customize.dpChooiceDate.Text + " 12:30";
            string costLitr = null;
            string transit = null;

            if (GS == "" || GS == null || GS == Resources.Get(Resources.Names.textGS)) GS = "Не указано";


            else if (!Exam.NumericValidation(volume)) ;
            else if (!Exam.NumericValidation(cost)) ;
            else if (!Exam.NumericValidation(consumption)) ;
            else if (!Exam.NamedExam(GS, "наименование АЗС", Resources.Get(Resources.Names.textGS))) ;

            else
            {
                
                customize.Close();

                if (
                    GS != customize.tbGS.DataContext.ToString() ||
                    volume != customize.tbVolume.DataContext.ToString() ||
                    cost != customize.tbCost.DataContext.ToString() ||
                    consumption != customize.tbConsumption.DataContext.ToString() ||
                    type != customize.cbType.DataContext.ToString() ||
                    date != DateTime.Parse(customize.dpChooiceDate.DataContext.ToString()).ToShortDateString() + " 12:30"
                    )
                {
                    costLitr = Calculate.GetCostLitr(volume, cost);
                    transit = Calculate.GetTransit(consumption, volume);

                    //инициализируем массив данных
                    desposed[e] = new string[] { date, GS.Trim(' '), type, Formate(volume), Formate(cost, 2), Formate(consumption), Formate(costLitr), Formate(transit) };

                    //запись данных
                    desposed = Calculate.DesposedSort(desposed);
                    WriteAndRead.WriteDll(name, desposed);

                    DataUpdate(main);

                    MessageBox.Show("Данные успешно изменены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                    

                else
                    MessageBox.Show("Были сохранены исходные данные", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            customize.Close();
        }

        //удаляет выбранный объект учета
        public static void ToDeliteObject(string name, Main main, Customize customize, int e)
        {
            string[][] desposed = WriteAndRead.GetArrayDll(name);
            string[][] newDesposed = new string[desposed.Length - 1][];

            //удаляем выбранный день
            for (int i = 0, j = 0; i < desposed.Length; i++)
                if (i != e)
                    newDesposed[j++] = desposed[i];
                else
                    j++;

            WriteAndRead.WriteDll(name, newDesposed);

            MessageBox.Show("Элемент успешно удален", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            customize.Close();
        }
    }
}

/// имя_учета(0), дата_создания(1), средний_расход_топлива(2), период(3)
/// дата(0), АЗС(1), Тип(2), Объем(3), Стоимость(4), Средний_расход(5), Стоимость_литра(6), Ожидаемая_длина_поездки(7)
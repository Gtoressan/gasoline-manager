using System;
using System.Linq;
using System.Globalization;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Gasoline
{
    class Calculate
    {
        /// <summary>
        /// Возврощает стоимость литра топлива
        /// </summary>
        /// <param name="volume">объем топлива</param>
        /// <param name="cost">цена топлива</param>
        /// <returns></returns>
        public static string GetCostLitr(string volume, string cost)
        {
            //инициализация объема топлива
            float v = float.Parse(volume);
            //инициализация общей стоимости
            float s = float.Parse(cost);

            //высчитывание стоимости литра
            string costLitr = Math.Round(s / v, 2).ToString();

            return costLitr;
        }

        /// <summary>
        /// Возврощает ожидаемую длину поездки
        /// </summary>
        /// <param name="consumption">средний расход топлива</param>
        /// <param name="volume">объем заправленного топлива</param>
        /// <returns></returns>
        public static string GetTransit(string consumption, string volume)
        {
            //инициализация объема топлива
            float v = float.Parse(volume);
            //инициализация среднего расхода
            float c = float.Parse(consumption);

            //высчитывание ожидаемой длину поездки
            string transit = Math.Round((v * 100) / c).ToString();

            return transit;
        }

        /// <summary>
        /// Возврощает массив вхождений каждого элемента в список
        /// </summary>
        /// <param name="list">список</param>
        /// <returns></returns>
        public static float[] GetNumberOccurrences(List<string> list)
        {
            //инициализируем массив неповторяющихся элементов
            string[] variousList = list.Distinct().ToArray();

            //инициализируем массив для числа повторений элементов
            float[] values = new float[variousList.Count()];

            //высчитываем кол-во вхождений каждого элемента
            for (int i = 0; 0 < list.Count && list.Contains(variousList[i]);)
            {
                values[i]++;
                list.Remove(variousList[i]);

                //если больше variousList[i] нет
                if (!list.Contains(variousList[i]))
                    i++;
            }

            return values;
        }

        /// <summary>
        /// Возврощает процентное отношение элементов списка между собой
        /// </summary>
        /// <param name="list">список</param>
        /// <returns></returns>
        public static string[][] GetPersentage(List<string> list)
        {
            string[] variousList = list.Distinct().ToArray();
            float[] values = GetNumberOccurrences(list);

            //находим значение 100 %
            double summ = 0.0d;
            foreach (double i in values)
                summ += i;

            //высчитываем процентное отношение каждого элемента variousList
            for (int i = 0; i < variousList.Length; i++)
                values[i] = float.Parse(Math.Round((values[i] / (summ / 100)), 1).ToString());

            //состовляем топик популярности элементов [элемент][% от общего кол-ва]
            string[][] topic = new string[variousList.Length][];

            for (int i = 0; i < variousList.Length; i++)
                topic[i] = new string[] { variousList[i], values[i].ToString() };

            return topic;
        }

        //высчитываем рейтинги популярности АЗС
        public static string[][] GetGSTopic(string name)
        {
            //получаем список АЗС
            List<string> list = WriteAndRead.GetListDll(name, 1);
            List<string> list2 = WriteAndRead.GetListDll(name, 1);

            //избавляемся от регистро-зависимости (делаем это красиво)
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            for (int i = 0; i < list.Count; i++)
                list[i] = ti.ToTitleCase(list[i]);

            //инициализируем матрицу [АЗС][% отношение]
            string[][] topic = GetPersentage(list);
            float[] number = GetNumberOccurrences(list2);

            for (int i = 0; i < topic.Length; i++)
                topic[i] = new string[] { $"Кол-во упоминаний: {number[i]}\n{topic[i][0]}", topic[i][1] };

            return topic;
        }

        //высчитывание 100 процентов для UploadedDay
        public static float Get100Percent(string name)
        {
            //стоимость, объем, средний расход топлива
            List<float> cost = GetDailyOption(name, 4);
            List<float> vol = GetDailyOption(name, 3);
            List<float> midConsumption = GetDailyOption(name, 5);

            float a = GetMediana(cost) * 0.1f + GetMediana(vol) * 0.7f + GetMediana(midConsumption) * 0.2f;
            a = float.Parse(Actions.Formate(a.ToString()));

            return a;
        }

        /// <summary>
        /// Возврощает медиану значений списка
        /// </summary>
        /// <param name="list">список значений</param>
        /// <returns></returns>
        public static float GetMediana(List<float> list)
        {
            list.Sort();
            int count = list.Count - 1;

            if (count % 2 == 0 && list.Count > 1)
                return list[int.Parse((0.5 * count).ToString())] + list[int.Parse((0.5 * count + 1).ToString())] / 2;
            else if (count % 2 == 1 && list.Count > 1)
                return list[int.Parse((0.5 * (count + 1)).ToString())];

            return list[0];
        }

        /// <summary>
        /// Возврощяет список каждодневных значений параметра 'е', существующих в .Dll
        /// </summary>
        /// <param name="name">имя файла .dll без расширения</param>
        /// <param name="e">индекс параметра</param>
        /// <returns></returns>
        public static List<float> GetDailyOption(string name, int e)
        {
            //инициализируем список всех дней, параметров, зависимых параметров и неповторяющихся дней
            List<DateTime> allDays = GetDays(name);
            List<float> allOptions = WriteAndRead.GetListDll(name, e).Select(option => float.Parse(option)).ToList();
            float[] options = new float[GetOriginalDays(name).Count];
            List<DateTime> days = GetOriginalDays(name);
            int count = allDays.Count;

            for (int i = 0, j = 0; i < count; i++)
            {
                //увеличиваем j, если в списке всех дней не нашелся
                //оригинальный день days[j]
                if (!allDays.Contains(days[j]))
                    j++;

                //переносим в options[j] элемент из allOptions
                options[j] += allOptions[i];

                //удаляем элемент days[j] из allDays, чтобы
                //двигать j дальше
                allDays.Remove(days[j]);
            }

            return options.ToList();
        }

        /// <summary>
        /// Возврощает список каждодневных значений параметра 'e', с учетом границ времени
        /// </summary>
        /// <param name="name">имя файла .dll без расширения</param>
        /// <param name="date">правая граница времени</param>
        /// <param name="date2">левая граница времени</param>
        /// <param name="e">индекс параметра</param>
        /// <returns></returns>
        public static List<float> GetDailyCustomOption(string name, DateTime date, DateTime date2, int e)
        {
            //костомный параметр, неповторяющиеся дни, дне-зависивый параметр
            List<float> list = new List<float>();
            List<DateTime> days = GetOriginalDays(name);
            List<float> option = GetDailyOption(name, e);

            //получение костомных данных параметра "е"
            int index = days.IndexOf(date);
            int lastIndex = days.IndexOf(date2);

            for (; index <= lastIndex; index++)
                list.Add(option[index]);

            return list;
        }

        /// <summary>
        /// Возврощает список всех значений параметра 'e', с учетом границ времени
        /// </summary>
        /// <param name="name">имя файла .dll без расширения</param>
        /// <param name="date">правая граница времени</param>
        /// <param name="date2">левая граница времени</param>
        /// <param name="e">индекс параметра</param>
        /// <returns></returns>
        public static List<float> GetCustomOption(string name, DateTime date, DateTime date2, int e)
        {
            //костомный параметр, дни, значения параметра
            List<float> list = new List<float>();
            List<DateTime> days = GetDays(name);
            List<float> option = WriteAndRead.GetListDll(name, e).Select(day => float.Parse(day)).ToList();

            //инициализация начального и конечного индекса
            int index = days.IndexOf(date);
            int lastIndex = days.LastIndexOf(date2);

            //составление списка кастомного параметра
            for (; index <= lastIndex; index++)
                list.Add(option[index]);

            return list;
        }

        /// <summary>
        /// Получаем список неповторяющихся дней
        /// </summary>
        /// <param name="name">имя файла без расширения</param>
        /// <returns></returns>
        public static List<DateTime> GetOriginalDays(string name)
        {
            //инициализируем список всех дней подряд
            List<DateTime> allDays = WriteAndRead.GetListDll(name, 0).Select(day => DateTime.Parse(day)).ToList();

            //инициализируем список неповторяющихся дней
            List<DateTime> days = new List<DateTime>();

            //заполняем список не повторяющимися днями
            for (int i = 0; i < allDays.Count; i++)
            {
                //инициализируем шаблон дня [день/месяц/год]
                DateTime d = new DateTime(allDays[i].Year, allDays[i].Month, allDays[i].Day);

                //добавляем день
                if (!days.Contains(d))
                    days.Add(d);
            }

            return days;
        }

        /// <summary>
        /// Возврощает весь список дней в формате [день][месяц][год]
        /// </summary>
        /// <param name="name">имя файла без расширения</param>
        /// <returns></returns>
        public static List<DateTime> GetDays(string name)
        {
            //список всех дней без приведения к формату
            List<string> allDays = WriteAndRead.GetListDll(name, 0);
            List<DateTime> days = new List<DateTime>();

            //приводим элементы allDays к формату и переписываем
            //в список days
            int count = allDays.Count;

            for (int i = 0; i < count; i++)
            {
                DateTime tmp = DateTime.Parse(allDays[i]);
                DateTime d = new DateTime(tmp.Year, tmp.Month, tmp.Day);
                days.Add(d);
            }

            return days;
        }

        /// <summary>
        /// Возврощает список дней с учетом крайних значений
        /// </summary>
        /// <param name="name">имя файла .dll без расширения</param>
        /// <param name="date">ближняя граница времени</param>
        /// <param name="date2">дальняя граница времени</param>
        /// <returns></returns>
        public static List<DateTime> GetOriginalDays(string name, DateTime date, DateTime date2)
        {
            //список всех дней, возвращаемый списокдней
            List<DateTime> allDays = GetOriginalDays(name);
            List<DateTime> days = new List<DateTime>();

            //индексы крайних элементов
            int index = allDays.IndexOf(date);
            int lastIndex = allDays.IndexOf(date2);

            //заполняем days согласно индексам
            for (; index <= lastIndex; index++)
                days.Add(allDays[index]);

            return days;
        }

        /// <summary>
        /// Записываем в TextBox сумму значений параметра 'е', с учетом границ времени
        /// </summary>
        /// <param name="name">имя файла .dll без расширения</param>
        /// <param name="e">индекс параметра</param>
        /// <param name="date">ближная граница времени</param>
        /// <param name="date2">дальняя граница времени</param>
        /// <param name="textBox">текстовое поле для записи</param>
        /// <param name="text">вспомогательный текст</param>
        public static void TotalNumber(string name, int e, DateTime date, DateTime date2, TextBox textBox, string text)
        {
            //список значения параметра "е"
            List<float> list = GetDailyCustomOption(name, date, date2, e);

            //сумма значение параметра "е"
            float summ = list.Sum();

            //приведения к виду N2
            string total = Actions.Formate(summ.ToString());

            //запись в тектовое поле
            textBox.Text = $"Всего {total} {text} за указанный промежуток";
        }
        
        /// <summary>
        /// Записывает в TextBox среднее значение параметра 'e', с учетом границ времени
        /// </summary>
        /// <param name="name">имя файла .dll без расширения</param>
        /// <param name="e">индекс параметра</param>
        /// <param name="date">ближная граница времени</param>
        /// <param name="date2">дальняя граница времени</param>
        /// <param name="textBox">текстовое поле для записи</param>
        /// <param name="text">вспомогательный текст</param>
        /// <param name="text2">второй вспомогательный текст</param>
        public static void MidleNumber(string name, int e, DateTime date, DateTime date2, TextBox textBox, string text, string text2)
        {
            //список всех значений и список подневных значений
            List<float> list = GetCustomOption(name, date, date2, e);
            List<float> listD = GetDailyCustomOption(name, date, date2, e);

            string mid = Actions.Formate((list.Sum() / list.Count).ToString());
            string midD = Actions.Formate((listD.Sum() / listD.Count).ToString());

            //заполнение текстового поля
            textBox.Text = $"В среднем {mid} {text} и {midD} {text2}";
        }


        /// <summary>
        /// Обновление данных в разделе "Аналитика"
        /// </summary>
        /// <param name="main">окно mainWindow</param>
        public static void InfoUpdate(Main main)
        {
            if (WriteAndRead.GetListDll(main.Title, 0).Count != 0)
            {
                DateTime date = GetOriginalDays(main.Title)[main.cbDate.SelectedIndex];
                DateTime date2 = GetOriginalDays(main.Title)[main.cbDate2.SelectedIndex];
                int count = GetOriginalDays(main.Title, date, date2).Count, period = 1;

                if (count >= 9) period = 7;

                //правка дней по возрастанию
                if (date > date2)
                {
                    DateTime tmpDate = date;
                    date = date2;
                    date2 = tmpDate;
                }

                TotalNumber(main.Title, 3, date, date2, main.tbAllVolumeInfo, "лит. было залито");
                TotalNumber(main.Title, 4, date, date2, main.tbAllCostInfo, "руб. было затрачено на заправку");
                TotalNumber(main.Title, 7, date, date2, main.tbAllTransitInfo, "км дороги было преодалено (ожидаемо)");
                MidleNumber(main.Title, 3, date, date2, main.tbMiddleVolumeInfo, "лит. заливается на каждой заправке", "лит. каждый день");
                MidleNumber(main.Title, 4, date, date2, main.tbMiddleCostInfo, "руб. тратится на каждой заправке", "руб. каждый день");

                float _100percent = Get100Percent(main.Title);

                UploadedInfo(main, _100percent, period, date, date2);
            }

            else
            {
                main.tbUploadedDaysInfo.Text = "Заполняйте учет новыми данными и получайте актульные сведения";
                main.tbInfo.Text = "Здесь будет указано количество литров залитых в самые нагруженные дни";
                main.tbInfo2.Text = "Здесь будет указан объем затрат на заправку в самые нагруженные дни";
                main.tbAllVolumeInfo.Text = null;
                main.tbAllCostInfo.Text = null;
                main.tbAllTransitInfo.Text = null;
                main.tbMiddleVolumeInfo.Text = null;
                main.tbMiddleCostInfo.Text = null;
            }
        }

        /// <summary>
        /// Заполняет подраздел UploadedDay согласно полученному массу Uploaded
        /// </summary>
        /// <param name="main">окно</param>
        /// <param name="_100percent">значений 100 процентов</param>
        /// <param name="e">период времени</param>
        /// <param name="date">ближняя граница времени</param>
        /// <param name="date2">дальняя граница времени</param>
        static void UploadedInfo(Main main, float _100percent, int e, DateTime date, DateTime date2)
        {
            //рванный массив uploaded
            //день(0), процент нагрузки(1), объем заправки(2), 
            //стоимость(3), средний расход топлива(4)
            string[,][] uploadedMatrix = GetUploaded(main.Title, e, date, date2, _100percent);
            string[][] uploaded = UploadedSort(uploadedMatrix, e);
            int length = uploadedMatrix.GetLength(0) + e;
            int count = uploaded.Length;

            if (length <= 9)
            {
                main.tbUploadedDaysInfo.Text = $"Самый нагруженный день за указанный период: {uploaded[0][0]}, нагрузка составила: {uploaded[0][1]}%";
                main.tbInfo.Text = $"В самый нагруженный день было заправлено {uploaded[0][2]} лит. топлива";
                main.tbInfo2.Text = $"Затрачено на заправку было {uploaded[0][3]} руб.";
            }

            else if (length > 9)
            {
                main.tbUploadedDaysInfo.Text = $"Самый нагруженый 7-ми дневный промежуток: {uploaded[0][0]} - {uploaded[e - 1][0]}";
                main.tbInfo.Text = $"В саммый нагруженный период было заправлено {uploaded[e][1]} лит. топлива";
                main.tbInfo2.Text = $"В саммый нагруженный период было потрачено {uploaded[e][2]} руб.";
            }
        }

        /// <summary>
        /// Возврощает рванный список с уровнем нагрузки
        /// </summary>
        /// <param name="name">имя файла .dll без расширения</param>
        /// <param name="e">период времени</param>
        /// <param name="date">ближнаяя граница</param>
        /// <param name="date2">дальнаяя граница</param>
        /// <returns></returns>
        static string[,][] GetUploaded(string name, int e, DateTime date, DateTime date2, float _100percent)
        {
            //оригинальные дни, объем, стоимость, средний расход
            List<DateTime> days = GetOriginalDays(name, date, date2);
            float[] vol = GetDailyCustomOption(name, date, date2, 3).ToArray();
            float[] cost = GetDailyCustomOption(name, date, date2, 4).ToArray();
            float[] midConsumption = GetDailyCustomOption(name, date, date2, 5).ToArray();
            int count = days.Count - e + 1;
            string[,][] uploaded = new string[count, e][];

            //заполняем рванный масси uploaded
            for (int i = 0, k = 0; i < count; i++)
            {
                for (int j = 0; j < e; j++)
                {
                    uploaded[i, k] = new string[5];

                    uploaded[i, k][0] = days[i + j].ToShortDateString();

                    float a = cost[i + j] * 0.1f + vol[i + j] * 0.7f + midConsumption[i + j] * 0.2f;
                    float upload = 100 * a / _100percent;
                    uploaded[i, k][1] = Actions.Formate((upload).ToString());

                    uploaded[i, k][2] = vol[i + j].ToString();
                    uploaded[i, k][3] = Actions.Formate(cost[i + j].ToString(), 2);
                    uploaded[i, k++][4] = midConsumption[i + j].ToString();
                }

                k = 0;
            }

            return uploaded;
        }

        /// <summary>
        /// Сортирует массив uploaded по убыванию, согласно уровню нагруженности
        /// </summary>
        /// <param name="uploaded">массив формата uploaded</param>
        /// <param name="e">велечина периода</param>
        /// <returns></returns>
        public static string[][] UploadedSort(string[,][] uploaded, int e)
        {
            //двумерный массив для сохранения саммого нагруженного периода,
            //массив для поиска индекса самого нагруженного периода
            string[][] array = new string[e + 1][];
            float[] upl = new float[uploaded.GetLength(0)];
            float[] vol = new float[uploaded.GetLength(0)];
            float[] cst = new float[uploaded.GetLength(0)];

            //проход через uploaded массив с заполнение indexes
            for (int i = 0; i < uploaded.GetLength(0); i++)
                for (int j = 0; j <= e; j++)
                    if (j < e)
                    {
                        upl[i] += float.Parse(uploaded[i, j][1]);       //нагрузка
                        vol[i] += float.Parse(uploaded[i, j][2]);       //объем заправки
                        cst[i] += float.Parse(uploaded[i, j][3]);       //стоимость заправки
                    }
                    else
                        upl[i] = (upl[i] + float.Parse(uploaded[i, j - 1][1])) / e;

            //получаем индекс самого нагруженненного периода
            int index = vol.ToList().IndexOf(vol.Max());

            //заполняем массив array, согласно индексу
            for (int i = 0; i < e; i++)
                array[i] = uploaded[index, i];

            //записываем в последний массив array суммарные значения
            array[e] = new string[3];
            array[e][0] = Actions.Formate(upl[index].ToString());       //средняя арифметическая нагрузка за саммый нагруженный период 
            array[e][1] = Actions.Formate(vol[index].ToString());       //сумма заправок за самый нагруженный период
            array[e][2] = Actions.Formate(cst[index].ToString(), 2);    //суммарные затраты за самый нагруженный период

            return array;
        }

        /// <summary>
        /// Сортирует массив относительно дней по возрастанию
        /// </summary>
        /// <param name="array">массив</param>
        /// <returns></returns>
        public static string[][] DesposedSort(string[][] array)
        {
            //состовляем список дней для сравнения
            List<DateTime> list = new List<DateTime>();

            foreach (string[] i in array)
                list.Add(DateTime.Parse(i[0]));

            //переписываем данные в новый массив
            string[][] sorting = new string[array.Length][];

            for (int i = 0; i < list.Count; i++)
            {
                int index = list.IndexOf(list.Min());
                list[index] = DateTime.MaxValue;

                sorting[i] = array[index];
            }

            return sorting;
        }
    }
}

/// имя_учета(0), дата_создания(1), средний_расход_топлива(2)
/// дата(0), АЗС(1), Тип(2), Объем(3), Стоимость(4), Средний_расход(5), Стоимость_литра(6), Ожидаемая_длина_поездки(7)
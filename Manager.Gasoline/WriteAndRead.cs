using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Gasoline
{
    /// <summary>
    /// Модуль работы с данными (запись/чтение)
    /// </summary>
    class WriteAndRead
    {
        /// <summary>
        /// Cоздание необходимых файлов и каталогов
        /// </summary>
        public static void CreateFileAndFirectory()
        {
            //создание файла accountingList.ini
            if (!File.Exists(Resources.Get(Resources.Names.Ini)))
                File.Create(Resources.Get(Resources.Names.Ini)).Close();

            //создание каталога accounting
            if (!Directory.Exists(Resources.Get(Resources.Names.Dir)))
                Directory.CreateDirectory(Resources.Get(Resources.Names.Dir));
        }

        /// <summary>
        /// Записываем новый учет в listF
        /// </summary>
        /// <param name="name"></param>
        /// <param name="e">количество дополнительных свойств</param>
        public static void WriteInFda(string name, int e)
        {
            //инициализируем время записи учета (дд.мм.гггг чч:мм)
            string date = $"|{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}";

            //инициализируем остальные свойства
            string options = null;

            for (int i = 0; i < e; i++)
                options += "|Не указано";

            //инициализируем массив свойств учета
            string[] array = { $"{name}{date}{options}" };

            //добавляем массив свойств учета в acountList.ini
            File.AppendAllLines(Resources.Get(Resources.Names.Ini), array);
        }

        /// <summary>
        /// Меняем значение свойства учета в listF
        /// </summary>
        /// <param name="name">имя файла без расширения</param>
        /// <param name="content">новое записываемое значение</param>
        /// <param name="e">индекс параметра</param>
        /// <returns></returns>
        public static string ChangeFdaPoint(string name, string content, int e)
        {
            //инициализируем разложенный массив свойств
            string[][] desposedArray = GetArrayFda();

            //инициализируем индекс по списку имен
            List<string> list = GetListFda(0);
            int index = list.IndexOf(name);

            //сохраняем значение свойства по индексу
            string oldOption = desposedArray[index][e];

            //записываем новое значение свойства по индексу
            desposedArray[index][e] = $"{content}";

            //инициализируем однострочный массив
            string[] array = new string[desposedArray.Length];
            for (int i = 0; i < desposedArray.Length; i++)
                for (int j = 0; j < desposedArray[i].Length; j++)
                    if (j == 0)
                        array[i] += $"{desposedArray[i][j]}";
                    else
                        array[i] += $"|{desposedArray[i][j]}";

            //переписываем файл listF
            File.WriteAllLines(Resources.Get(Resources.Names.Ini), array);

            return oldOption;
        }

        /// <summary>
        /// Записываем данные в .dll файл
        /// </summary>
        /// <param name="name">имя файла без расширения</param>
        /// <param name="array">записываемый массив</param>
        public static void WriteDll(string name, string[] array)
        {
            //инициализируем время записи данных (дд.мм.гггг чч:мм)
            string date = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}";

            //инициализируем отформатированный массив данных
            string[] formattedArray = new string[1];

            formattedArray[0] = date;

            for (int i = 0; i < array.Length; i++)
                formattedArray[0] += $"|{array[i]}";

            File.AppendAllLines($"{Resources.Get(Resources.Names.Dir)}\\{name}.dll", formattedArray);
        }

        /// <summary>
        /// Записываем данные в .dll файл
        /// </summary>
        /// <param name="name">имя файла без расширения</param>
        /// <param name="array">записываемый рванный массив</param>
        public static void WriteDll(string name, string[][] array)
        {
            foreach (string[] i in array)
            {
                string[] tmpArray = new string[1];
                tmpArray[0] = i[0];
                for (int j = 1; j < i.Length; j++)
                    tmpArray[0] += "|" + i[j];

                File.AppendAllLines(Resources.Get(Resources.Names.backupFile), tmpArray);
            }

            File.Replace(Resources.Get(Resources.Names.backupFile), $"{Resources.Get(Resources.Names.Dir)}\\{name}.dll", "non.dll");
            File.Delete("non.dll");
        }


        /// <summary>
        /// Получаем разложенный массив свойств из listF
        /// </summary>
        public static string[][] GetArrayFda()
        {
            //инициализируем одномерный массив строк
            string[] array = File.ReadAllLines(Resources.Get(Resources.Names.Ini)).ToArray();

            //инициализируем разложенный массив по знаку "|"
            string[][] decomposedArray = new string[array.Length][];

            for (int i = 0; i < array.Length; i++)
                decomposedArray[i] = array[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            return decomposedArray;
        }

        /// <summary>
        /// Получаем список указанного свойства из listF
        /// </summary>
        /// <param name="e">индекс необходимого параметра</param>
        /// <returns></returns>
        public static List<string> GetListFda(int e)
        {
            //инициализация списка
            List<string> list = new List<string>();

            //инициализация разложенного массива
            string[][] decomposedArray = GetArrayFda();

            //получения списка указанного параметра
            for (int i = 0; i < decomposedArray.Length; i++)
                list.Add(decomposedArray[i][e]);

            return list;
        }

        /// <summary>
        /// Получаем разложенный массив параметров из .dll
        /// </summary>
        /// <param name="name">имя файла без расширения</param>
        /// <returns></returns>
        public static string[][] GetArrayDll(string name)
        {
            //инициализируем массив дней файла .dll
            string[] days = File.ReadAllLines($"{Resources.Get(Resources.Names.Dir)}\\{name}.dll");

            //инициализируем разложенный массив [день][параметр]
            string[][] desposedDays = new string[days.Length][];

            //разбиваем разбиваем дни на параметры
            for (int i = 0; i < days.Length; i++)
                desposedDays[i] = days[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            return desposedDays;
        }

        /// <summary>
        /// Получаем список указанного параметра из .dll
        /// </summary>
        /// <param name="name"></param>
        /// <param name="e">индекс необходимого параметра</param>
        /// <returns></returns>
        public static List<string> GetListDll(string name, int e)
        {
            //инициализация списка
            List<string> list = new List<string>();

            //инициализация разложенного массива
            string[][] decomposedArray = GetArrayDll(name);

            //получение списка указанного параметра
            for (int i = 0; i < decomposedArray.Length; i++)
                list.Add(decomposedArray[i][e]);

            return list;
        }



        /// <summary>
        /// Получаем список для заполнения таблицы
        /// </summary>
        /// <param name="name">имя файла без расширения</param>
        /// <returns></returns>
        public static List<Table> ReadForTable(string name)
        {
            List<Table> list = new List<Table>();
            string[][] array = GetArrayDll(name);

            for (int i = 0; i < array.Length; i++)
                list.Add(new Table(array[i][0], array[i][1], array[i][2], array[i][3], array[i][4], array[i][5], array[i][6] + " руб.", array[i][7] + " км."));

            return list;
        }

        /// <summary>
        /// Получаем список для первой диграммы
        /// </summary>
        /// <param name="name">имя файла без расширения</param>
        /// <param name="version">0 - по записям, 1 - по дням</param>
        /// <returns></returns>
        public static List<FirstChart> ReadForFirstChart(string name, int version)
        {
            //инициализируем список-шаблон для заполнения диаграммы
            List<FirstChart> list = new List<FirstChart>();

            if (version == 0)
            {
                //инициализируем массив данных учета
                string[][] array = GetArrayDll(name);

                //заполняем список данными из учета
                for (int i = 0; i < array.Length; i++)
                    list.Add(new FirstChart($"{array[i][2]}\n{array[i][0]}\nЗалито (лит.):", float.Parse(array[i][3])));
            }

            else if (version == 1)
            {
                //список оригиральных дней, список подневных заправок
                List<DateTime> days = Calculate.GetOriginalDays(name);
                List<float> vol = Calculate.GetDailyOption(name, 3);

                //заполняем список данными из учета
                for (int i = 0; i < days.Count; i++)
                    list.Add(new FirstChart($"{days[i].ToShortDateString()}\nЗалито (лит.):", vol[i]));
            }

            return list;
        }

        /// <summary>
        /// Получаем список для второй диаграммы
        /// </summary>
        /// <param name="name">имя файла без расширения</param>
        /// <param name="version">0 - по записям, 1 - по дням</param>
        /// <returns></returns>
        public static List<SecondChart> ReadForSecondChart(string name, int version)
        {
            //инициализируем список-шаблон для заполнения диаграммы
            List<SecondChart> list = new List<SecondChart>();

            if (version == 0)
            {
                //инициализируем массив данных
                string[][] array = GetArrayDll(name);

                //заполняем список данными из учета
                for (int i = 0; i < array.Length; i++)
                    list.Add(new SecondChart($"{array[i][1]}\n{array[i][0]}\nЗалито на:", float.Parse(array[i][4])));
            }
            else if (version == 1)
            {
                //оригинальные дни, затраты
                List<DateTime> days = Calculate.GetOriginalDays(name);
                List<float> cost = Calculate.GetDailyOption(name, 4);

                //заполняем список данными из учета
                for (int i = 0; i < days.Count; i++)
                    list.Add(new SecondChart($"{days[i].ToShortDateString()}\nЗалито на:", cost[i]));
            }

            return list;
        }

        /// <summary>
        /// Получаем список для круговой диаграммы
        /// </summary>
        /// <param name="name">имя файла без расширения</param>
        /// <returns></returns>
        public static List<PieChart> ReadForPieChart(string name)
        {
            //инициализируем список-шаблон для заполнения диаграммы
            List<PieChart> list = new List<PieChart>();

            //инициализируем массив популярности АЗС
            string[][] topic = Calculate.GetGSTopic(name);

            //заполняем список данными из учета
            for (int i = 0; i < topic.Length; i++)
                list.Add(new PieChart(topic[i][0], float.Parse(topic[i][1])));

            return list;
        }
    }
}

/// имя_учета(0), дата_создания(1), средний_расход_топлива(2)
/// дата(0), АЗС(1), Тип(2), Объем(3), Стоимость(4), Средний_расход(5), Стоимость_литра(6), Ожидаемая_длина_поездки(7)
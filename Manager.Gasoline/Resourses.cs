namespace Manager.Gasoline
{
    //текстовые ресурсы
    public class Resources
    {
        public enum Names
        {
            Dir = 1,
            Ini = 2,

            textGS = 3,
            textVol = 4,
            textForName = 5,
            textMidConsumption = 6,
            textCost = 7,

            defoltItem = 8,
            backupFile = 9
        }

        /// <summary>
        /// Возврощает стрококвое значение данных <see cref="Names"/>
        /// </summary>
        /// <param name="voice">необходимое значение из <see cref="Names"/></param>
        /// <returns></returns>
        public static string Get(Names type)
        {
            switch (type)
            {
                case Names.Dir:
                    return "accounting";

                case Names.Ini:
                    return "accounts.ini";

                case Names.textGS:
                    return "Укажите здесь наименование АЗС";

                case Names.textVol:
                    return "Укажите здесь кол-во заправленных литров";

                case Names.textForName:
                    return "Укажите имя здесь";

                case Names.textMidConsumption:
                    return "Укажите здесь средний расход";

                case Names.textCost:
                    return "Укажите здесь стоимость заправки";

                case Names.defoltItem:
                    return "Вы еще не завели ни одного учета";

                case Names.backupFile:
                    return "backUpFile.dll";
            }

            return "null";
        }
    }

    //шаблон для заполнения таблицы
    class Table
    {
        public Table(string date, string gs, string type, string volume, string cost, string consumption, string costLitr, string transit)
        {
            Date = date;
            GS = gs;
            Type = type;
            Volume = volume;
            Cost = cost;
            Consumption = consumption;
            CostLitr = costLitr;
            Transit = transit;
        }
        public string Date { get; set; }
        public string GS { get; set; }
        public string Type { get; set; }
        public string Volume { get; set; }
        public string Cost { get; set; }
        public string Consumption { get; set; }
        public string CostLitr { get; set; }
        public string Transit { get; set; }
    }

    //шаблон для заполнения первой диаграммы
    public class FirstChart
    {
        public FirstChart(string dateAndType, float volume)
        {
            DateAndType = dateAndType;
            Volume = volume;
        }
     

    //шаблон для заполнения второй диаграммы
    public class SecondChart
    {
        public SecondChart(string dateAndGS, float cost)
        {
            DateAndGS = dateAndGS;
            Cost = cost;
        }
        public string DateAndGS { get; set; }
        public float Cost { get; set; }
    }

    //шаблон для заполнения круговой диаграммы
    public class PieChart
    {
        public PieChart(string GSAndPoint, float percent)
        {
            GS = GSAndPoint;
            Percent = percent;
        }
        public string GS { get; set; }
        public float Percent { get; set; }
    }
}

/// имя_учета(0), дата_создания(1), средний_расход_топлива(2)
/// дата(0), АЗС(1), Тип(2), Объем(3), Стоимость(4), Средний_расход(5), Стоимость_литра(6), Ожидаемая_длина_поездки(7)
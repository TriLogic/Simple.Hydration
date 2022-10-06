using Simple.Hydration;
using System.Data;

namespace Simple.Hydration.Test
{

    public class Meal
    {
        public string MealName;
    }

    public class MealInfo : Meal
    {
        public DateTime MealTime { get; set; }
    }

    public class MealTime : Meal
    {
        [HydrateWith("MealTime")]
        public DateTime TimeOfDay { get; set; }
    }


    public class Program
    {
        static void Main()
        {
            HydrateSingle();

            Console.WriteLine();

            HydrateMany();

            Console.ReadKey();
        }

        public static void HydrateSingle()
        {
            Console.WriteLine("Hydrate Many:");

            Dictionary<string, string> values = new Dictionary<string, string>
            {
                { "MealName", "Dinner" },
                { "MealTime", DateTime.Now.ToString() },
            };

            var meal = new Hydrator<MealInfo>()
                .Hydrate(key =>
                {
                    return values[key];
                });

            Console.WriteLine($"Hydrated Meal: {meal.MealName}, Time: {meal.MealTime}");
        }


        public static void HydrateMany()
        {
            Console.WriteLine("Hydrate Many:");

            DataTable agenda = new DataTable();
            agenda.Columns.Add("MealName", typeof(string));
            agenda.Columns.Add("MealTime", typeof(string));

            string today = DateTime.Now.ToString("M/d/yyyy");

            agenda.Rows.Add(new[] { "Breakfast", DateTime.Parse(today).AddHours(6).AddMinutes(30).ToString() } );
            agenda.Rows.Add(new[] { "Lunch", DateTime.Parse(today).AddHours(12).ToString() });
            agenda.Rows.Add(new[] { "Dinner", DateTime.Parse(today).AddHours(18).ToString() });

            var hydrator = new Hydrator<MealTime>();
            Dictionary<string, int> OrdinalCache = new();
            var list = hydrator.Hydrate<DataRow>(agenda.AsEnumerable(), (row, key) =>
            {
                if (OrdinalCache.ContainsKey(key))
                    return row.ItemArray[OrdinalCache[key]].ToString();

                var ordinal = row.Table.Columns[key].Ordinal;
                OrdinalCache[key] = ordinal;
                return row.ItemArray[ordinal].ToString();
            });

            list.ForEach(meal =>
            {
                Console.WriteLine($"Hydrated Meal: {meal.MealName}, Time: {meal.TimeOfDay}");
            });

        }
    }

    // public static System.Data.EnumerableRowCollection<System.Data.DataRow> AsEnumerable(this System.Data.DataTable source);

}
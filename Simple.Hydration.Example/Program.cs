using Simple.Hydration;
using System.Data;

namespace Simple.Hydration.Example
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

    public class MealWithGuests : MealTime
    {
        [HydrateWith("Guests")]
        public int? GuestCount { get; set; }
    }

    public class Program
    {
        private static DateTime Now = DateTime.Now;

        private static string KeyName_MealName = "MealName";
        private static string KeyName_MealTime = "MealTime";
        private static string KeyName_MealGuests = "Guests";

        private static string NameOfBreakfast = "Breakfast";
        private static string NameOfLunch = "Lunch";
        private static string NameOfSnacks = "Snacks";
        private static string NameOfDinner = "Dinner";

        private static DateTime TimeForBreakfast;
        private static DateTime TimeForLunch;
        private static DateTime TimeForSnacks;
        private static DateTime TimeForDinner;

        private static int GuestsAtBreakfast = 0;
        private static int GuestsAtLunch = 1;
        private static int GuestsAtSnacks = 5;
        private static int GuestsAtDinner = 2;

        private static DataTable? Meals;

        private static Dictionary<string, string?>? SingleMealDinner;

        private static List<string> GuestsNotIncluded = new List<string>() { KeyName_MealName, KeyName_MealTime };
        private static List<string> GuestsExcluded = new List<string>() { KeyName_MealGuests };

        private static MealWithGuests TheMealBreakfast;
        private static MealWithGuests TheMealLunch;
        private static MealWithGuests TheMealSnacks;
        private static MealWithGuests TheMealDinner;

        private static List<MealWithGuests> MealBenchMarks;

        static void Main()
        {
            Setup();

            HydrateSingleFromDictionary();

            Console.WriteLine();

            HydrateManyFromDataTable();

            Console.WriteLine();

            HydrateManyFromDataTableSkipField();

            Console.ReadKey();
        }

        private static void Setup()
        {
            TimeForBreakfast = new DateTime(Now.Year, Now.Month, Now.Day, 6, 30, 0, 0);
            TimeForLunch = new DateTime(Now.Year, Now.Month, Now.Day, 12, 0, 0, 0);
            TimeForSnacks = new DateTime(Now.Year, Now.Month, Now.Day, 13, 45, 0, 0);
            TimeForDinner = new DateTime(Now.Year, Now.Month, Now.Day, 19, 0, 0, 0);

            Meals = new DataTable();
            Meals.Columns.Add(KeyName_MealName, typeof(string));
            Meals.Columns.Add(KeyName_MealTime, typeof(string));
            Meals.Columns.Add(KeyName_MealGuests, typeof(int));

            Meals.Rows.Add(new[] { NameOfBreakfast, TimeForBreakfast.ToString(), GuestsAtBreakfast.ToString() });
            Meals.Rows.Add(new[] { NameOfLunch, TimeForLunch.ToString(), GuestsAtLunch.ToString() });
            Meals.Rows.Add(new[] { NameOfSnacks, TimeForSnacks.ToString(), GuestsAtSnacks.ToString() });
            Meals.Rows.Add(new[] { NameOfDinner, TimeForDinner.ToString(), GuestsAtDinner.ToString() });

            SingleMealDinner = new Dictionary<string, string?>
            {
                { KeyName_MealName, NameOfDinner },
                { KeyName_MealTime, TimeForDinner.ToString() },
                { KeyName_MealGuests, GuestsAtDinner.ToString() },
            };

            TheMealBreakfast = new MealWithGuests { MealName = NameOfBreakfast, TimeOfDay = TimeForBreakfast, GuestCount = GuestsAtBreakfast };
            TheMealLunch = new MealWithGuests { MealName = NameOfLunch, TimeOfDay = TimeForLunch, GuestCount = GuestsAtLunch };
            TheMealSnacks = new MealWithGuests { MealName = NameOfSnacks, TimeOfDay = TimeForSnacks, GuestCount = GuestsAtSnacks };
            TheMealDinner = new MealWithGuests { MealName = NameOfDinner, TimeOfDay = TimeForDinner, GuestCount = GuestsAtDinner };

            MealBenchMarks = new List<MealWithGuests>
            {
                TheMealBreakfast,
                TheMealLunch,
                TheMealSnacks,
                TheMealDinner
            };
        }


        public static void HydrateSingleFromDictionary()
        {
            Console.WriteLine("Hydrate Single:");

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


        public static void HydrateManyFromDataTable()
        {
            var hydrator = new Hydrator<MealWithGuests>();
            Dictionary<string, int> OrdinalCache = new();

            var list = hydrator.HydrateMany<DataRow>(Meals.AsEnumerable(), (row, key) =>
            {
                if (OrdinalCache.ContainsKey(key))
                    return row.ItemArray[OrdinalCache[key]].ToString();

                var ordinal = row.Table.Columns[key].Ordinal;
                OrdinalCache[key] = ordinal;
                return row.ItemArray[ordinal].ToString();
            });

            foreach (MealWithGuests meal in list)
            {
                Console.WriteLine($"Hydrated Meal: {meal.MealName}, Time: {meal.TimeOfDay}, Guests: {meal.GuestCount}");
            }
        }

        public static void HydrateManyFromDataTableSkipField()
        {
            var hydrator = new Hydrator<MealWithGuests>();
            Dictionary<string, int> OrdinalCache = new();

            var list = hydrator.HydrateMany<DataRow>(Meals.AsEnumerable(), (row, key) =>
            {
                // don't populate the guests field
                if (key == "Guests")
                    return (null, true);

                if (OrdinalCache.ContainsKey(key))
                    return (row.ItemArray[OrdinalCache[key]].ToString(), false);

                var ordinal = row.Table.Columns[key].Ordinal;
                OrdinalCache[key] = ordinal;

                return (row.ItemArray[ordinal].ToString(), false);
            });

            foreach (MealWithGuests meal in list)
            {
                Console.WriteLine($"Hydrated Meal: {meal.MealName}, Time: {meal.TimeOfDay}, Guests: {meal.GuestCount}");
            }
        }
    }

}
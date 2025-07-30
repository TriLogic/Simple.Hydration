using System.Data;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Legacy;
using Simple.Hydration;

namespace Simple.Hydration.Tests
{
    public class Tests
    {
        private DateTime Now = DateTime.Now;

        private static string KeyName_MealName = "MealName";
        private static string KeyName_MealTime = "MealTime";
        private static string KeyName_MealGuests = "Guests";

        private static string NameOfBreakfast = "Breakfast";
        private static string NameOfLunch = "Lunch";
        private static string NameOfSnacks = "Snacks";
        private static string NameOfDinner = "Dinner";

        private DateTime TimeForBreakfast;
        private DateTime TimeForLunch;
        private DateTime TimeForSnacks;
        private DateTime TimeForDinner;

        private static int GuestsAtBreakfast = 0;
        private static int GuestsAtLunch = 1;
        private static int GuestsAtSnacks = 5;
        private static int GuestsAtDinner = 2;

        private DataTable? Meals;

        private Dictionary<string, string?>? SingleMealDinner;

        private List<string> GuestsNotIncluded = new List<string>() { KeyName_MealName, KeyName_MealTime };
        private List<string> GuestsExcluded = new List<string>() { KeyName_MealGuests };

        private MealWithGuests TheMealBreakfast;
        private MealWithGuests TheMealLunch;
        private MealWithGuests TheMealSnacks;
        private MealWithGuests TheMealDinner;

        private List<MealWithGuests> MealBenchMarks;

        [OneTimeSetUp]
        public void Setup()
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

        [OneTimeTearDown]
        public void Cleanup()
        {
            Meals.Clear();
            Meals.Dispose();
            Meals = null;
        }

        [Test]
        public void HydrateSingle()
        {
            var meal = new Hydrator<MealWithGuests>()
                .Hydrate(key =>
                {
                    return SingleMealDinner[key];
                });

            Assert.That(meal, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(meal.MealName, Is.Not.Null);
                Assert.That(meal.MealName, Is.EqualTo(NameOfDinner));
            });

            Assert.Multiple(() =>
            {
                Assert.That(meal.TimeOfDay, Is.Not.Null);
                Assert.That(meal.TimeOfDay.ToString(), Is.EqualTo(TimeForDinner.ToString()));
            });

            Assert.Multiple(() =>
            {
                Assert.That(meal.GuestCount, Is.Not.Null);
                Assert.That(meal.GuestCount.ToString(), Is.EqualTo(GuestsAtDinner.ToString()));
            });
        }

        [Test]
        public void HydrateSingleGuestsNotIncludedByList()
        {
            var meal = new Hydrator<MealWithGuests>()
                .HydrateWith(GuestsNotIncluded, key =>
                {
                    return SingleMealDinner[key];
                });

            Assert.That(meal, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(meal.MealName, Is.Not.Null);
                Assert.That(meal.MealName, Is.EqualTo(NameOfDinner));
            });

            Assert.Multiple(() =>
            {
                Assert.That(meal.TimeOfDay, Is.Not.Null);
                Assert.That(meal.TimeOfDay.ToString(), Is.EqualTo(TimeForDinner.ToString()));
            });

            Assert.That(meal.GuestCount, Is.Null);
        }

        [Test]
        public void HydrateSingleGuestsExcludedByList()
        {
            var meal = new Hydrator<MealWithGuests>()
                .HydrateWithout(GuestsExcluded, key =>
                {
                    return SingleMealDinner[key];
                });

            Assert.That(meal, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(meal.MealName, Is.Not.Null);
                Assert.That(meal.MealName, Is.EqualTo(NameOfDinner));
            });

            Assert.Multiple(() =>
            {
                Assert.That(meal.TimeOfDay, Is.Not.Null);
                Assert.That(meal.TimeOfDay.ToString(), Is.EqualTo(TimeForDinner.ToString()));
            });

            Assert.That(meal.GuestCount, Is.Null);
        }


        [Test]
        public void HydrateSingleGuestsSkippedByFunction()
        {
            var meal = new Hydrator<MealWithGuests>()
                .Hydrate(key =>
                {
                    if (key == KeyName_MealGuests)
                        return (null, true);

                    return (SingleMealDinner[key], false);
                });

            Assert.That(meal, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(meal.MealName, Is.Not.Null);
                Assert.That(meal.MealName, Is.EqualTo(NameOfDinner));
            });

            Assert.Multiple(() =>
            {
                Assert.That(meal.TimeOfDay, Is.Not.Null);
                Assert.That(meal.TimeOfDay.ToString(), Is.EqualTo(TimeForDinner.ToString()));
            });

            Assert.That(meal.GuestCount, Is.Null);
        }


        [Test]
        public void HydrateMultipleAllFields()
        {
            var hydrator = new Hydrator<MealWithGuests>();
            Dictionary<string, int> OrdinalCache = new();

            var list = hydrator.Hydrate<DataRow>(Meals.AsEnumerable(), (row, key) =>
            {
                if (OrdinalCache.ContainsKey(key))
                    return row.ItemArray[OrdinalCache[key]].ToString();

                var ordinal = row.Table.Columns[key].Ordinal;
                OrdinalCache[key] = ordinal;
                return row.ItemArray[ordinal].ToString();
            });

            Assert.Multiple(() =>
            {
                Assert.That(list, Is.Not.Null);
                Assert.That(list.Count, Is.EqualTo(Meals.Rows.Count));
            });

            foreach (MealWithGuests meal in list)
            {
                var benchmark = MealBenchMarks
                    .Where(m => m.MealName == meal.MealName)
                    .FirstOrDefault();

                // assert the benchmark exists
                Assert.That(benchmark, Is.Not.Null);

                // assert the fields all match
                Assert.Multiple(() =>
                {
                    Assert.That(meal.MealName, Is.EqualTo(benchmark.MealName));
                    Assert.That(meal.TimeOfDay, Is.EqualTo(benchmark.TimeOfDay));
                    Assert.That(meal.GuestCount, Is.EqualTo(benchmark.GuestCount));
                });
            }
        }

        [Test]
        public void HydrateMultipleWithIncludeList()
        {
            var hydrator = new Hydrator<MealWithGuests>();
            Dictionary<string, int> OrdinalCache = new();

            var list = hydrator.HydrateWith<DataRow>(Meals.AsEnumerable(), GuestsNotIncluded, (row, key) =>
            {
                if (OrdinalCache.ContainsKey(key))
                    return row.ItemArray[OrdinalCache[key]].ToString();

                var ordinal = row.Table.Columns[key].Ordinal;
                OrdinalCache[key] = ordinal;
                return row.ItemArray[ordinal].ToString();
            });

            Assert.Multiple(() =>
            {
                Assert.That(list, Is.Not.Null);
                Assert.That(list.Count, Is.EqualTo(Meals.Rows.Count));
            });

            foreach (MealWithGuests meal in list)
            {
                var benchmark = MealBenchMarks
                    .Where(m => m.MealName == meal.MealName)
                    .FirstOrDefault();

                // assert the benchmark exists
                Assert.That(benchmark, Is.Not.Null);

                // assert the fields all match
                Assert.Multiple(() =>
                {
                    Assert.That(meal.MealName, Is.EqualTo(benchmark.MealName));
                    Assert.That(meal.TimeOfDay, Is.EqualTo(benchmark.TimeOfDay));
                    Assert.That(meal.GuestCount, Is.Null);
                });
            }
        }

        [Test]
        public void HydrateMultipleWithExcludeList()
        {
            var hydrator = new Hydrator<MealWithGuests>();
            Dictionary<string, int> OrdinalCache = new();

            var list = hydrator.HydrateWithout<DataRow>(Meals.AsEnumerable(), GuestsExcluded, (row, key) =>
            {
                if (OrdinalCache.ContainsKey(key))
                    return row.ItemArray[OrdinalCache[key]].ToString();

                var ordinal = row.Table.Columns[key].Ordinal;
                OrdinalCache[key] = ordinal;
                return row.ItemArray[ordinal].ToString();
            });

            Assert.Multiple(() =>
            {
                Assert.That(list, Is.Not.Null);
                Assert.That(list.Count, Is.EqualTo(Meals.Rows.Count));
            });

            foreach (MealWithGuests meal in list)
            {
                var benchmark = MealBenchMarks
                    .Where(m => m.MealName == meal.MealName)
                    .FirstOrDefault();

                // assert the benchmark exists
                Assert.That(benchmark, Is.Not.Null);

                // assert the fields all match
                Assert.Multiple(() =>
                {
                    Assert.That(meal.MealName, Is.EqualTo(benchmark.MealName));
                    Assert.That(meal.TimeOfDay, Is.EqualTo(benchmark.TimeOfDay));
                    Assert.That(meal.GuestCount, Is.Null);
                });
            }
        }


    }
}

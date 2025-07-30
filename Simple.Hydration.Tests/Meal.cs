using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Simple.Hydration;

namespace Simple.Hydration.Tests
{
    public class Meal
    {
        public string? MealName;
    }

    public class MealInfo : Meal
    {
        public DateTime? MealTime { get; set; }
    }

    public class MealTime : Meal
    {
        [HydrateWith("MealTime")]
        public DateTime? TimeOfDay { get; set; }
    }

    public class MealWithGuests : MealTime
    {
        [HydrateWith("Guests")]
        public int? GuestCount { get; set; }
    }
}

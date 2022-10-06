using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Hydration
{
    public interface IHydrator<T>
    {
        public T Hydrate(Func<string, string> lookup);
    }
}

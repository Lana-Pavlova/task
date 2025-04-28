using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task
{
    internal class Helper
    {
        private static PracticeContext instance;
        public static PracticeContext GetContext() => instance ??= new PracticeContext();
    }
}

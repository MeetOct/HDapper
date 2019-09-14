using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Test
{
    [TestClass]
    public class Test_linq_to_object
    {
        [TestMethod]
        public void Test()
        {
            var list = new List<int>() { 0, 1, 2, 3, 4, 5, 6 };
            var ss = list.Where(l =>
            {
                System.Diagnostics.Debug.WriteLine($"where1");
                Console.WriteLine("where1");
                return l > 1;
            });
            ss.ToList();
            foreach (var item in ss)
            {
                Console.WriteLine($"{item}");
            }
        }
    }
}

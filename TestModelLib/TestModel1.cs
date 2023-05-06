using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModelLib
{
    public class TestModel1
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public void GOGOGO(string source)
        {
            Console.WriteLine("driver程序： 得到 主程序的 Di中的TestModel1的 GOGOOG" + "source"+ source);
        }
    }
}

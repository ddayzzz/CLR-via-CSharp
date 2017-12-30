using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic
{
    class BoxAndUnbox
    {
        public static void Main()
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            List<int> glist = new List<int>();
            CodeTime.CodeTimer.Time("非泛型集合装箱", 1, () =>
            {
                for (int i = 0; i < 50000; ++i)
                {
                    list.Add(i);
                }
            });
            CodeTime.CodeTimer.Time("泛型集合", 1, () =>
            {
                for (int i = 0; i < 50000; ++i)
                {
                    glist.Add(i);
                }
            });
            CodeTime.CodeTimer.Time("非泛型的集合列表", 10000, () =>
            {

                foreach(var i in list)
                {
                    Int32 f = (Int32)i;

                }
            });
            CodeTime.CodeTimer.Time("泛型的集合列表", 10000, () =>
            {

                foreach (var i in glist)
                {
                    ;
                }
            });
        }
    }
}

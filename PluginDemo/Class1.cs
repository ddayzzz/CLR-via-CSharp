using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreFeature.ReflecionPluginDemo;
public sealed class AddIn_A : IAddIn
{
    public string DoSomething(object obj)
    {
        return "插件A的DoSomething";
    }
}
public sealed class AddIn_B : IAddIn
{
    public string DoSomething(object obj)
    {
        return "插件B的DoSomething";
    }
}
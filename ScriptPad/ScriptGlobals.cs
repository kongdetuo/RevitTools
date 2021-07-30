using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScriptPad
{
    public static class ScriptGlobals
    {
        public static object GlobalObject = new object();

        public static string StartScript;

        public static string templateScript;

        public static List<Assembly> InitAssemblies;
    }
}
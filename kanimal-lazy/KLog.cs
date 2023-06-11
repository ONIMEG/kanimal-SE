using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kanimal_lazy {
    internal class KLog {
        public static void Banner() {
            Console.WriteLine(@"
 _
| | __ _ _____   _
| |/ _` |_  / | | |
| | (_| |/ /| |_| |
|_|\__,_/___|\__, |
             |___/
 _               _                 _
| | ____ _ _ __ (_)_ __ ___   __ _| |
| |/ / _` | '_ \| | '_ ` _ \ / _` | |
|   < (_| | | | | | | | | | | (_| | |
|_|\_\__,_|_| |_|_|_| |_| |_|\__,_|_|

                  v1.0.0  MIT  @ttdly

");
        }

        public static void Success() {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"
+-------------------------------------------------------------+
|                                                             |
|   ██████ ██   ██  █████   █████   █████   ██████  ██████    |
|  ██      ██   ██ ██   ██ ██   ██ ██   ██ ██      ██         |
|   █████  ██   ██ ██      ██      ███████  █████   █████     |
|       ██ ██   ██ ██   ██ ██   ██ ██           ██      ██    |
|  ██████   ██████  █████   █████   ██████ ██████  ██████     |
|                                                             |
+-------------------------------------------------------------+
");

            Console.ResetColor();
        }

        public static void Info(string msg, bool add) {
            if (add) { };
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("[{0}] {1}", DateTime.Now, msg);
            Console.ResetColor();
        }
        public static void Error(string msg, bool add) {
            if (add) { };
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("[{0}] {1}", DateTime.Now, msg);
            Console.ResetColor();
        }
        public static void Warning(string msg, bool add) {
            if (add) { };
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[{0}] {1}", DateTime.Now, msg);
            Console.ResetColor();
        }
        public static void Wink(string msg, bool add) {
            if (add) { };
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("[{0}] {1}", DateTime.Now, msg);
            Console.ResetColor();
        }
        public static string ErrorLine(int code) {
            return $"[{DateTime.Now}]-[ErrorCode]:  {code}";
        }
    }
    
}

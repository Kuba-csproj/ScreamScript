using System.Text;

namespace ScreamScript;

class Program
{
    private static bool dev = false;
    static void Main(string[] args)
    {
        string code;
        
        if (dev)
            args = new[] { "./Test.scream" };

        if (args.Length == 0)
        {
            Console.WriteLine("Please enter a file path for the script to be interpreted.");
            return;
        }
        
        code = File.ReadAllText(args[0]);

        Interpreter i = new Interpreter(code);
    }
}
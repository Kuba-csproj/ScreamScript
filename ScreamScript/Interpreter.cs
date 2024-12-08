using System.Data;
using System.Text;

namespace ScreamScript;

public class Interpreter
{
    private Dictionary<string, object> stack = new Dictionary<string, object>();
    private Dictionary<string, int> labels = new Dictionary<string, int>();
    
    static char[] _alphabet = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ', '!','?', '>', ':', '\n'];

    private string _code;
    private string[] _lines;
    private int _activeLine;
    
    public Interpreter(string code)
    {
        _code = code;
        _activeLine = 0;
        _lines = _code.Split("!").Select((string a)=>a.Trim()).ToArray();

        for (int i=0; i<_lines.Length; i++)
        {
            if (_lines[i].StartsWith("GAH"))
            {
                if(_lines[i].Substring(3).Length <= 0)
                    throw new Exception("!!!AAAAAAAAAAA!!!!!!!!! GAH"); //Expected label name
                labels.Add(_lines[i].Substring(3), i);
            }
        }

        while (_activeLine < _lines.Length-1)
        {
            InterpretActiveLine();
        }
    }

    public void InterpretActiveLine()
    {
        if(_lines[_activeLine] == "AA")
            InterpretActiveLineVariableInit();
        else if (_lines[_activeLine].StartsWith("GAH"))
            _activeLine++;
        else if(_lines[_activeLine] == "AAGG")
            InterpretActiveLineCondition();
        else if(_lines[_activeLine] == "AAAA")
            InterpretActiveLineInput();
        else if(_lines[_activeLine] == "AAAAA")
            InterpretActiveLinePrintString();
        else if(_lines[_activeLine] == "AAaAA")
            InterpretActiveLinePrintNumber();
    }

    private void InterpretActiveLineCondition()
    {
        if (!_lines[_activeLine + 1].StartsWith("Aa"))
        {
            throw new Exception("!!!AAAAAAAAAAA!!!!!!!!! aaA"); //Expected condition start
        }

        int expressionLength = 0;
        bool closed = false;
        StringBuilder sb = new StringBuilder();

        for (int i = _activeLine+2; i < _lines.Length; i++)
        {
            if (_lines[i] == "aA")
            {
                closed = true;
                break;
            }

            expressionLength++;
            
            if(_lines[i].StartsWith("AaA"))
                sb.Append(_lines[i].Length-3);
            else if (_lines[i].StartsWith("AAAghA"))
                sb.Append(stack[_lines[i]].ToString());
            else
                sb.Append(_lines[i]);
        }

        if (!closed)
        {
            throw new Exception("!!!AAAAAAAAAAA!!!!!!!!! aAA"); //Expected end of expression
        }

        _activeLine += expressionLength+1;
        
        string expression = (sb.ToString());

        expression = expression.Replace("AGGH", "+").Replace("AGGh", "-").Replace("agGH", "*").Replace("AgHH", "/").Replace("AAGH", "==")
            .Replace("aaGH", "<").Replace("AAgh", ">").Replace("aAGh", "!=");

        if(Evaluate(expression, stack))
        {
            _activeLine = (labels[_lines[_activeLine + 2]]);
        }
        else
        {
            _activeLine += +3;
        }
    }
    
    static bool Evaluate(string expression, in Dictionary<string, object> variables) //TODO: Create real expression evaluator
    {
        foreach (KeyValuePair<string, object> variable in variables)
        {
            expression = expression.Replace(variable.Key, variable.Value.ToString());
        }
        

        var dataTable = new DataTable();
        return Convert.ToBoolean(dataTable.Compute(expression, null));
    }

    private void InterpretActiveLineInput()
    {
        if (!_lines[_activeLine + 1].StartsWith("AAAghA"))
        {
            throw new Exception("!!!AAAAAAAAAAA!!!!!!!!! aaa"); //Expected a variable 
        }
        
        int input = int.Parse(Console.ReadLine());
        
        if(stack.ContainsKey(_lines[_activeLine+1]))
            stack[_lines[_activeLine + 1]] = input;
        else
            stack.Add(_lines[_activeLine + 1], input);

        _activeLine += 2;
    }

    private void InterpretActiveLinePrintNumber()
    {
        if (!_lines[_activeLine + 1].StartsWith("AAAghA"))
        {
            throw new Exception("Proměnná musí mít název");
        }

        if (stack[_lines[_activeLine + 1]] is int)
        {
            Console.WriteLine(stack[_lines[_activeLine + 1]]);
            _activeLine += 2;
            return;
        }
        
        StringBuilder sb = new StringBuilder();
                
        foreach (int ch in (List<int>) stack[_lines[_activeLine+1]])
        {
            sb.Append(ch);
        }
                
        _activeLine += 2;
    }

    private void InterpretActiveLinePrintString()
    {
        if (!_lines[_activeLine + 1].StartsWith("AAAghA"))
        {
            throw new Exception("Proměnná musí mít název");
        }
        
        if (stack[_lines[_activeLine + 1]] is int)
        {
            Console.WriteLine(stack[_lines[_activeLine + 1]]);
            _activeLine += 2;
            return;
        }
                
        StringBuilder sb = new StringBuilder();
                
        foreach (int ch in (List<int>) stack[_lines[_activeLine+1]])
        {
            sb.Append((Math.Sign(ch)>=0)?_alphabet[ch]:char.ToLower(_alphabet[-ch]));
        }
                
        Console.WriteLine(sb.ToString());

        _activeLine += 2;
    }

    private void InterpretActiveLineVariableInit()
    {
        if (!_lines[_activeLine + 1].StartsWith("AAAghA"))
        {
            throw new Exception("Proměnná musí mít název");
        }

        if (_lines[_activeLine + 2] == "AaAaaaggghhh")
        {
            Random r = new Random();
            if (stack.ContainsKey(_lines[_activeLine + 1]))
                stack[_lines[_activeLine + 1]] = r.Next(100);
            else
                stack.Add(_lines[_activeLine + 1], r.Next(100));
            
            _activeLine += 3;
            return;
        }

        List<int> numbers = new List<int>();
        int l = _activeLine + 2;
        while (_lines[l].StartsWith("Aaa") || _lines[l].StartsWith("AaA"))
        {
            string literal = _lines[l].Substring(3);
            numbers.Add(_lines[l].StartsWith("Aaa")?-literal.Length:literal.Length);
            l++;
        }
                
        if (stack.ContainsKey(_lines[_activeLine + 1]))
            stack[_lines[_activeLine + 1]] = numbers;
        else
            stack.Add(_lines[_activeLine + 1], numbers);
        
        _activeLine += 2 + numbers.Count;
    }
}
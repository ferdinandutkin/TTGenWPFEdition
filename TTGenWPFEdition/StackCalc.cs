using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TTGenWPFEdition
{
    class ExprResult
    {


        public bool Result { get; set; }
        public string Expression { get; set; }

        public ExprResult(string variable, bool value) => (Expression, Result) = (variable, value);

        public ExprResult(bool value) : this(value.ToString(), value)
        {

        }


        static public ExprResult And(ExprResult l, ExprResult r) =>
            new ExprResult(l.Expression + " ∧ " + r.Expression, l.Result && r.Result);

        static public ExprResult Or(ExprResult l, ExprResult r) =>
            new ExprResult(l.Expression + " ∨ " + r.Expression, l.Result || r.Result);

        static public ExprResult Implication(ExprResult l, ExprResult r) =>
            new ExprResult(l.Expression + "  ⇒ " + r.Expression, !l.Result || r.Result);

        static public ExprResult Equivalence(ExprResult l, ExprResult r) =>
            new ExprResult(l.Expression + " ⇔ " + r.Expression, l.Result == r.Result);


        static public ExprResult Xor(ExprResult l, ExprResult r) =>
            new ExprResult(l.Expression + " ⊕ " + r.Expression, l.Result != r.Result);

        static public ExprResult Not(ExprResult res) =>
            new ExprResult("¬" + res.Expression, !res.Result);


    }


    class ExpressionStack
    {
        public Action<ExprResult> OnEval = (ExprResult _) => { };



        public static Dictionary<string, int> Priority = new Dictionary<string, int> { { "!", 5 }, { "&", 4 }, { "|", 3 }, { "^", 3 }, { ">", 2 }, { "=", 1 } };


        public static Dictionary<string, Func<ExprResult, ExprResult, ExprResult>> OpDictionary =
            new Dictionary<string, Func<ExprResult, ExprResult, ExprResult>>
            {
                { "&", ExprResult.And },
                { "|", ExprResult.Or },
                { "^", ExprResult.Xor },
                { ">", ExprResult.Implication },
                { "=", ExprResult.Equivalence }

            };
        Dictionary<string, bool> variableValues;

        public ExpressionStack(Dictionary<string, bool> variableValues) => this.variableValues = variableValues;

        public ExpressionStack() => variableValues = new Dictionary<string, bool>();

        public static readonly Dictionary<string, bool> toBool = new Dictionary<string, bool>{{"True", true}, {"False", false}, { "true", true}, { "false", false}, { "1", true}, { "0", false}};

        Stack<ExprResult> operands = new Stack<ExprResult>();
        Stack<string> operators = new Stack<string>();

        void EvalTop()
        {


            if (operators.Peek() == "(")
            {
                operators.Pop();
                return;
            }

            if (operators.Peek() == "!")
            {
                ExprResult operand = operands.Pop();
                ExprResult result = ExprResult.Not(operand);


                OnEval(result);

                operands.Push(result);
            }
            else
            {

                ExprResult right = operands.Pop();


                ExprResult left = operands.Pop();


                ExprResult result = OpDictionary[operators.Peek()](left, right);

                OnEval(result);

                operands.Push(result);
            }

            operators.Pop();

        }

        public ExprResult ForceEval()
        {
            while (!(operators.Count == 0))
            {
                EvalTop();
            }
            return operands.Peek();
        }


        public void Push(string token)
        {
            if (variableValues.TryGetValue(token, out var value))
            {

                if (operators.Count != 0 && operators.Peek() == "(")
                {
                    token = "( " + token;
                }
                operands.Push(new ExprResult(token, value));

            }
            else if (toBool.TryGetValue(token, out var val))
            {
                operands.Push(new ExprResult(val));
            }

            else if (Priority.ContainsKey(token))
            {
                if (operators.Count == 0 || operators.Peek() == "(" || Priority[token] > Priority[operators.Peek()])
                {
                    operators.Push(token);
                }
                else if (token == "!" && token == operators.Peek() && operators.Count > operands.Count)
                {
                    operators.Pop();
                }
                else
                {
                    EvalTop();
                    Push(token);
                }
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                operands.Peek().Expression += " )";

                while (operators.Peek() != "(")
                {
                    EvalTop();
                }
                operators.Pop();
            }



            
        }

    }


    class TruthTableGen
    {

        List<string> variables = new List<string>();
        List<string> expression = new List<string>();


        string ProcessExpression(string expression)
        {

            Dictionary<char, char> replaceDictionary = new Dictionary<char, char>{
                { '∨', '|' }, { '∧', '&' },
        { '⇒', '>'}, { '⊕',  '^'}, { '⇔', '='}, { '¬', '!'}

            };

            foreach (var (key, value) in replaceDictionary.Select(x => (x.Key, x.Value))) //КТО СЛОМАЛ ДЕКОНСТРУКЦИЮ КОРТЕЖЕЙЙ
            {
                expression = expression.Replace(key, value);
            }




            return expression.Replace("true", "1").Replace("false", "0").Replace("True", "1").Replace("False", "0");
        }

        public TruthTableGen(string expression)
        {
            foreach (char ctoken in ProcessExpression(expression))
            {
                string stoken = char.ToString(ctoken);

                if (stoken != " ")
                {
                    if (ExpressionStack.Priority.ContainsKey(stoken) || ExpressionStack.toBool.TryGetValue(stoken, out bool _) || stoken == ")" || stoken == "(")
                    {
                        this.expression.Add(stoken);
                    }
                    else
                    {
                        this.expression.Add(stoken);
                        if (!variables.Contains(stoken))
                        {
                            variables.Add(stoken);
                        }

                    }
                }


            }
        }
        public List<List<string>> Generate()
        {

            if (variables.Count == 0)
            {
                ExpressionStack stack = new ExpressionStack();
                foreach (var token in expression)
                {
                    stack.Push(token);
                }
                return new List<List<string>> { new List<string>() { stack.ForceEval().Result.ToString() } };
            }
            List<List<string>> ret = new List<List<string>>();
            //


            ret.Add(variables.Select(var => var).Prepend("№").ToList()); //DEEP COPY

           

            for (int i = 0; i < 2 << (variables.Count - 1); i++)
            {
               
                Dictionary<string, bool> varibaleValues = new Dictionary<string, bool>();

                List<string> row = new List<string> { i.ToString()};

                foreach (var (variable, j) in variables.Select((variable, j) => (variable, j)))
                {
                    varibaleValues[variable] = Convert.ToBoolean((i >> (variables.Count - 1 - j)) & 1);

                    row.Add(varibaleValues[variable].ToString());
 
                }

              


                ExpressionStack stack = new ExpressionStack(varibaleValues);

                stack.OnEval = (ExprResult result) =>
                {
                    if (i == 0)
                    {
                        ret[0].Add(result.Expression);
                    }

                    row.Add(result.Result.ToString());

                };

                foreach (var token in expression)
                {
                    stack.Push(token);

                }
                stack.ForceEval();
                ret.Add(row);



            }

            return ret;

        }
    }
}

                
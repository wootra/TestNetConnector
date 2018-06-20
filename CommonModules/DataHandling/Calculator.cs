using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHandling.Calculate
{
    /* Sample how to use
   public partial class Form1 : Form
    {
        VariableStack _vs;
        Calculator _calc;
        BindingSource _bs;
        public Form1()
        {
            InitializeComponent();

            _vs = new VariableStack();
            _calc = new Calculator(_vs);
            
            _vs.add("testVal",1,typeof(int));
            _bs = new BindingSource();

            List<String> nameList = _vs.nameList;
            foreach(String name in nameList){
                _bs.Add(new VarNameData(name, _vs.getValue(name)));
            }
            DVar.DataSource = _bs; //DVar is a DataView Component
            DVar.EditMode = DataGridViewEditMode.EditOnEnter;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            String text = TRequest.Text;
            String answer;
           // try
           // {
                answer = _calc.calculate(text);
                TAnswer.Text = answer;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

        }

    }
    public class VarNameData
    {
        public String name { get; set; }
        public String value { get; set; }

        public VarNameData(String name, String value)
        {
            this.name = name;
            this.value = value;
        }
    }     
     */
    public class VariableStack
    {

        public Stack<VariableStore> stack;
        public List<String> nameList { get { return getTopStack().nameList; } }

        public VariableStore topLevel { get { return getTopStack(); } }

        public VariableStack()
        {
            stack = new Stack<VariableStore>();
            addALevel(); //기본으로 한개의 레벨은 가지고 있음.
        }
        public void addALevel()
        {
            stack.Push(new VariableStore());
        }
        public int clearALevel()
        {
            if (stack.Count > 1) stack.Pop(); //top level은 삭제하지 않는다.
            return stack.Count - 1;
        }
        public void add(String name, int value, Type type)
        {
            getTopStack().add(name, value, type);
        }

        public void add(String name, String value)
        {
            getTopStack().add(name, value);
        }

        public int getLowestLevel()
        {
            return stack.Count - 1;
        }
        public VariableStore getTopStack()
        {
            return stack.ElementAt<VariableStore>(0);
        }
        public VariableStore getStackInLevel(int level)
        {
            return stack.ElementAt<VariableStore>(level);
        }

        public VariableStore.ValueType getValueType(String name)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                VariableStore.ValueType type = getStackInLevel(i).getValueType(name);
                if (type != VariableStore.ValueType.NoExist) return type;
            }
            return VariableStore.ValueType.NoExist;
        }

        public String getValueStr(String name)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                VariableStore.ValueType type = getStackInLevel(i).getValueType(name);
                if (type == VariableStore.ValueType.String) return getStackInLevel(i).getValueStr(name);
            }
            throw new Exception(name + " : There's no such a string variable that has the name in the stack..");
        }

        public int getValueInt(String name)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                VariableStore.ValueType type = getStackInLevel(i).getValueType(name);
                if (type == VariableStore.ValueType.Number) return getStackInLevel(i).getValueInt(name);
            }
            throw new Exception(name + " : There's no such a number variable that has the name in the stack..");
        }

        public String getValue(String name)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                VariableStore.ValueType type = getStackInLevel(i).getValueType(name);
                if (type == VariableStore.ValueType.Number) return "" + getStackInLevel(i).getValueInt(name);
                else if (type == VariableStore.ValueType.String) return "" + getStackInLevel(i).getValueInt(name);
            }
            throw new Exception(name + " : There's no such a variable that has the name in the stack..");
        }


    }
    public class VariableStore
    {
        public enum ValueType { String, Number, NoExist };

        public Dictionary<String, int> intValue;
        public Dictionary<String, String> strValue;
        public Dictionary<String, Type> valueType;
        public List<String> nameList;

        public VariableStore()
        {
            intValue = new Dictionary<string, int>();
            strValue = new Dictionary<string, string>();
            valueType = new Dictionary<string, Type>();
            nameList = new List<string>();
        }
        public Boolean add(String name, int value, Type type)
        {
            if (getValueType(name) == ValueType.NoExist)
            {
                nameList.Add(name);
                intValue.Add(name, value);
                valueType.Add(name, type);
                return true;
            }
            else
            {
                return false;
            }
        }
        public Boolean add(String name, String value)
        {
            if (getValueType(name) == ValueType.NoExist)
            {

                nameList.Add(name);
                strValue.Add(name, value);
                valueType.Add(name, typeof(String));
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool remove(String name)
        {
            if (getValueType(name) == ValueType.Number)
            {
                intValue.Remove(name);
                valueType.Remove(name);
                nameList.Remove(name);

                return true;
            }
            else if (getValueType(name) == ValueType.String)
            {
                strValue.Remove(name);
                valueType.Remove(name);
                nameList.Remove(name);

                return true;
            }
            else //no exist
            {
                return false;
            }
        }
        public ValueType getValueType(String name)
        {
            int value;
            String valueStr;
            if (intValue.TryGetValue(name, out value))
            {
                return ValueType.Number;
            }
            else if (strValue.TryGetValue(name, out valueStr))
            {
                return ValueType.String;
            }
            else return ValueType.NoExist;
        }
        public String getValueStr(String name)
        {
            String value;
            if (strValue.TryGetValue(name, out value)) return value;
            else throw new Exception("그런 이름의 값이 스택에 존재하지 않습니다.");
        }

        public int getValueInt(String name)
        {
            int value;
            if (intValue.TryGetValue(name, out value)) return value;
            else throw new Exception("그런 이름의 값이 스택에 존재하지 않습니다.");
        }
    }
    public class Operators
    {

        private List<String[]> opLevel;

        public Operators()
        {
            opLevel = new List<String[]>();
            opLevel.Add(new String[] { "(", ")", "\n" });
            opLevel.Add(new String[] { "+", "-" });
            opLevel.Add(new String[] { "*", "/" });
            opLevel.Add(new String[] { "^" });
        }
        public int getLevel(String op)
        {
            for (int i = opLevel.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < opLevel[i].Length; j++)
                {
                    if (opLevel[i][j].Equals(op)) return i;
                }
            }
            return -1;
        }
        public Boolean isOp(String op)
        {
            if (getLevel(op) < 0) return false;
            else return true;
        }
        public String getAllOperators()
        {
            String ops = "";
            foreach (String[] strs in opLevel)
            {
                foreach (string str in strs)
                {
                    ops += str;
                }
            }
            return ops;
        }
    }
    public class Calculator
    {
        private Operators _opList;
        public Stack<String> _op;
        public Stack<String> _unit;
        private int _bracketOpen = 0;
        private int _topLevel = 0;
        private int _level = 0;

        private enum UnitType { Number, Name, None, String, Op };
        private VariableStack valueStack;

        public Calculator(VariableStack vs)
        {
            _opList = new Operators();
            _op = new Stack<string>();
            _unit = new Stack<string>();

            valueStack = vs;
        }

        public String calculate(String syntex)
        {
            if (syntex == null || syntex.Length == 0) return "";
            String tempNum = "";
            String tempName = "";
            String tempOp = "";
            String tempString = "";
            syntex = syntex.Trim();
            UnitType nowEditing = UnitType.None;
            //if (syntex.LastIndexOf('\n') != 0) syntex += '\n'; //맨 뒤에 줄바꿈 표시가 없으면 추가해 준다. 아래의 parsing에서 parsing을 끝내는 역할을 한다.
            syntex = "(" + syntex + ")";

            String editingsyntex = "";
            foreach (Char a in syntex)
            {
                editingsyntex += a;
                if (nowEditing != UnitType.String && a.Equals('\n') == false && Char.IsWhiteSpace(a)) //문자입력중이 아니라면 공백은 무시
                {
                    continue;
                }


                #region 현재 편집중인 유닛이 취소되는 조건.
                if (nowEditing == UnitType.Name && Char.IsLetterOrDigit(a) == false)
                {
                    VariableStore.ValueType type = valueStack.getValueType(tempName);
                    if (type != VariableStore.ValueType.NoExist)
                    {
                        _unit.Push(valueStack.getValue(tempName));
                        tempName = "";
                        nowEditing = UnitType.None;
                    }
                    else
                    {
                        throw new Exception(tempName + ": there's no such a variable's name in the variable stack." + "\n" + editingsyntex + "^");
                    }
                }
                else if (nowEditing == UnitType.Number && Char.IsDigit(a) == false && a.Equals('.') == false)
                {
                    _unit.Push(tempNum);
                    tempNum = "";
                    nowEditing = UnitType.None;
                }
                else if (nowEditing == UnitType.String && a.Equals('\"'))
                {
                    int countBS = 0;
                    for (int i = tempString.Length - 1; i >= 0; i--)
                    {
                        if (tempString[i].Equals('\\')) countBS++;
                        else break; //처음으로 \가 아닌 것이 나왔을 때 룹을 나간다.
                    }
                    if (countBS % 2 == 0)//과거에 \가 짝수로 나왔다면 마지막 나온 따옴표와는 관계가 없다. 예을 들어, \\"는 {\,"}이므로, 따옴표가 닫히는 구문이다.
                    {
                        _unit.Push("\"" + tempString + "\"");//trim으로 처리하기 위해서 처음과 끝에 스페이스를 붙인다. 스페이스를 붙이는 이유는 숫자로 parsing되지 않게 하기 위해서이다.
                        tempString = "";
                        nowEditing = UnitType.None;
                        continue;
                    }
                }
                else if (nowEditing == UnitType.Op && (Char.IsLetterOrDigit(a) || a.Equals('.') || a.Equals('\"') || "_$#@".IndexOf(a) >= 0))
                {
                    if (_opList.isOp(tempOp))
                    {
                        _op.Push(tempOp);
                        //checkStack(); //op가 끝나고 나면, 우선순위에 따라서 계산을 하면서 진행함..
                        tempOp = "";
                        nowEditing = UnitType.None;
                    }
                    else
                    {
                        throw new Exception(tempOp + " : there's no such a operator in the operator stack." + "\n" + editingsyntex + "^");
                    }
                }
                if (a.Equals('\n'))
                {
                    _op.Push("\n");
                    checkStack();
                    break;
                }//한줄이 끝나면 구문을 나가고 후처리를 한다.
                #endregion


                if (nowEditing == UnitType.None) //다른 unit을 편집중이 아닐 때..
                {
                    if (a.Equals('\"'))
                    {
                        nowEditing = UnitType.String;
                        tempString = "";
                        continue;
                    }

                    if (a.Equals('.')) //.으로 시작하면 소수점 앞의 0을 생략한 것으로 본다.
                    {
                        nowEditing = UnitType.Number;
                        tempNum += "0" + a;
                        continue;
                    }

                    if (Char.IsDigit(a))
                    { //숫자로 시작한다면 숫자이다.
                        nowEditing = UnitType.Number;
                        tempNum += a;
                        continue;
                    }

                    if (Char.IsLetter(a) || "_$#@".IndexOf(a) >= 0)
                    { //문자로 시작하거나 _$#@로 시작한다면 변수명이다.
                        nowEditing = UnitType.Name;
                        tempName += a;
                        continue;
                    }

                    if (a.Equals(')'))
                    { //괄호를 닫으면 그 괄호까지의 작업을 처리해 준다.
                        _op.Push("" + a);
                        tempOp = "";
                        checkStack();
                        continue;
                    }

                    if (_opList.getAllOperators().IndexOf(a) >= 0)
                    { //op에서 사용되는 문자일 때는 operator라고 본다.
                        nowEditing = UnitType.Op;
                        tempOp += a;
                        continue;
                    }
                    throw new Exception(a + " : this character cannot be used in this program." + "\n" + editingsyntex + "^");
                }


                if (nowEditing == UnitType.String)
                {
                    tempString += a;
                    continue;
                }

                if (nowEditing == UnitType.Name)
                {
                    tempName += a;
                    continue;
                }

                if (nowEditing == UnitType.Number)
                {
                    tempNum += a;
                    continue;
                }

                if (nowEditing == UnitType.Op)
                {
                    if (tempOp.Equals("(")) //다른 op가 들어왔는데 이전에 (였다면, 부호를 넣은 것이다. +와 -, (만 허용해야 함.
                    {
                        if (a.Equals('+') || a.Equals('-'))
                        {
                            _unit.Push("0"); //-1은 0-1과 같다.
                            _op.Push(tempOp); //앞의 것은 스택에 넣고
                            tempOp = "" + a; //다음것만 유지한다.
                            continue;
                        }
                        else if (a.Equals('('))
                        {
                            _op.Push(tempOp); //앞의 것은 스택에 넣고
                            tempOp = "" + a; //다음것만 유지한다.
                            continue;
                        }
                        else
                        {
                            throw new Exception("you cannot use the syntex like (" + a + "number) ... you tried : " + editingsyntex + "^");
                        }
                    }
                    else if (a.Equals('(')) //어떠한 부호가 있은 다음 괄호가 나오는 경우.
                    {
                        _op.Push(tempOp); //앞의 것은 스택에 넣고
                        tempOp = "" + a; //다음것만 유지한다.
                        continue;
                    }
                    else
                    {
                        tempOp += a;
                        if (_opList.isOp(tempOp))
                        {
                            _op.Push(tempOp);
                            tempOp = "";
                            nowEditing = UnitType.None;
                        }
                        else
                        {
                            throw new Exception("you cannot use the operator like " + tempOp + " in this program. : " + editingsyntex + "^");
                        }
                    }
                    continue;
                }
            }

            //checkStack();
            return _unit.Pop();
        }

        private void checkStack()
        {
            String op = _op.Pop();

            Stack<String> values = new Stack<string>();
            Stack<String> ops = new Stack<string>();

            if (op.Equals(")") || op.Equals("\n")) //마지막 들어온 op가 )혹은 \n라면, 
            {
                if (op.Equals(")"))
                {
                    _bracketOpen--; // 이전 (가 나올때까지 끊어서 먼저 계산하여 다시 스택에 넣어야 한다. 혹은 \n라면 맨처음까지 스택에 넣어야 한다.
                }
                //op = _op.Pop(); //하나 더 빼서
                //values.Push(_unit.Pop());
                while (op.Equals("(") == false && _op.Count > 0) //(가 아니라면 루프를 돌면서 값과 쌍으로 빼 준다. 여기서 값은 op보다 하나 더 있어야 한다.
                { // (가 나오면 더이상 넣지 않고 루프 끝냄..
                    values.Push(_unit.Pop()); //value가 op보다 하나 더 빠져야 한다.
                    op = _op.Pop();
                    _level = _opList.getLevel(op);
                    _topLevel = (_topLevel < _level) ? _level : _topLevel;
                    if (op.Equals("(") == false) ops.Push(op);

                }
                int level = _topLevel;
                List<String> valList = values.ToList();// 뒤에서부터 빼서 리스트에 넣는다. 이렇게 함으로서, 처음에 넣었던 순서대로 회복되었다.
                List<String> opList = ops.ToList();

                while (level > 0)
                {
                    for (int i = 0; i < opList.Count; i++)
                    {
                        if (_opList.getLevel(opList[i]) == level)
                        {
                            valList[i] = operation(opList[i], valList[i], valList[i + 1]);
                            valList.RemoveAt(i + 1);
                            opList.RemoveAt(i);
                            i--;
                        }
                    }
                    level--;
                }
                _unit.Push(valList[0]);//마지막에 남은 값을 다시 넣음.
            }
            else
            {
                _op.Push(op);
                return;
            }

        }

        private string operation(string op, string val1, string val2)
        {
            int int1 = 0, int2 = 0;
            float float1 = 0, float2 = 0;
            String type1, type2;


            if (Int32.TryParse(val1, out int1))
            {
                type1 = "int";
            }
            else if (float.TryParse(val1, out float1))
            {
                type1 = "float";
            }
            else if (val1.Length > 0)
            {
                type1 = "string";
            }
            else
            {
                throw new Exception("입력이 없습니다. 프로그램 에러.: val1 : " + val1);
            }

            if (Int32.TryParse(val2, out int2))
            {
                type2 = "int";
            }
            else if (float.TryParse(val2, out float2))
            {
                type2 = "float";
            }
            else if (val2.Length > 0)
            {
                type2 = "string";
            }
            else
            {
                throw new Exception("입력이 없습니다. 프로그램 에러.: val2 : " + val2);
            }


            switch (op)
            {
                case "+":
                    if (type1.Equals("string") || type2.Equals("string")) return "\"" + val1.Replace("\"", "") + val2.Replace("\"", "") + "\"";
                    else if (type1.Equals("float") || type2.Equals("float")) return (float.Parse(val1) + float.Parse(val2)).ToString();
                    else return (int.Parse(val1) + int.Parse(val2)).ToString();
                case "-":
                    if (type1.Equals("string") || type2.Equals("string")) return val1.Replace(val2.Replace("\"", ""), "");
                    else if (type1.Equals("float") || type2.Equals("float")) return (float.Parse(val1) - float.Parse(val2)).ToString();
                    else return (int.Parse(val1) - int.Parse(val2)).ToString();
                case "*":
                    if (type1.Equals("string") && type2.Equals("int")) //문자 뒤에 숫자로 곱셈을 했다면 문자를 반복한다.
                    {
                        int size = int.Parse(val2);
                        if (size == 0) return "  ";
                        val1 = val1.Replace("\"", "");
                        String temp = "";

                        for (int i = 0; i < int.Parse(val2); i++) temp += val1;
                        return "\"" + temp + "\"";
                    }
                    if (type1.Equals("string") || type2.Equals("string")) throw new Exception("* operator cannot be used between strings. you tried " + val1 + "*" + val2);
                    else if (type1.Equals("float") || type2.Equals("float")) return (float.Parse(val1) * float.Parse(val2)).ToString();
                    else return (int.Parse(val1) * int.Parse(val2)).ToString();
                case "/":
                    if (type1.Equals("string") && type2.Equals("int")) //문자 뒤에 숫자로 나눗셈을 했다면 문자를 정해진 크기로 나눈다.
                    {
                        int unitSize = int.Parse(val2);
                        if (unitSize == 0) throw new Exception("you tried to divide with zero. you tried " + val1 + "/" + val2);

                        if (val1.Length - 2 < unitSize) return val1;
                        val1 = val1.Replace("\"", "");

                        int size = val1.Length / unitSize; //나머지는 없는 숫자로 나뉜다. 예를 들어, 5를 2로 나누면 2가 나오므로, 1개의 문자가 남게 된다.

                        String newStr = "";
                        for (int i = 0; i < size; i++) newStr += val1.Substring(i * unitSize, unitSize) + ","; //,로 나눈다.
                        if (val1.Length % unitSize > 0) newStr += val1.Substring((size) * unitSize); //남은 것이 있다면, 마지막에는 남은 스트링을 모두 넣는다.
                        else val1.Substring(0, val1.Length - 1); //맨 마지막 ,를 없앤다.
                        return "\"" + val1 + "\"";
                    }
                    if (type1.Equals("string") || type2.Equals("string")) throw new Exception("/ operator cannot be used between strings. you tried " + val1 + "/" + val2);
                    else if (type1.Equals("float") || type2.Equals("float")) return (float.Parse(val1) / float.Parse(val2)).ToString();
                    else return (int.Parse(val1) / int.Parse(val2)).ToString();

                case "^":
                    if (type1.Equals("string") || type2.Equals("string")) throw new Exception("^ operator cannot be used between strings. you tried " + val1 + "^" + val2);
                    else if (type1.Equals("float"))
                    {
                        if (type2.Equals("int"))
                        {

                            if (int2 == 0) return "1";

                            if (float1 < 0) throw new Exception("it is not possible to calculate this syntex in this program. arg1 is less than 0. you tried " + val1 + "^" + val2);
                            float newValue = float1;

                            bool val2Minus = (int2 < 0);
                            if (val2Minus) int2 *= -1; //일단 +로 바꾸고 계산한다.
                            for (int i = 0; i < int2; i++)
                            {
                                newValue *= float1;
                            }
                            if (val2Minus) newValue = 1 / newValue; //지수법칙에서 양의 정수의 음제곱수은 1/양의정수의 양제곱수 이다.
                            return newValue.ToString();
                        }
                        else //type2 is float
                        {
                            if (float2 == 0) return "1.0";

                            if (float1 < 0) throw new Exception("it is not possible to calculate this syntex in this program. arg1 is less than 0. you tried " + val1 + "^" + val2);
                            float newValue = float1;
                            int2 = (int)(1.0 / float2);

                            bool val2Minus = (int2 < 0);
                            if (val2Minus) int2 *= -1; //일단 +로 바꾸고 계산한다.

                            if (int2 != 2) throw new Exception("if you want calculate sqrt, use 0.5. other float for arg2 is not possible. ex> 16.0^0.5 -> 4.0 . you tried " + val1 + "^" + val2);

                            newValue = (float)Math.Sqrt((double)float1);
                            if (val2Minus) newValue = 1 / newValue; //지수법칙에서 양의 정수의 음제곱수은 1/양의정수의 양제곱수 이다.
                            return newValue.ToString();
                        }

                    }
                    else //type1 is int
                    {
                        if (int1 < 0) throw new Exception("it is not possible to calculate this syntex in this program. arg1 is less than 0. you tried " + val1 + "^" + val2);
                        if (type2.Equals("int"))
                        {

                            if (int2 == 0) return "1";


                            float newValue = 1;

                            bool val2Minus = (int2 < 0);
                            if (val2Minus) int2 *= -1; //일단 +로 바꾸고 계산한다.
                            for (int i = 0; i < int2; i++)
                            {
                                newValue *= int1;
                            }
                            if (val2Minus)
                            {
                                newValue = 1 / newValue; //지수법칙에서 양의 정수의 음제곱수은 1/양의정수의 양제곱수 이다.

                                return newValue.ToString();
                            }
                            else
                            {
                                return ((int)newValue).ToString();
                            }
                        }
                        else //type2 is float
                        {
                            if (float2 == 0) return "1.0";

                            float newValue = (float)int1;
                            int2 = (int)(1.0 / float2);

                            bool val2Minus = (int2 < 0);
                            if (val2Minus) int2 *= -1; //일단 +로 바꾸고 계산한다.

                            if (int2 != 2) throw new Exception("if you want calculate sqrt, use 0.5. other float for arg2 is not possible. ex> 16.0^0.5 -> 4.0 . you tried " + val1 + "^" + val2);

                            newValue = (float)Math.Sqrt((double)int1);
                            if (val2Minus) newValue = 1 / newValue; //지수법칙에서 양의 정수의 음제곱수은 1/양의정수의 양제곱수 이다.
                            return newValue.ToString();
                        }

                    }
                default:
                    throw new Exception("there's no such a operator as " + op);
            }

        }

    }
    public class TypeDefs
    {
        public String[] name;
        public int[] type;
        public TypeDefs()
        {
            name = new String[] { "long", "int", "short", "byte", "_8", "_4", "_2", "_1", "string", "_s" };
            type = new int[] { 8, 4, 2, 1, 8, 4, 2, 1, -1, -1 };
        }
        public Boolean isTypeDef(String unit)
        {
            String[] strs = unit.Split("()".ToCharArray());
            if (strs != null && strs.Length > 0)
            {
                foreach (String s in name)
                {
                    if (s.Equals(strs[0]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public int getByteSize(String unit)
        {
            String[] strs = unit.Split("()".ToCharArray());
            if (strs != null && strs.Length > 0)
            {
                for (int i = 0; i < name.Length; i++)
                {
                    if (name[i].Equals(strs[0]))
                    {
                        if (type[i] < 0)
                        {
                            if (strs.Length < 2) throw new Exception("잘못된 식:( " + unit + " )\nstring의 정의는 크기를 동반해야 합니다. 형식: string(크기) 또는 _s(크기)");
                            else
                            {
                                int size;
                                Int32.TryParse(strs[1], out size);
                                return size;
                            }
                        }
                        else
                        {
                            return type[i];
                        }
                    }
                }

                throw new Exception("잘못된 식: ( " + unit + " ) \n 타입명이 아닙니다.");

            }
            throw new Exception("비어있는 식입니다.");
        }


    }
}

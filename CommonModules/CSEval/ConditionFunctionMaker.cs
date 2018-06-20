using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;

namespace CSEval
{
    public class ConditionFunctionMaker
    {
        static TypeParser parser = new TypeParser(Assembly.GetExecutingAssembly(), new List<string>()
                {
                    "System" ,
                    "System.Collections.Generic" ,
                    "System.Linq" ,
                    "System.Text" ,
                    "System.Windows" ,
                    "System.Windows.Shapes" ,
                    "System.Windows.Controls" ,
                    "System.Windows.Media" ,
                    "System.IO" ,
                    "System.Reflection" ,
                    "CSEval"
                }
                );

        
        public static Func<int, bool> GetIntConditionFunction(string condition, string argName="")
        {
            Func<int, bool> condFunc;

             string str;
            int outValue;

            if (condition == null || condition.Trim().Length == 0)
            {
                condFunc = new Func<int, bool>((x) =>
                {
                    return true;//무조건 pass
                });//기본함수..
            }
            else if (int.TryParse(condition, out outValue))
            {
                //simple function..
                condFunc = new Func<int, bool>((x) =>
                {
                    if (x == outValue) return true;
                    else return false;
                });//기본함수..
            }
            else 
            {



                        
                TypeParser.DefaultParser = parser;
                //이때는 형식을 x: x==1 과 같은 식으로 썼을 때이다. 아니면 x==1과 같이 쓰면 된다.
                // x: x==1 에서 x값을 바꾸면 cond: cond==1 이라고 쓸 수 있다.
                //string[] tokens = condition.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string name;
                string cond;
                if (argName.Equals(""))
                {
                    name = "x";
                    int val;
                    if (int.TryParse(condition, out val))
                    {
                        cond = "x==" + condition;
                    }
                    else
                    {
                        cond = condition;
                    }
                }
                else
                {
                    name = argName;
                    cond = condition;
                }
                if (cond.Length == 0) cond = "true";
                str = @"
                bool Test (int "+name+@") 
                { 
                    return "+ cond + @";
                }";
                LexList lexList = LexListGet(str);
                condFunc = MakeMethod<Func<int, bool>>.Compile(parser, lexList);
                        
            }

            return condFunc;
                    
        }

        static LexList LexListGet(string s)
        {
            LexListBuilder lb = new LexListBuilder();
            lb.Add(s);
            return lb.ToLexList();
        }

    }
        
}

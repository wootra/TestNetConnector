using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomParser
{
    public class FunctionInfo
    {
        String[] _args;
        String _name;
        //bool _exists = false;
        funcInfoFunc Func=null;
        public FunctionInfo(String name, funcInfoFunc func, params String[] args)
        {
            this._name = name;
            this._args = args;
            //this._exists = true;
            this.Func = func;
        }

        public FunctionInfo(FunctionInfo templateFuncToClone, params String[] args)
        {
            this._name = templateFuncToClone.Name;
            //this._exists = templateFuncToClone.Exists;
            this.Func = templateFuncToClone.Func;
            if ((args == null || args.Length == 0) && templateFuncToClone.Args != null && templateFuncToClone.Args.Length > 0)
            {
                this._args = new string[templateFuncToClone.Args.Length];
                templateFuncToClone.Args.CopyTo(this._args, 0);
            }
            else if(args!=null && args.Length>0)
            {
                this._args = new string[args.Length];
                args.CopyTo(this._args, 0);
            }
        }

        public object Invoke(string[] strArgs=null)
        {
            object[] args = GetArgs(strArgs);
            if (this.Func != null)
            {
                if (args != null) return this.Func(args);
                else return this.Func();
            }
            else if (StructXMLParser.FunctionsList.ContainsKey(_name))
            {
                if (StructXMLParser.FunctionsList[_name].Func != null)
                {
                    if (args != null) return StructXMLParser.FunctionsList[_name].Func(args);
                    else return StructXMLParser.FunctionsList[_name].Func();
                }
                else
                {
                    throw new Exception("function [" + _name + "] exists in functionList. but not correct function.");
                    //return null;//function 리스트에 functionInfo는 있지만 func가 배정되지 않았다.
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 실제 args들을 가져온다.
        /// </summary>
        /// <param name="argsToInsert"></param>
        /// <returns></returns>
        public object[] GetArgs(String[] argsToInsert=null)
        {

            String[] strArgs = (argsToInsert==null)? _args : argsToInsert;
            if (strArgs != null && strArgs.Length > 0)
            {
                object[] args = new object[strArgs.Length];
                for (int i = 0; i < strArgs.Length; i++)
                {
                    if (strArgs[i].Length > 0 && strArgs[i][0].Equals('@'))//변수
                    {
                        String varName = strArgs[i].Substring(1);
                        if (StructXMLParser.VariablesList.ContainsKey(varName))
                        {
                            if (StructXMLParser.VariablesList[varName].Values.Length > 1)
                            {
                                args[i] = StructXMLParser.VariablesList[varName].Values;//변수가 배열이면 배열 그대로 가져옴..
                            }
                            else
                            {
                                args[i] = StructXMLParser.VariablesList[varName].Values[0];//변수가 일반 숫자면 그냥 숫자를 가져옴..
                            }
                        }
                        else
                        {
                            args[i] = strArgs[i];//그대로 넣어줌..
                        }
                    }
                }
                return args;
            }
            else return null;
        }

        public FunctionInfo()
        {
            //this._exists = false;
        }

        public String Name { get { return _name; } }

        public String[] Args { get { return _args; } }

        public bool Exists { get {
            if (_name == null || _name.Length == 0) return false;
            else if (StructXMLParser.FunctionsList.ContainsKey(_name)) return true;
            else return false;
        } }

        public void setFunction(String name, funcInfoFunc func, params String[] args)
        {
            this._name = name;
            this._args = args;
            this.Func = func;
        }

        public void setFunction(FunctionInfo funcInfo, params string[] args)
        {
            this._name = funcInfo.Name;
            if(args!=null) this._args = args;
            else if (funcInfo.Args != null && funcInfo.Args.Length>0)
            {
                this._args = new string[funcInfo.Args.Length];
                funcInfo.Args.CopyTo(this._args, 0);
            }
            this.Func = funcInfo.Func;
        }



    }
    public delegate object funcInfoFunc(params object[] args);
}

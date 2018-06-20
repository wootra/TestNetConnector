using System;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CSharp;

namespace EvalCSCode
{
	public class EvalCSCode
	{
		public static object Eval(string sCSCode) 
		{
			CSharpCodeProvider c = new CSharpCodeProvider();
			ICodeCompiler icc = c.CreateCompiler();
			CompilerParameters cp = new CompilerParameters();

			cp.ReferencedAssemblies.Add("system.dll");
			cp.ReferencedAssemblies.Add("system.xml.dll");
			cp.ReferencedAssemblies.Add("system.data.dll");
			cp.ReferencedAssemblies.Add("system.windows.forms.dll");
			cp.ReferencedAssemblies.Add("system.drawing.dll");

			cp.CompilerOptions = "/t:library";
			cp.GenerateInMemory = true;
			StringBuilder sb = new StringBuilder("");
			
			sb.Append("using System;\n" );
			sb.Append("using System.Xml;\n");
			sb.Append("using System.Data;\n");
			sb.Append("using System.Data.SqlClient;\n");
			sb.Append("using System.Windows.Forms;\n");
			sb.Append("using System.Drawing;\n");
				
			sb.Append("namespace CSCodeEvaler{ \n");
			sb.Append("public class CSCodeEvaler{ \n");
			sb.Append("public object EvalCode(){\n");
			sb.Append("return "+sCSCode+"; \n");
			sb.Append("} \n");
			sb.Append("} \n");
			sb.Append("}\n");

			CompilerResults cr = icc.CompileAssemblyFromSource(cp, sb.ToString());
			if( cr.Errors.Count > 0 )
			{
				MessageBox.Show("ERROR: " + cr.Errors[0].ErrorText, "Error evaluating cs code", MessageBoxButtons.OK ,MessageBoxIcon.Error );
				return null;
			}

			System.Reflection.Assembly a = cr.CompiledAssembly;
			object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");
			Type t = o.GetType();
			MethodInfo mi = t.GetMethod("EvalCode");
			object s = mi.Invoke(o, null);
			return s;
		}

		public static object EvalWithParam(string sCSCode, object oParam) 
		{
			CSharpCodeProvider c = new CSharpCodeProvider();
			ICodeCompiler icc = c.CreateCompiler();
			CompilerParameters cp = new CompilerParameters();

			cp.ReferencedAssemblies.Add("system.dll");
			cp.ReferencedAssemblies.Add("system.xml.dll");
			cp.ReferencedAssemblies.Add("system.data.dll");
			cp.ReferencedAssemblies.Add("system.windows.forms.dll");
			cp.ReferencedAssemblies.Add("system.drawing.dll");
			//cp.ReferencedAssemblies.Add("EvalCSCode.exe");

			cp.CompilerOptions = "/t:library";
			cp.GenerateInMemory = true;
			StringBuilder sb = new StringBuilder("");
			
			sb.Append("using System;\n" );
			sb.Append("using System.Xml;\n");
			sb.Append("using System.Data;\n");
			sb.Append("using System.Data.SqlClient;\n");
			sb.Append("using System.Windows.Forms;\n");
			sb.Append("using System.Drawing;\n");
			//sb.Append("using EvalCSCode;\n");
			
			sb.Append("namespace CSCodeEvaler{ \n");
			sb.Append("public class CSCodeEvaler{ \n");
			sb.Append("public object EvalCode(object oParam){\n");
			sb.Append("MessageBox.Show(oParam.ToString()); \n");
			sb.Append("return "+sCSCode+"; \n");
			sb.Append("} \n");
			sb.Append("} \n");
			sb.Append("}\n");
			//Debug.WriteLine(sb.ToString())// ' look at this to debug your eval string

			CompilerResults cr = icc.CompileAssemblyFromSource(cp, sb.ToString());
			if( cr.Errors.Count > 0 )
			{
				MessageBox.Show("ERROR: " + cr.Errors[0].ErrorText, "Error evaluating cs code", MessageBoxButtons.OK ,MessageBoxIcon.Error );
				return null;
			}

			System.Reflection.Assembly a = cr.CompiledAssembly;
			object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");
			Type t = o.GetType();
			MethodInfo mi = t.GetMethod("EvalCode");
			object[] oParams = new object[1];
			oParams[0] = oParam;
			object s = mi.Invoke(o, oParams);
			return s;
		}

		public static object EvalWithRef(string sCSCode) 
		{
			CSharpCodeProvider c = new CSharpCodeProvider();
			ICodeCompiler icc = c.CreateCompiler();
			CompilerParameters cp = new CompilerParameters();

			cp.ReferencedAssemblies.Add("system.dll");
			cp.ReferencedAssemblies.Add("system.xml.dll");
			cp.ReferencedAssemblies.Add("system.data.dll");
			cp.ReferencedAssemblies.Add("system.windows.forms.dll");
			cp.ReferencedAssemblies.Add("system.drawing.dll");
			cp.ReferencedAssemblies.Add("EvalCSCode.exe");

			cp.CompilerOptions = "/t:library";
			cp.GenerateInMemory = true;
			StringBuilder sb = new StringBuilder("");
			
			sb.Append("using System;\n" );
			sb.Append("using System.Xml;\n");
			sb.Append("using System.Data;\n");
			sb.Append("using System.Data.SqlClient;\n");
			sb.Append("using System.Windows.Forms;\n");
			sb.Append("using System.Drawing;\n");
			sb.Append("using EvalCSCode;\n");
			
			sb.Append("namespace CSCodeEvaler{ \n");
			sb.Append("public class CSCodeEvaler{ \n");
			sb.Append("public object EvalCode(){\n");
			sb.Append("object o = new object(); \n");
			sb.Append(sCSCode + " \n");
			sb.Append("return o; \n");
			sb.Append("} \n");
			sb.Append("} \n");
			sb.Append("}\n");
			//Debug.WriteLine(sb.ToString())// ' look at this to debug your eval string

			CompilerResults cr = icc.CompileAssemblyFromSource(cp, sb.ToString());
			if( cr.Errors.Count > 0 )
			{
				MessageBox.Show("ERROR: " + cr.Errors[0].ErrorText, "Error evaluating cs code", MessageBoxButtons.OK ,MessageBoxIcon.Error );
				return null;
			}

			System.Reflection.Assembly a = cr.CompiledAssembly;
			object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");
			Type t = o.GetType();
			MethodInfo mi = t.GetMethod("EvalCode");
			object s = mi.Invoke(o, null);
			return s;
		}

		public static void callAFunc(){
			//EvalCSCode.EvalCSCode.callAFunc();
			MessageBox.Show("callAFunc: Hello from a independent function in this assembly","HyFromAFunc", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public void callBFunc()
		{
			//EvalCSCode.EvalCSCode.callAFunc();
			MessageBox.Show("callBFunc: Hello from another independent function in this assembly","HyFromBFunc", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
//End Namespace
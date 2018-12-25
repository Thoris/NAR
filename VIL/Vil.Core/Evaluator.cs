using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;

namespace Vil.Core
{
    public class Evaluator
    {
        // Fields
        private object _Compiled;
        private Type _CompiledType;
        private const string staticMethodName = "__foo";

        // Methods
        public Evaluator(EvaluatorItem[] items)
        {
            this._CompiledType = null;
            this._Compiled = null;
            this.ConstructEvaluator(items);
        }

        public Evaluator(EvaluatorItem item)
        {
            this._CompiledType = null;
            this._Compiled = null;
            EvaluatorItem[] items = new EvaluatorItem[] { item };
            this.ConstructEvaluator(items);
        }

        public Evaluator(Type returnType, string expression, string name)
        {
            this._CompiledType = null;
            this._Compiled = null;
            EvaluatorItem[] items = new EvaluatorItem[] { new EvaluatorItem(returnType, expression, name) };
            this.ConstructEvaluator(items);
        }

        private void ConstructEvaluator(EvaluatorItem[] items)
        {
            ICodeCompiler compiler = new CSharpCodeProvider().CreateCompiler();
            CompilerParameters parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add("system.dll");
            parameters.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(MetricProvider)).Location);
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            StringBuilder builder = new StringBuilder();
            builder.Append("using System; \n");
            builder.Append("namespace Vil.Core { \n");
            builder.Append("  public class _Evaluator { \n");
            foreach (EvaluatorItem item in items)
            {
                builder.AppendFormat("    public {0} {1}(MetricProvider me) ", item.ReturnType.Name, item.Name);
                builder.Append("{ ");
                builder.AppendFormat("      return ({0}); ", item.Expression);
                builder.Append("}\n");
            }
            builder.Append("} }");
            CompilerResults results = compiler.CompileAssemblyFromSource(parameters, builder.ToString());
            if (results.Errors.HasErrors)
            {
                StringBuilder builder2 = new StringBuilder();
                builder2.Append("Error Compiling Expression: ");
                foreach (CompilerError error in results.Errors)
                {
                    builder2.AppendFormat("{0}\n", error.ErrorText);
                }
                throw new Exception("Error Compiling Expression: " + builder2.ToString());
            }
            this._Compiled = results.CompiledAssembly.CreateInstance("Vil.Core._Evaluator");
        }

        public object Evaluate(string name)
        {
            return this._Compiled.GetType().GetMethod(name).Invoke(this._Compiled, null);
        }

        public bool EvaluateBool(string name)
        {
            return (bool)this.Evaluate(name);
        }

        public int EvaluateInt(string name)
        {
            return (int)this.Evaluate(name);
        }

        public bool EvaluateMyBool(string name, MetricProvider metricProvider)
        {
            return (bool)this._Compiled.GetType().GetMethod(name).Invoke(this._Compiled, new object[] { metricProvider });
        }

        public string EvaluateString(string name)
        {
            return (string)this.Evaluate(name);
        }

        public static bool EvaluateToBool(string code)
        {
            Evaluator evaluator = new Evaluator(typeof(bool), code, "__foo");
            return (bool)evaluator.Evaluate("__foo");
        }

        public static int EvaluateToInteger(string code)
        {
            Evaluator evaluator = new Evaluator(typeof(int), code, "__foo");
            return (int)evaluator.Evaluate("__foo");
        }

        public static object EvaluateToObject(string code)
        {
            Evaluator evaluator = new Evaluator(typeof(object), code, "__foo");
            return evaluator.Evaluate("__foo");
        }

        public static string EvaluateToString(string code)
        {
            Evaluator evaluator = new Evaluator(typeof(string), code, "__foo");
            return (string)evaluator.Evaluate("__foo");
        }
    }


}
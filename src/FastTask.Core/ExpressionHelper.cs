using System;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace FastTask.Core
{
    public class ExpressionHelper
    {
        public static string ToString(LambdaExpression expression)
        {
            return (expression.Body as MethodCallExpression).Method.ToString();
        }
        public static Script ToExpression(string expression)
        {
            return CSharpScript.Create<Action>(expression);
        }
    }
}
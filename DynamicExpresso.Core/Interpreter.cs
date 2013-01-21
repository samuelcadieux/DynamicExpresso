﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DynamicExpresso
{
    public class Interpreter
    {
        ParserSettings _settings = new ParserSettings();

        public Interpreter()
        {
        }

        public Interpreter SetVariable(string name, object value)
        {
            _settings.Keywords.Add(name, Expression.Constant(value));

            return this;
        }

        public Interpreter SetFunction<TDelegate>(string name, Expression<TDelegate> expression)
        {
            _settings.Keywords.Add(name, expression);

            return this;
        }

        public Interpreter Using(Type type)
        {
            _settings.KnownTypes.Add(type.Name, type);

            return this;
        }

        public Function Parse(string expressionText, params FunctionParameter[] parameters)
        {
            var arguments = (from p in parameters
                            select ParameterExpression.Parameter(p.Type, p.Name)).ToArray();

            var parser = new ExpressionParser(expressionText, arguments, _settings);
            var expression = parser.Parse();

            var lambdaExp = Expression.Lambda(expression, arguments);

            return new Function(lambdaExp);
        }

        public object Eval(string expressionText, params FunctionParameter[] parameters)
        {
            return Parse(expressionText, parameters).Invoke(parameters);
        }
    }
}

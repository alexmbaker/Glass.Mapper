﻿using System.Configuration;
using Glass.Mapper.Configuration;
using Glimpse.Core.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Mapper.Glimpse
{
    public abstract class ContextTab : ITab
    {
        public string Name
        {
            get { return "Glass.Mapper Context"; }
        }

        public object GetData(ITabContext context)
        {
            return GetContexts();
        }

        public RuntimeEvent ExecuteOn
        {
            get { return RuntimeEvent.EndRequest; }
        }

        public Type RequestContextType
        {
            get { return null; }
        }

        private object GetContexts()
        {
            var contextDatas = new List<object>();

            foreach (var context in Glass.Mapper.Context.Contexts)
            {
                var contextData = new
                    {
                        ContextName = context.Value.Name,
                        Configurations = context.Value.TypeConfigurations.Select(x => MapTypeConfiguration(x.Value))
                    };
                contextDatas.Add(contextData);
            }
            return contextDatas;
        }

        public abstract object MapTypeConfiguration(AbstractTypeConfiguration config);

        protected string CleanTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var inners = type.GenericTypeArguments;
                var sb = new StringBuilder();
                var genericPart = type.GetGenericTypeDefinition();


                if (genericPart == typeof (IEnumerable<>))
                    sb.Append("IEnumerable");

                sb.Append("<");
                foreach (var inner in inners)
                {
                    sb.Append(CleanTypeName(inner));
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(">");

                return sb.ToString();

            }
            else
            {
                return type.FullName;
            }
        }
    }
}
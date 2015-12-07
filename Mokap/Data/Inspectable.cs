using NLog;
using System;
using System.Reflection;
using System.Text;

namespace Mokap.Data
{
    abstract class Inspectable
    {
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(base.ToString());

            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (fields.Length > 0)
            {
                builder.Append("(");

                var lastField = fields[fields.Length - 1];
                foreach (var field in fields)
                {
                    var value = field.GetValue(this);
                    if (value != null)
                    {
                        if (field.FieldType.IsArray)
                        {
                            builder.AppendFormat("Count({0})={1}", field.Name, ((Array)value).Length);
                        }
                        else
                        {
                            builder.AppendFormat("{0}={1}", field.Name, value.ToString());
                        }

                        if (field != lastField)
                        {
                            builder.Append(", ");
                        }
                    }
                }

                builder.Append(")");
            }

            return builder.ToString();
        }
    }
}

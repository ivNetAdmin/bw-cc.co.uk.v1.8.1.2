
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

using NHibernate;
using NHibernate.Transform;

namespace ivNet.Club.Helpers
{
    public static class NhTransformers
    {
        public static readonly IResultTransformer ExpandoObject;

        static NhTransformers()
        {
            ExpandoObject = new ExpandoObjectResultSetTransformer();
        }

        private class ExpandoObjectResultSetTransformer : IResultTransformer
        {
            public IList TransformList(IList collection)
            {
                return collection;
            }

            public object TransformTuple(object[] tuple, string[] aliases)
            {
                var expando = new ExpandoObject();
                var dictionary = (IDictionary<string, object>) expando;
                for (int i = 0; i < tuple.Length; i++)
                {
                    string alias = aliases[i];
                    if (alias != null)
                    {
                        dictionary[alias] = tuple[i];
                    }
                }
                return expando;
            }
        }
    }

    public static class NHibernateExtensions
    {
        public static IList<dynamic> DynamicList(this IQuery query)
        {
            return query.SetResultTransformer(NhTransformers.ExpandoObject)
                .List<dynamic>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.UserTypes;
using NHibernate.Type;
using System.Reflection;
using System.Data;
using NHibernate.Engine;
using NHibernate;
using System.Globalization;

namespace Money.Storage.NHibernate
{    
    public class MoneyCompositeUserType : GenericUserType<Money>
    {
        public MoneyCompositeUserType(
                        bool isMutable, 
                        IType[] propertyTypes, 
                        string[] propertyNames, 
                        GetMap getMap, 
                        DeepCopyMap deepCopyMethod)
            : base(isMutable, propertyTypes, propertyNames, getMap, deepCopyMethod)
        {
        }

        public MoneyCompositeUserType()
            : this("")
        {
        }

        public MoneyCompositeUserType(string columnPrefix)
            : this(false,
                new IType[] { NHibernateUtil.Currency, NHibernateUtil.CultureInfo },
                new string[] { "Amount", "CultureInfo" }.Select(col => columnPrefix+col).ToArray(),
                new GetMap(delegate(object[] values){
                    decimal amount = (decimal)values[0];
                    string cultureName = (string)values[1];
                    return new Money(amount, cultureName);
                }),
                new DeepCopyMap(delegate(Money source) { 
                    return source == null ? null : new Money(source.Amount, source.EnglishCultureName);
                }))
        {
        }
    }
}

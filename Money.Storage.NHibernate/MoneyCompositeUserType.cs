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
using NCommon.NHibernate;

namespace Money.Storage.NHibernate
{    
    public class MoneyCompositeUserType : CompositeUserTypeBase<Money>
    {
        /// <summary>
        /// Initializes a new instance of the MoneyCompositeUserType class.
        /// </summary>
        public MoneyCompositeUserType()
        {
            MapProperty(x => x.Amount);
            MapProperty(x => x.EnglishCultureName);
        }

        protected override Money CreateInstance(object[] propertyValues)
        {
            decimal amount = (decimal)propertyValues[0];
            string cultureName = propertyValues[1].ToString();

            return new Money(amount, cultureName);
        }

        protected override Money PerformDeepCopy(Money source)
        {
            return source == null ? null : new Money(source.Amount, source.EnglishCultureName);
        }

        public override bool IsMutable
        {
            get { return false; }
        }
    }
}

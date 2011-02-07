using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.UserTypes;
using NHibernate.Type;
using System.Reflection;
using System.Data;
using NHibernate.Engine;

namespace Money.Storage.NHibernate
{
    public class ExampleMoneyCompositeUserType : ICompositeUserType
    {
        ///<summary>
        ///
        ///            Get the value of a property
        ///           
        ///</summary>
        ///
        ///<param name="component">an instance of class mapped by this "type"</param>
        ///<param name="property">The position index of the property.Corresponds to the column
        ///  index within the IDataReader row.
        /// See <see cref="NullSafeGet"/> method for indices knowledge.
        /// </param>
        ///<returns>
        ///the property value
        ///</returns>
        ///
        public object GetPropertyValue(object component, int property)
        {
            // 0 =Amount
            // 1 = Currency
            Money money = (Money)component;
            if (property == 0)
            {
                return money.Amount;
            }
            else
            {
                return money.Currency;
            }
        }

        ///<summary>
        ///
        ///            Set the value of a property
        ///           
        ///</summary>
        ///
        ///<param name="component">an instance of class mapped by this "type"</param>
        ///<param name="property"></param>
        ///<param name="value">the value to set</param>
        public void SetPropertyValue(object component, int property, object value)
        {
            throw new InvalidOperationException("Money is an immutable object. SetPropertyValue isn't supported.");

        }

        ///<summary>
        ///
        ///            Compare two instances of the class mapped by this type for persistence
        ///            "equality", ie. equality of persistent state.
        ///           
        ///</summary>
        ///
        ///<param name="x"></param>
        ///<param name="y"></param>
        ///<returns>
        ///
        ///</returns>
        ///
        public new bool Equals(object x, object y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        ///<summary>
        ///
        ///            Retrieve an instance of the mapped class from a IDataReader. Implementors
        ///            should handle possibility of null values.
        ///           
        ///</summary>
        ///
        ///<param name="dr">IDataReader</param>
        ///<param name="names">the column names</param>
        ///<param name="session"></param>
        ///<param name="owner">the containing entity</param>
        ///<returns>
        ///An new instance of the custom type.
        ///</returns>
        ///
        public object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner)
        {
            if (dr == null)
            {
                return null;
            }
            string amountColumn = names[0];
            string currencyColumn = names[1];
            double val = (double)NHibernateUtil.Double.NullSafeGet(dr, amountColumn, session, owner);
            string currency = NHibernateUtil.String.NullSafeGet(dr, currencyColumn, session, owner).ToString();

            Money money = new Money(val, currency);
            return money;

        }

        ///<summary>
        ///
        ///            Write an instance of the mapped class to a prepared statement.
        ///            Implementors should handle possibility of null values.
        ///            A multi-column type should be written to parameters starting from index.
        ///           
        ///</summary>
        ///
        ///<param name="cmd"></param>
        ///<param name="value"></param>
        ///<param name="index"></param>
        ///<param name="session"></param>
        public void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (value == null)
                return;
            double amount = 0;
            string currency = "";
            if (value != null)
            {
                amount = ((Money)value).Amount;
                currency = ((Money)value).Currency;

            }
            NHibernateUtil.Double.NullSafeSet(cmd, amount, index, session);
            NHibernateUtil.String.NullSafeSet(cmd, currency, index + 1, session);
        }

        ///<summary>
        ///
        ///            Return a deep copy of the persistent state, stopping at entities and at collections.
        ///           
        ///</summary>
        ///
        ///<param name="value">generally a collection element or entity field</param>
        ///<returns>
        ///
        ///</returns>
        ///
        public object DeepCopy(object value)
        {
            return new Money(((Money)value).Amount, ((Money)value).Currency);
        }

        ///<summary>
        ///
        ///            Transform the object into its cacheable representation.
        ///            At the very least this method should perform a deep copy.
        ///            That may not be enough for some implementations,
        ///            method should perform a deep copy.
        ///        That may not be enough for some implementations, however;
        ///        for example, associations must be cached as identifier values. (optional operation)
        ///           
        ///</summary>
        ///
        ///<param name="value">the object to be cached</param>
        ///<param name="session"></param>
        ///<returns>
        ///
        ///</returns>
        ///
        public object Disassemble(object value, ISessionImplementor session)
        {
            return DeepCopy(value);
        }

        ///<summary>
        ///
        ///            Reconstruct an object from the cacheable representation.
        ///            At the very least this method should perform a deep copy. (optional operation)
        ///           
        ///</summary>
        ///
        ///<param name="cached">the object to be cached</param>
        ///<param name="session"></param>
        ///<param name="owner"></param>
        ///<returns>
        ///
        ///</returns>
        ///
        public object Assemble(object cached, ISessionImplementor session, object owner)
        {
            return DeepCopy(cached);
        }

        ///<summary>
        ///
        ///            Get the "property names" that may be used in a query.
        ///           
        ///</summary>
        ///
        public string[] PropertyNames
        {
            get { return new string[2] { "Amount", "Currency" }; }
        }

        ///<summary>
        ///
        ///            Get the corresponding "property types"
        ///           
        ///</summary>
        ///
        public IType[] PropertyTypes
        {
            get
            {
                return new IType[2] { NHibernateUtil.Double, NHibernateUtil.String };
            }
        }

        ///<summary>
        ///
        ///            The class returned by NullSafeGet().
        ///           
        ///</summary>
        ///
        public Type ReturnedClass
        {
            get { return typeof(Money); }
        }

        ///<summary>
        ///
        ///            Are objects of this type mutable?
        ///           
        ///</summary>
        ///
        public bool IsMutable
        {
            get { return false; }
        }
    }
}

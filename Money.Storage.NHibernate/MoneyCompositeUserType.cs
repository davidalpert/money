using System;
using System.Data;
using NHibernate.UserTypes;
using NHibernate;
using FluentNHibernate.Conventions;

public class MoneyCompositeUserType : ICompositeUserType
{
    public Type ReturnedClass { get { return typeof(Money.Money); } }

    public new bool Equals(object x, object y)
    {
        if (object.ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;
        return x.Equals(y);
    }

    public object DeepCopy(object value) { return value; }

    public bool IsMutable { get { return false; } }

    public object NullSafeGet(IDataReader dr, string[] names, NHibernate.Engine.ISessionImplementor session, object owner)
    {
        object obj0 = NHibernateUtil.Decimal.NullSafeGet(dr, names[0]);
        object obj1 = NHibernateUtil.String.NullSafeGet(dr, names[1]);
        if (obj0 == null || obj1 == null) return null;
        decimal value = (decimal)obj0;
        string currency = (string)obj1;
        return new Money.Money(value, currency);
    }

    public void NullSafeSet(IDbCommand cmd, object obj, int index, NHibernate.Engine.ISessionImplementor session)
    {
        if (obj == null)
        {
            ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
            ((IDataParameter)cmd.Parameters[index + 1]).Value = DBNull.Value;
        }
        else
        {
            Money.Money src = (Money.Money)obj;
            ((IDataParameter)cmd.Parameters[index]).Value = src.Amount;
            ((IDataParameter)cmd.Parameters[index + 1]).Value = src.EnglishCultureName;
        }
    }

    public string[] PropertyNames
    {
        get { return new string[] { "Amount", "CultureInfo" }; }
    }

    public NHibernate.Type.IType[] PropertyTypes
    {
        get
        {
            return new NHibernate.Type.IType[] {
            NHibernateUtil.Decimal, NHibernateUtil.String };
        }
    }

    public object GetPropertyValue(object component, int property)
    {
        Money.Money src = (Money.Money)component;
        if (property == 0)
            return src.Amount;
        else
            return src.EnglishCultureName;
    }

    public void SetPropertyValue(object comp, int property, object value) { 
        throw new Exception("Immutable!");
    }

    public object Assemble(object cached, NHibernate.Engine.ISessionImplementor session, object owner)
    {
        return cached;
    }

    public object Disassemble(object value, NHibernate.Engine.ISessionImplementor session)
    {
        return value;
    }

    public int GetHashCode(object x)
    {
        return x.GetHashCode();
    }

    public object Replace(object original, object target, NHibernate.Engine.ISessionImplementor session, object owner)
    {
        return original; 
    }
}
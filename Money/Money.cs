using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Money
{
    /// <summary>
    /// A class for handling Money
    /// </summary>
    /// <remarks>
    /// <para>
    /// Based on Jason Hunt's C# Money class 
    /// (http://www.noticeablydifferent.com/CodeSamples/Money.aspx)
    /// licensed under a Creative Commons Attribution 2.5 Canada License. 
    /// </para>
    /// </remarks>
    public sealed class Money : IEquatable<Money>, IComparable, IComparable<Money>
    {
        CultureInfo cultureInfo;
        RegionInfo regionInfo;
        decimal amount;

        public Money() : this(0, CultureInfo.CurrentCulture) { }
        public Money(decimal amount) : this(amount, CultureInfo.CurrentCulture) { }
        public Money(long amount) : this(amount, CultureInfo.CurrentCulture) { }
        public Money(string cultureName) : this(new CultureInfo(cultureName)) { }
        public Money(decimal amount, string cultureName) : this(amount, new CultureInfo(cultureName)) { }
        public Money(CultureInfo cultureInfo) : this(0, cultureInfo) { }
        public Money(decimal amount, CultureInfo cultureInfo)
        {
            if (cultureInfo == null) throw new ArgumentNullException("cultureInfo");
            this.cultureInfo = cultureInfo;
            this.regionInfo = new RegionInfo(cultureInfo.LCID);
            this.amount = amount;
        }
        public Money(long amount, CultureInfo cultureInfo)
        {
            if (cultureInfo == null) throw new ArgumentNullException("cultureInfo");
            this.cultureInfo = cultureInfo;
            this.regionInfo = new RegionInfo(cultureInfo.LCID);
            this.amount = amount;
        }
        public string EnglishCultureName
        {
            get { return cultureInfo.Name; }
        }
        public string ISOCurrencySymbol
        {
            get { return regionInfo.ISOCurrencySymbol; }
        }
        public decimal Amount
        {
            get { return amount; }
        }
        public int DecimalDigits
        {
            get { return cultureInfo.NumberFormat.CurrencyDecimalDigits; }
        }
        public static bool operator >(Money first, Money second)
        {
            AssertSameCurrency(first, second);
            return first.amount > second.amount;
        }
        public static bool operator >=(Money first, Money second)
        {
            AssertSameCurrency(first, second);
            return first.amount >= second.amount;
        }
        public static bool operator <=(Money first, Money second)
        {
            AssertSameCurrency(first, second);
            return first.amount <= second.amount;
        }
        public static bool operator <(Money first, Money second)
        {
            AssertSameCurrency(first, second);
            return first.amount < second.amount;
        }
        public static Money operator +(Money first, Money second)
        {
            AssertSameCurrency(first, second);
            return new Money(first.Amount + second.Amount, first.EnglishCultureName);
        }
        public static Money Add(Money first, Money second)
        {
            return first + second;
        }
        public static Money operator -(Money first, Money second)
        {
            AssertSameCurrency(first, second);
            return new Money(first.Amount - second.Amount, first.EnglishCultureName);
        }
        public static Money Subtract(Money first, Money second)
        {
            return first - second;
        }
        public static implicit operator Money(decimal amount)
        {
            return new Money(amount, CultureInfo.CurrentCulture);
        }
        public static implicit operator Money(long amount)
        {
            return new Money(amount, CultureInfo.CurrentCulture);
        }
        public override bool Equals(object obj)
        {
            return (obj is Money) && Equals((Money)obj);
        }
        public override int GetHashCode()
        {
            return amount.GetHashCode() ^ cultureInfo.GetHashCode();
        }
        private static void AssertSameCurrency(Money first, Money second)
        {
            if (first.ISOCurrencySymbol != second.ISOCurrencySymbol)
                throw new InvalidOperationException("Money type mismatch.");
        }
        public bool Equals(Money other)
        {
            if (object.ReferenceEquals(other, null)) return false;
            return ((ISOCurrencySymbol == other.ISOCurrencySymbol) && (amount == other.amount));
        }
        public static bool operator ==(Money first, Money second)
        {
            if (object.ReferenceEquals(first, second)) return true;
            if (object.ReferenceEquals(first, null) || object.ReferenceEquals(second, null)) return false;
            return (first.ISOCurrencySymbol == second.ISOCurrencySymbol && first.Amount == second.Amount);
        }
        public static bool operator !=(Money first, Money second)
        {
            return !first.Equals(second);
        }
        public static Money operator *(Money money, decimal value)
        {
            if (money == null) throw new ArgumentNullException("money");
            return new Money(Decimal.Floor(money.Amount * value), money.EnglishCultureName);
        }
        public static Money Multiply(Money money, decimal value)
        {
            return money * value;
        }
        public static Money operator /(Money money, decimal value)
        {
            if (money == null) throw new ArgumentNullException("money");
            return new Money(money.Amount / value, money.EnglishCultureName);
        }
        public static Money Divide(Money first, decimal value)
        {
            return first / value;
        }
        public Money Copy()
        {
            return new Money(Amount, cultureInfo);
        }
        public Money Clone()
        {
            return new Money(cultureInfo);
        }
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (!(obj is Money))
            {
                throw new ArgumentException("Argument must be money");
            }
            return CompareTo((Money)obj);
        }
        public int CompareTo(Money other)
        {
            if (this < other)
            {
                return -1;
            }
            if (this > other)
            {
                return 1;
            }
            return 0;
        }
        public override string ToString()
        {
            return Amount.ToString("C", cultureInfo);
        }
        public string ToString(string format)
        {
            return Amount.ToString(format, this.cultureInfo);
        }
        public static Money LocalCurrency
        {
            get { return new Money(CultureInfo.CurrentCulture); }
        }
    }
}

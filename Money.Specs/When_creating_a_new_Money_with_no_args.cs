using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using System.Globalization;
using System.Threading;

namespace Money.Specifications
{
    public class When_creating_a_new_Money_with_no_args 
    {
        static Money money;

        Establish context = () =>
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        };

        Because of = () => money = new Money();

        It should_default_to_an_amount_of_0 = () => money.Amount.ShouldEqual(0m);

        It should_have_a_precision_of_2_decimals = () => money.DecimalDigits.ShouldEqual(2);

        It should_default_to_the_culture_of_the_current_thread = () => 
            money.EnglishCultureName.ShouldEqual(Thread.CurrentThread.CurrentCulture.Name);

        It should_default_to_the_currency_symbol_for_the_current_region = () => 
            money.ISOCurrencySymbol.ShouldEqual(RegionInfo.CurrentRegion.ISOCurrencySymbol);

        It should_use_the_current_culture_when_converting_to_a_string = () => 
            money.ToString().ShouldEqual(0m.ToString("C", Thread.CurrentThread.CurrentCulture));
    }

    public class When_creating_a_new_Money_with_enCA_and_no_amount
    {
        static Money money;

        Because of = () => money = new Money("en-CA");

        It should_have_an_amount_of_0 = () => money.Amount.ShouldEqual(0m);

        It should_have_a_precision_of_2_decimals = () => money.DecimalDigits.ShouldEqual(2);

        It should_default_to_frCA = () => money.EnglishCultureName.ShouldEqual("en-CA");

        It should_default_to_CAD_as_the_currency_symbol = () => money.ISOCurrencySymbol.ShouldEqual("CAD");

        It should_ToString_as_french_currency = () => money.ToString().ShouldEqual("$0.00");
    }

    public class When_creating_a_new_Money_with_frCA_and_no_amount
    {
        static Money money;

        Because of = () => money = new Money("fr-CA");

        It should_have_an_amount_of_0 = () => money.Amount.ShouldEqual(0m);

        It should_have_a_precision_of_2_decimals = () => money.DecimalDigits.ShouldEqual(2);

        It should_default_to_frCA = () => money.EnglishCultureName.ShouldEqual("fr-CA");

        It should_default_to_CAD_as_the_currency_symbol = () => money.ISOCurrencySymbol.ShouldEqual("CAD");

        It should_ToString_as_french_currency = () => money.ToString().ShouldEqual("0,00 $");
    }

    public class When_creating_a_new_Money_with_enUS_and_no_amount
    {
        static Money money;

        Because of = () => money = new Money("en-US");

        It should_have_an_amount_of_0 = () => money.Amount.ShouldEqual(0m);

        It should_have_a_precision_of_2_decimals = () => money.DecimalDigits.ShouldEqual(2);

        It should_default_to_frCA = () => money.EnglishCultureName.ShouldEqual("en-US");

        It should_default_to_CAD_as_the_currency_symbol = () => money.ISOCurrencySymbol.ShouldEqual("USD");

        It should_ToString_as_french_currency = () => money.ToString().ShouldEqual("$0.00");
    }

    public class When_creating_a_new_Money_with_decimal_4_point_33
    {
        static Money money;

        Because of = () => money = new Money(4.33m);

        It should_have_an_amount_of_4_dollars_and_33_cents = () => money.Amount.ShouldEqual(4.33m);

        It should_have_a_precision_of_2_decimals = () => money.DecimalDigits.ShouldEqual(2);

        It should_ToString_with_the_proper_amount = () => money.ToString().ShouldEqual("$4.33");
    }
}
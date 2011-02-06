using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using System.Globalization;

namespace Money.Specifications
{
    public class When_creating_a_new_Money_with_no_args
    {
        static Money money;

        Because of = () => money = new Money();

        It should_have_an_amount_of_0 = () => money.Amount.ShouldEqual(0m);

        It should_have_a_precision_of_2_decimals = () => money.DecimalDigits.ShouldEqual(2);

        It should_default_to_enCA = () => money.EnglishCultureName.ShouldEqual("en-CA");

        It should_default_to_CAD_as_the_currency_symbol = () => money.ISOCurrencySymbol.ShouldEqual("CAD");

        It should_ToString_as_english_currency = () => money.ToString().ShouldEqual("$0.00");
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

    public class When_creating_a_new_Money_with_decimal_4_point_33
    {
        static Money money;

        Because of = () => money = new Money(4.33m);

        It should_have_an_amount_of_4_dollars_and_33_cents = () => money.Amount.ShouldEqual(4.33m);

        It should_have_a_precision_of_2_decimals = () => money.DecimalDigits.ShouldEqual(2);

        It should_ToString_with_the_proper_amount = () => money.ToString().ShouldEqual("$4.33");
    }
}
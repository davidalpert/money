using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using FluentNHibernate.Conventions;
using Money.Storage.NHibernate;

namespace Money.Specs.TestContext
{
    public class ProductMap : ClassMap<Product>
    {
        /// <summary>
        /// Initializes a new instance of the ProductMap class.
        /// </summary>
        public ProductMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.PurchasePrice, "Purchase").CustomType<MoneyCompositeUserType>();
            //Map(x => x.RentalPrice, "Rental").CustomType<MoneyCompositeUserType>();
        }
    }

    public class Product
    {
        /// <summary>
        /// Initializes a new instance of the Product class.
        /// </summary>
        public Product()
        {
            Id = Guid.Empty;
            RentalPrice = new Money();
            PurchasePrice = new Money();
        }

        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual Money RentalPrice { get; set; }

        public virtual Money PurchasePrice { get; set; }
    }
}

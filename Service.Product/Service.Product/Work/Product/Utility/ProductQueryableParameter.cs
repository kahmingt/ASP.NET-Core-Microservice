using Service.Product.Shared.Utility;

namespace Service.Product.Work.Utility
{
    public class ProductQueryableParameter : QueryableParameterBase
    {
        public ProductQueryableParameter()
        {
            // Default sorting order.
            OrderBy = "ProductID";
        }

        /// <summary>
        /// Return all Products with UnitInStock value greater than MinUnitInStock.
        /// </summary>
        [ValidateNullableUnitInStock]
        public string? MinUnitsInStock { get; set; }

        /// <summary>
        /// Return all Products with UnitInStock value less than MaxUnitInStock.
        /// </summary>
        [ValidateNullableUnitInStock]
        public string? MaxUnitsInStock { get; set; }

        /// <summary>
        /// Return all Products with UnitPrice value greater than MinUnitPrice.
        /// </summary>
        [ValidateNullableUnitPrice]
        public string? MinUnitPrice { get; set; }

        /// <summary>
        /// Return all Products with UnitPrice value less than MaxUnitPrice.
        /// </summary>
        [ValidateNullableUnitPrice]
        public string? MaxUnitPrice { get; set; }


        protected Dictionary<string, Tuple<string?, string?>> RangeFilterBy()
        {
            Dictionary<string, Tuple<string?, string?>> dictionary = new();
            dictionary["UnitsInStock"] = new Tuple<string?, string?>(MinUnitsInStock, MaxUnitsInStock);
            dictionary["UnitPrice"] = new Tuple<string?, string?>(MinUnitPrice, MaxUnitPrice);
            return dictionary;
        }
    }
}
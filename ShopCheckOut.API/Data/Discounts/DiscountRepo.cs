using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Discounts
{
    public class DiscountRepo : IDiscountRepo
    {
        private readonly MockData Dataset;
        public DiscountRepo()
        {
            Dataset = new MockData();

        }

        public Task AddNewDiscout(DiscountsModel discount, int productId)
        {
            Dataset._mockDiscounts.Add(discount);
            Dataset._mockProductDiscounts.Add(
                new ProductDiscountModel
                {
                    ProductId = productId,
                    DiscountId = discount.Id,
                    Product = Dataset._mockProducts.FirstOrDefault(p => p.Id == productId) ?? new ProductsModel(),
                    Discount = discount
                });

            return Task.CompletedTask;

        }

        public Task DeleteDiscount(int discountId)
        {
            var discount = Dataset._mockDiscounts.FirstOrDefault(d => d.Id == discountId);
            if (discount == null)
            {
                throw new KeyNotFoundException($"Discount {discountId} not found");
            }
            discount.IsActive = false;
            Dataset._mockProductDiscounts.RemoveAll(pd => pd.DiscountId == discountId);

            return Task.CompletedTask;
        }

        public Task<List<ProductDiscountModel>> GetAvailableDiscounts()
        {
            var productDict = Dataset._mockProducts.ToDictionary(p => p.Id);
            var discountDict = Dataset._mockDiscounts.ToDictionary(d => d.Id);

            var result = Dataset._mockProductDiscounts.Select(pd => new ProductDiscountModel
            {
                ProductId = pd.ProductId,
                DiscountId = pd.DiscountId,
                Product = productDict.TryGetValue(pd.ProductId, out var product) ? product : new ProductsModel(),
                Discount = discountDict.TryGetValue(pd.DiscountId, out var discount) ? discount : null
            }).ToList();

            return Task.FromResult(result);
        }

        public Task<PriceAfterDiscountReturn> PriceAfterDiscount(ProductsModel product, int quantity)
        {
            // Find all discounts applicable to the product
            var productDiscounts = Dataset._mockProductDiscounts
                .Where(pd => pd.ProductId == product.Id)
                .ToList();

            // If no discounts, return the original price
            if (productDiscounts.Count == 0)
            {
                return Task.FromResult(new PriceAfterDiscountReturn
                    (product.Price * quantity,
                    0,
                    null)
                );
            }

            // Get all active discounts for the product
            var discounts = Dataset._mockDiscounts
                .Where(d => productDiscounts.Any(pd => pd.DiscountId == d.Id) && d.IsActive && d.MinQuantity <= quantity)?
                .ToList();

            // Calculate the lowest price and the corresponding discount
            var originalPrice = product.Price * quantity;
            //var lowestPrice = originalPrice;
            var finalPrice = 0;
            string? highestDiscount = "";
            int totalSaved = 0;

            if (discounts?.Count == 1)
            {
                var discount = discounts[0];
                var discountedPrice = discount.ApplyDiscount(product.Price, quantity);

                if (discountedPrice.Price < originalPrice)
                {
                    finalPrice = discountedPrice.Price;
                    highestDiscount = discount.Name;
                    totalSaved = originalPrice - discountedPrice.Price;
                }
            }
            else if (discounts?.Count > 1)
            {
                TryDiscountCombinations(discounts, product.Price, quantity, ref finalPrice, ref highestDiscount, ref totalSaved);
            }

            if (highestDiscount == "")
            {
                highestDiscount = null;
            }
            return Task.FromResult(new PriceAfterDiscountReturn
               (finalPrice == 0 ? originalPrice : finalPrice,
                totalSaved,
                highestDiscount)
           );
        }

        private static void TryDiscountCombinations(
            List<DiscountsModel> discounts,
            int productPrice,
            int remainingQuantity,
            ref int finalPrice,
            ref string discoutNames,
            ref int totalSaved)
        {
            if (remainingQuantity == 0)
            {
                return;
            }
            var avaliableDiscounts = discounts.Where(d => d.GetMinQuantity() <= remainingQuantity).ToList();
            if (avaliableDiscounts.Count == 0)
            {
                finalPrice += productPrice * remainingQuantity;
                return;
            }

            var originalPrice = productPrice * remainingQuantity;
            var _discoutPrice = 0;
            var _remainingQuantity = remainingQuantity;
            var _discoutNames = "";
            var _totalSaved = 0;
            foreach (var discount in avaliableDiscounts)
            {
                var newPrice = discount.ApplyDiscount(productPrice, remainingQuantity);
                var newSaved = productPrice * remainingQuantity - newPrice.Price;
                if (newPrice.Price < originalPrice)
                {
                    _discoutPrice = newPrice.Price - newPrice.RemaindQuantity * productPrice; // only add the discount price
                    _discoutNames = discount.Name;
                    _totalSaved = newSaved;
                    _remainingQuantity = newPrice.RemaindQuantity;
                }
            }

            finalPrice += _discoutPrice;
            discoutNames += _discoutNames == "" ? "" : _discoutNames + "; ";
            totalSaved += _totalSaved;

            TryDiscountCombinations(
                discounts,
                productPrice,
                _remainingQuantity,
                ref finalPrice,
                ref discoutNames,
                ref totalSaved
                );
        }
    }
}

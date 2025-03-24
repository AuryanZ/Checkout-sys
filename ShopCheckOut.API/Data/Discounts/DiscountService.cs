using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Discounts
{
    public class DiscountService : IDiscountService
    {
        private readonly List<ProductDiscountModel> _mockProductDiscounts;
        private readonly List<DiscountsModel> _mockDiscounts;
        private readonly List<ProductsModel> _mockProducts;
        public DiscountService()
        {
            _mockProductDiscounts = new MockData().GetMockProductDiscounts();
            _mockDiscounts = new MockData().GetMockDiscounts();
            _mockProducts = new MockData().GetMockProducts();
        }

        public Task<bool> AddNewDiscout(DiscountsModel discount, int productId)
        {
            try
            {
                _mockDiscounts.Add(discount);
                _mockProductDiscounts.Add(
                    new ProductDiscountModel
                    {
                        ProductId = productId,
                        DiscountId = discount.Id,
                        Product = _mockProducts.FirstOrDefault(p => p.Id == productId),
                        Discount = discount
                    });

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> DeleteDiscount(int discountId)
        {
            try
            {
                var discount = _mockDiscounts.FirstOrDefault(d => d.Id == discountId);
                if (discount == null)
                {
                    throw new Exception($"Discount with ID {discountId} not found.");
                }
                var productDiscounts = _mockProductDiscounts.Where(pd => pd.DiscountId == discountId).ToList();
                foreach (var pd in productDiscounts)
                {
                    _mockProductDiscounts.Remove(pd);
                }
                discount.IsActive = false;

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public Task<List<ProductDiscountModel>> GetAvailableDiscounts()
        {
            var result = _mockProductDiscounts.Select(pd => new ProductDiscountModel
            {
                ProductId = pd.ProductId,
                DiscountId = pd.DiscountId,
                Product = _mockProducts.FirstOrDefault(p => p.Id == pd.ProductId),
                Discount = _mockDiscounts.FirstOrDefault(d => d.Id == pd.DiscountId)
            }).ToList();

            return Task.FromResult(result);
        }

        public Task<PriceAfterDiscountReturn> PriceAfterDiscount(ProductsModel product, int quantity)
        {
            // Find all discounts applicable to the product
            var productDiscounts = _mockProductDiscounts
                .Where(pd => pd.ProductId == product.Id)
                .ToList();

            // If no discounts, return the original price
            if (!productDiscounts.Any())
            {
                return Task.FromResult(new PriceAfterDiscountReturn
                    (product.Price * quantity,
                    0,
                    null)
                );
            }

            // Get all active discounts for the product
            var discounts = _mockDiscounts
                .Where(d => productDiscounts.Any(pd => pd.DiscountId == d.Id) && d.IsActive && d.minQuantity <= quantity)?
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

                if (discountedPrice.price < originalPrice)
                {
                    finalPrice = discountedPrice.price;
                    highestDiscount = discount.Name;
                    totalSaved = originalPrice - discountedPrice.price;
                }
            }
            else if (discounts?.Count > 1)
            {
                // get list of all possible combinations of discounts
                var discountCombinations = new List<List<DiscountsModel>>();
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
            var avaliableDiscounts = discounts.Where(d => d.minQuantity <= remainingQuantity).ToList();
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
                var newSaved = productPrice * remainingQuantity - newPrice.price;
                if (newPrice.price < originalPrice)
                {
                    _discoutPrice = newPrice.price - newPrice.remaindQuantity * productPrice; // only add the discount price
                    _discoutNames = discount.Name;
                    _totalSaved = newSaved;
                    _remainingQuantity = newPrice.remaindQuantity;
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

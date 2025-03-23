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

        public Task<(int priceAfterDiscount, int totalSaved, DiscountsModel? highesDiscount)> PriceAfterDiscount(ProductsModel product, int quantity)
        {
            // Find all discounts applicable to the product
            var productDiscounts = _mockProductDiscounts
                .Where(pd => pd.ProductId == product.Id)
                .ToList();

            // If no discounts, return the original price
            if (!productDiscounts.Any())
            {
                return Task.FromResult((product.Price * quantity, 0, (DiscountsModel?)null));
            }

            // Get all active discounts for the product
            var discounts = _mockDiscounts
                .Where(d => productDiscounts.Any(pd => pd.DiscountId == d.Id) && d.IsActive)
                .ToList();

            // Calculate the lowest price and the corresponding discount
            var originalPrice = product.Price * quantity;
            var lowestPrice = originalPrice;
            DiscountsModel? highestDiscount = null;
            int totalSaved = 0;

            foreach (var discount in discounts)
            {
                var discountedPrice = discount.ApplyDiscount(product.Price, quantity);
                if (discountedPrice < lowestPrice)
                {
                    lowestPrice = discountedPrice;
                    highestDiscount = discount;
                    totalSaved = originalPrice - discountedPrice;
                }
            }

            return Task.FromResult((lowestPrice, totalSaved, highestDiscount));
        }

    }
}

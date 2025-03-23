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

        public Task<(int priceAfterDiscount, DiscountsModel? highesDiscount)> PriceAfterDiscount(ProductsModel product, int quantity)
        {
            var productDiscounts = _mockProductDiscounts.Where(pd => pd.ProductId == product.Id).ToList();
            if (!productDiscounts.Any())
            {
                return Task.FromResult((product.Price, (DiscountsModel)null));
            }
            var discounts = _mockDiscounts
                            .Where(d => productDiscounts.Any(pd => pd.DiscountId == d.Id) && d.IsActive)
                            .ToList();
            var priceAfterDiscount = product.Price;

            // if multiple discounts are available, apply the one with the highest discount
            DiscountsModel? highestDiscount = null;
            var lowestPrice = product.Price * quantity;

            // Check all possible discounts and apply the one with the lowest price
            if (discounts.Count >= 1)
            {
                foreach (var discount in discounts)
                {
                    if (discount == null || quantity < discount.MinQuantity || discount.DiscountTiers == null)
                    {
                        continue;
                    }
                    var discountTiers = discount.DiscountTiers
                        .Where(dt => dt.Threshold <= quantity)
                        .ToList();
                    if (!discountTiers.Any())
                    {
                        continue;
                    }
                    foreach (var tier in discountTiers)
                    {
                        switch (discount.Type)
                        {
                            case "Percentage":
                                {
                                    int discountPrice = (product.Price * quantity) * (tier.Percentage ?? 100) / 100;
                                    if (discountPrice < lowestPrice)
                                    {
                                        lowestPrice = (int)discountPrice;
                                        highestDiscount = discount;
                                    }
                                    break;
                                }
                            case "Fixed Price":
                                {
                                    int fixedPriceItems = quantity / tier.Threshold;
                                    int remainingItems = quantity % tier.Threshold;
                                    int totalFixedPrice = (fixedPriceItems * (tier.FixedPrice ?? product.Price)) + (remainingItems * product.Price);

                                    if (totalFixedPrice < lowestPrice)
                                    {
                                        lowestPrice = totalFixedPrice;
                                        highestDiscount = discount;
                                    }
                                    break;
                                }
                            case "Free Item":
                                {
                                    int freeItems = quantity / tier.Threshold;
                                    int remainingItemsPrice = (quantity - freeItems) * product.Price;

                                    if (remainingItemsPrice < lowestPrice)
                                    {
                                        lowestPrice = remainingItemsPrice;
                                        highestDiscount = discount;
                                    }
                                    break;
                                }
                        }

                    }
                }
            }

            if (highestDiscount != null)
            {
                priceAfterDiscount = lowestPrice;
            }
            return Task.FromResult((priceAfterDiscount, highestDiscount));
        }

    }
}

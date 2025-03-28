# Checkout-sys

Project introduction:
http://codekata.com/kata/kata09-back-to-the-checkout/ 


## Work Roadmap
1. Complete the core functions
    - [x] get products by Category
    - [x] get product by SKu
    - [x] add product
    - [x] New order
    - [x] add order item
    - [x] Delete order item
    - [X] Checkout

2. Add Discount feature
    - [X] Add Discount
    - [X] Delete Discount
    - [X] Update Discount
    - [x] PriceAfterDiscount

3. Apply Discount
    - [x] Add totalSaved field in Order
    - [x] "Add order item" function should check if the discount is available for the product
    - [x] "Delete order item" function should check and remove the discount if the product is removed or the quantity is reduced

TODO:
- [ ] move process to service layer
    - Add item to order
    - Delete item from order
    - Apply discount 
- [ ] Add more test cases

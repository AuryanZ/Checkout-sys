# Checkout-sys

Project introduction:
http://codekata.com/kata/kata09-back-to-the-checkout/ 

## Data Design
![Data Design](https://github.com/AuryanZ/Checkout-sys/blob/feature/core-funcs/Imgs/ERD.png)

## Basic Workflow
![Basic Workflow](https://github.com/AuryanZ/Checkout-sys/blob/feature/core-funcs/Imgs/WorkFlow.png)

## Work Roadmap
1. Complete the core functions
    - [x] get products by Category
    ```
    GET /products?category=fruit
    Response:
    {
        "products": [
            {
                "sku": "A",
                "name": "Apple",
                "price": 1.0
            },
            {
                "sku": "B",
                "name": "Banana",
                "price": 0.5
            }
        ]
    }
    ```
    - [x] get product by SKu
    ```
    GET /products/A
    Response:
    {
        "sku": "A",
        "name": "Apple",
        "price": 1.0
    }
    ```
    - [x] add product
    ```
    POST /products
    Request:
    {
        "sku": "C",
        "name": "Cherry",
        "category": "fruit"
        ...
        "price": 2.0,
    }
    Response: 201 Created
    ```
    - [ ] add order
    ```
    POST /orders
    Request:
    {
       "Date": "2025-01-01",
       "Customer": "",
    }
    Response:
    {
        "OrderID": 1,
        "TotalAmount": 0.0,
    }
    ```
    - [ ] add order item
    ```
    POST /orders/1/items
    Request:
    {
        "OrderID": 1,
        "sku": "A"
        "Quantity": 4
    }
    Response: 
    {
        "OrderID": 1,
        "TotalAmount": 4.0,
    }
    ```
    - [ ] Delete order item
    ```
    DELETE /orders/1/items/A
    Response:
    {
        "OrderID": 1,
        "TotalAmount": 0.0,
    }
    ```
    - [ ] Checkout
    ```
    POST /orders/1/checkout
    Request:
    {
        "OrderID": 1
    }
    Response:
    {
        "OrderID": 1,
        "TotalAmount": 4.0,
    }
    ```
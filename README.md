# GrocyTransformingProxy

A proxy for the Grocy API that transforms a request to the `Stock/Products/{productId}/Add` endpoint so that if the `stock_label_type` is not included in the request, the default `stock_label_type` for that product will be added.

If further API transforms are required, then they will be added at a future date.

## Why

1. The `Stock/Products/{productId}/Add` API endpoint only requires one value, `amount`. Any other omitted value will be set to some default.
1. The Grocy.Mobile iOS app does not have a mechanism for sending the `stock_label_type`, so it is omitted from the request.

Therefore, a user of the Grocy Mobile iOS app who wants anything other than "No Label" for the `stock_label_type` when adding a stock item via the app is not able to accomplish the desired behavior.
This proxy rectifies that by modifying the request to have a value for `stock_label_type` when one is defined in the product 
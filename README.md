# fetchy-mcfetch-points

- This project was developed to solve Fetch's [coding challenge](https://github.com/fetch-rewards/receipt-processor-challenge).

## Implementation
- [Swagger documentation](https://fetchy-mcfetch-points.azurewebsites.net/)
  - See `Test` controller to see plain-text output of tests.
- API base URL relative to spec: `https://fetchy-mcfetch-points.azurewebsites.net/api`
  - Generate requests from the Swagger documentation or use a tool like [Postman](https://www.postman.com/) (don't forget to set your `Content-Type` as `application/json`).

## Assumptions

The challenge is open-ended in certain respects. I made these assumptions:
- Time is local to the server instance.
- A specific receipt can be submitted at most once.
- A receipt can be unique identified by the combination of the retailer, date and time of purchase, total, and items.
- The API returns a consistent response for the `receipts/process` and `/api/receipts/{id}/points` endpoints. Both return the `id` and `points` properties. The challenge specifies slimmer payloads but I'm assuming that extra properties are fine.

## Potential enhancements
- Normalize error payloads. Depending on the type of error, the shape of the returned value varies.
- Rework testing code; my no-library approach ended up pretty clunky :/
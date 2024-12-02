# fetchy-mcfetch-points

- This project was developed to solve Fetch's [coding challenge](https://github.com/fetch-rewards/receipt-processor-challenge).

## Implementation
- [Swagger documentation](https://fetchy-mcfetch-points.azurewebsites.net/)
- [API base endpoint](https://fetchy-mcfetch-points.azurewebsites.net/api/)
  - Use standard HTTP verbs to interact with the API
  - Generate requests from the Swagger documentation or use a tool like [Postman](https://www.postman.com/) (don't forget to set your `Content-Type` as `application/json`).

## Assumptions

The challenge is open-ended in certain respects. I made these assumptions:
- Time is local to the server instance.
- A specific receipt can be submitted at most once.
- A receipt can be unique identified by the combination of the retailer, date and time of purchase, total, and items.

## Potential enhancements
- Normalize error payloads. Depending on the type of error, the shape of the returned value varies.
- Clean up testing code; it ended up pretty clunky :/
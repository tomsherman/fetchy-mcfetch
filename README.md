# fetchy-mcfetch

- This project was developed to meet [this coding challenge](https://fetch-hiring.s3.us-east-1.amazonaws.com/points.pdf)

## Implementation
- [Swagger documentation](https://fetch-points20210919001315.azurewebsites.net/)
- [API base endpoint](https://fetch-points20210919001315.azurewebsites.net/api/points)
  - Use standard HTTP verbs to interact with the API
  - Generate requests from the Swagger documentation or use a tool like [Postman](https://www.postman.com/) (don't forget to set your `Content-Type` as `application/json`).

## API notes
- API follows the spirit of HTTP verbs:
  - `GET` returns current balances by payer
  - `POST` is used to spend points
  - `PUT` offers 2 idempotent methods:
    - Add a credit assigned to a given payer with a specific timestamp
    - Clear any existing data and populates example data provided in points.pdf
  - `DELETE` clears all points

## TODO
- More elegant error handling
- Tests

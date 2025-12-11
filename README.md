# PaymentApi (Clean Architecture minimal)

## ??????
1. docker compose up --build
2. Swagger: http://localhost:5000/swagger

## Endpoints
- POST /auth/register-test (??????? ????????? user/test123)
- POST /auth/login { login, password } -> { token }
- POST /auth/logout (Authorization: Bearer {token})
- POST /payment (Authorization: Bearer {token}) — ??????? 1.10 USD

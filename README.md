# PaymentApi

API для авторизации пользователей и выполнения платежей (баланс в USD).
Простая реализация с JWT-авторизацией, учётом сессий и атомарными транзакциями для списаний.

Стек:

- .NET 8 Web API

- PostgreSQL 15

- Docker + Docker Compose

## Быстрый старт

1. Склонируйте репозиторорию и перейдите в папку проекта:

- `git clone git@github.com:baiaman1/PaymentApi.git
- cd PaymentApi`


2. Соберите и запустите сервисы через Docker Compose:

- `docker-compose up --build`


3. После успешного запуска:

- API доступен по: http://localhost:5000

- Swagger UI: http://localhost:5000/swagger/index.html

# Endpoints

Все запросы, кроме */auth/login* и */users/register* — требуют заголовка:

Authorization: Bearer <JWT_TOKEN>

## Login

URL: POST /auth/login

Описание: Аутентификация пользователя. При успешном вводе логина/пароля возвращает JWT токен.

Тело (JSON):

{
  "login": "user1",
  "password": "password123"
}


Успешный ответ (200):

{
  "token": "<JWT_TOKEN>"
}


Ошибки:

401 — неверный логин или пароль

Защита от брутфорса: несколько неудачных попыток блокируют вход на некоторое время.

## Logout

URL: POST /auth/logout

Описание: Инвалидирует (отзывает) текущий токен — дальнейшие запросы с этим токеном будут отклоняться.

Заголовки:

Authorization: Bearer <JWT_TOKEN>


Успешный ответ (200):

{
  "message": "Logged out successfully"
}


Ошибки:

401 — если токен не передан, неверный или уже отозван.

## Payment

URL: POST /payment

Описание: Списывает с баланса конкретной сессии (привязано к токену) 1.10 USD. Операция атомарная — защищена транзакцией и блокировкой записи.

Заголовки:

Authorization: Bearer <JWT_TOKEN>


Успешный ответ (200):

{
  "balance": 6.90
}


При недостатке средств (400):

{
  "error": "Insufficient balance",
  "balance": 0.50
}


### Замечания:

Новый пользователь при добавлении получает стартовый баланс 8.00 USD. Если токен был отозван (через logout), платеж выполнить нельзя.

# Структура данных (кратко)

**Users** — содержит Id, Login, PasswordHash, Balance (decimal), FailedAttempts, LockedUntil и т.д.

**Sessions** — хранит Token, UserId, CreatedAt, IsRevoked (поддержка нескольких сессий).

**PaymentHistory** — запись каждого платежа: Id, UserId, Amount, Currency, CreatedAt.

# Полезные команды (локально)

Собрать и запустить:

`docker-compose up --build`


Остановить и удалить контейнеры:

`docker-compose down`


Посмотреть логи конкретного сервиса:

`docker-compose logs -f api`
`docker-compose logs -f db`


Выполнить команду в запущённом контейнере:

`docker-compose exec api bash`

# Примеры curl

### Login

curl -X POST "http://localhost:5000/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"login":"user1","password":"password123"}'


### Payment

curl -X POST "http://localhost:5000/payment" \
  -H "Authorization: Bearer <JWT_TOKEN>"


### Logout

curl -X POST "http://localhost:5000/auth/logout" \
  -H "Authorization: Bearer <JWT_TOKEN>"
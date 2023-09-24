# Project `white-pages`

[![License](https://img.shields.io/badge/License-BSD_3--Clause-blue.svg)](https://opensource.org/licenses/BSD-3-Clause) [![Continuous Integration Pipeline](https://github.com/zatarain/white-pages/actions/workflows/api.yml/badge.svg)](https://github.com/zatarain/white-pages/actions/workflows/api.yml) [![codecov](https://codecov.io/gh/zatarain/white-pages/graph/badge.svg?token=55VMMF1IUP)](https://codecov.io/gh/zatarain/white-pages)

This project aims to be an API to manage customer contacts and their addresses. The project is an exercise to discuss about software engineering technical topics like software development, testing,  continuos integration, possible deployment, etcetera. More specifically, to discuss the development of an [API (Application Programming Interface)][what-is-api] to **manage customer addresses** written in [.NET Core with C#][dotnet-core]. The name of the whole solution is `White Pages` after the old fashion directory book, so this might be `The Modern White Pages`.

## 📂 Table of content

* 📹 [Overview](#-overview)
  - ☑️ [Requirements](#☑️-requirements)
  - 🤔 [Assumptions](#-assumptions)
* 📐 [Design](#-design)
  - 📊 [Data model](#-data-model)
    * 👤 [Customer](#-customer)
    * ✍🏽 [Address](#-address)
  - 🔚 [End-points](#-end-points)
* 🏗️ [Implementation details](#-implementation-details)
  - 📦 [Dependencies](#-dependencies)
  - 🗄️ [Storage](#-storage)
* ⏯️ [Running](#-running)
  - 🍏 [Development Mode](#-development-mode)
  - 🍎 [Production Mode](#-production-mode)
* ✅ [Testing](#-testing)
  - 🧪 [Manual](#-manual)
  - ♻️ [Automated](#-automated)
  - 💯 [Coverage](#-coverage)
* 📚 [References](#-references)

## 📹 Overview

This simple API aims to manage a customer addresses database, this means we will have a collection of customers and we will be able to add their addresses. The application should be ready to deploy as a [Docker][docker] container, so we need to generate an image for it available to download in [Docker Hub][docker-hub].

### ☑️ Requirements

The API should be able to manage a database for the customers and each customer may have many addresses related to it. Allowing the client to perform following operations:

* **List all the customers.** It should return the list of all customers in the system.
* **Create a customer.** It should insert a new record for customer provided by the client. However, a customer can only exists once within the database.
* **Deactivate a customer.** It should allow the client to update the status of a given customer to inactive.
* **Delete a customer.** It should allow the client to delete a customer from the system and its addresses.
* **Activate a customer.** It should allow the client to update the status of a given customer to inactive.
* **List only active customers.** It should return the list of active customers in the system.
* **View customer details.** It should show the details of a customer provided by the client and its addresses.
* **Add a new address to a customer.** It should create a new address record for a customer. A customer may have multiple addresses.
* **Delete addresses.** The API should provide a mechanism to delete addresses, but cannot delete the last one, a customer MUST have at least one address.

### 🤔 Assumptions

This just is a small example and either consider some assumptions and/or it's not taking care about some corner case scenarios like following:

* The data transfer objects for the input of all the request and successful responses is JSON or empty.
* The environment variables and secrets (e. g. `POSTGRES_USERNAME` to connect to the database) for API configuration are set in the host machine (see [Running section](#-running) below for more information). In the real world the secrets should be stored and provisioned by an external system (e. g. AWS Secret Manager).
* The storage will be a [PostgreSQL][postgresql] relational database using a [Docker][docker] container with a volume for development.
* The customer and address records are created via separated requests for different end-points. So, when we created a customer is assumed active and without any address.
* In order to create an address we send a URL parameter for `customerId` to the end-point, in the reality this should be taken from the user who is currently authorised via some mechanism like [JSON Web Token][what-is-jwt] (which is not implemented) and sends the request.
* When the main address for a customer is deleted, some of the secondary addresses of the customer will become the new main address, but this one is arbitrarily selected (like just the first result of the query).
* In order to determine whether a customer already exists in the database, we will distinct them based-on Email and Phone fields.
* We will use ALPHA ISO codes for the country. Currently only 3 countries are supported United Kingdom (`GB`), Mexico (`MX`), United Stated (`US`).
* The validation of the postcodes are based on a simple regular expression check depending of the country where is the address, the system doesn't check if the postcode actually exists.
* Even that the API is using asynchronous tasks, we are not using [ACID Database Transactions][acid-transactions], so there could be multi-threading issues like inconsistent data.

## 📐 Design

The architecture will be a HTTP API for a microservice that will consume some configuration and use ORM (via [Entity Framework][ef-docs]) to represent the records in the database tables and also a Model-Controller (MC) pattern design, so the controllers will contain the handlers for the API requests, while the entity models will represent the data. The service will be stateless, so we won't hold any state (e. g. session management) on the server side, instead is designed to be an asynchronous service and use authorisation tokens.

### 📊 Data model

In order to store and manipulate the data needed the API will rely on the entities shown in following diagram:

```mermaid
erDiagram
  Customer {
    Id integer PK
    MainAddressId integer FK
    Title character_varying(20)
    Forename character_varying(50)
    Surname character_varying(50)
    Email character_varying(75)
    Phone character_varying(15)
    IsActive boolean
    CreatedAt timestamp
    LastUpdatedAt timestamp
  }

  Address {
    Id integer PK
    CustomerId integer FK
    Line1 character_varying(80)
    Line2 character_varying(80)
    Town character_varying(50)
    County character_varying(50)
    Postcode character_varying(10)
    Country character_varying(2)
    CreatedAt timestamp
    LastUpdatedAt timestamp
  }

  Address }o--|| Customer: "has many Addresses"
  Customer }|--|{ Address: "has a Main"

```

As we can see in the diagram, a `Customer` _may has_ many `Address`es, which is made possible with the foreign key `CustomerId` within the `Address` entity. Then, a `Customer` _has_ a single `MainAddress`s thanks to the foreign key `MainAddressId`.

The API manages the persistency of the data with a 🐘 [PostgreSQL][postgresql] database, using a 🐳 Docker container with a volume. So, the records for entities shown in the diagram will be stored as rows in tables. PostgreSQL manages a [wide range of data types][postgresql-data-types] so, in order to keep the things simple we will use data type autogenerated by Entity Framework .NET tool for migrations.

#### 👤 Customer

This entity will represent the customers in the system and each record will be stored in the table `Customers` which has following fields:

| ⏹️ | Name            |     Type        | Description                                                |
|:--:| :---            |    :----:       | :---                                                       |
| 🗝️ | `Id`            | `INTEGER`       | Auto-sequence identifier for the customer                  |
| ✳️ | `MainAddressId` | `INTEGER`       | Foreign key for the main address of the customer           |
| 🔤 | `Title`         | `CHARACTER(20)` | **Mandatory**. Title of the customer                       |
| 🔤 | `Forename`      | `CHARACTER(50)` | **Mandatory**. Customer forename                           |
| 🔤 | `Surname`       | `CHARACTER(50)` | **Mandatory**. Customer surname                            |
| 🔤 | `Email`         | `CHARACTER(75)` | **Mandatory**. Email of the customer                       |
| 🔤 | `Phone`         | `CHARACTER(15)` | **Mandatory**. Mobile phone number of the customer         |
| 🗓️ | `CreatedAt`     | `TIMESTAMP`     | Timestamp with time zone representing the creation time    |
| 🗓️ | `LastUpdatedAt` | `TIMESTAMP`     | Timestamp with time zone representing the last update time |

#### 📌 Address

This entity will represent the addresses of the customers in the system and each record will be stored in the table `Addresses` which has following fields:

| ⏹️ | Name            |     Type        | Description                                                |
|:--:| :---            |    :----:       | :---                                                       |
| 🗝️ | `Id`            | `INTEGER`       | Auto-numeric identifier for the address                    |
| ✳️ | `CustomerId`    | `INTEGER`       | Foreign key for the customer                               |
| 🔤 | `Line1`         | `CHARACTER(80)` | **Mandatory**. First line of the address                   |
| 🔤 | `Line2`         | `CHARACTER(80)` | An optional second line for the address                    |
| 🔤 | `Town`          | `CHARACTER(50)` | **Mandatory**. Town, city, village where is the address    |
| 🔤 | `County`        | `CHARACTER(50)` | County or state where is the address                       |
| 🔤 | `Postcode`      | `CHARACTER(10)` | **Mandatory**. A valid postcode based on the country       |
| 🔤 | `Country`       | `CHARACTER(2)`  | **Mandatory**. Country ISO ALPHA-2 code (`GB`, `MX`, `US`) |
| 🗓️ | `CreatedAt`     | `TIMESTAMP`     | Timestamp with time zone representing the creation time    |
| 🗓️ | `LastUpdatedAt` | `TIMESTAMP`     | Timestamp with time zone representing the last update time |

### 🔚 End-points

The input for all the API end-points will be always in JSON format and in most of the cases and the output will be in the same format. The end-points for the API are described in following table:

| Method   | Address                     | Description                            | Success Status | Possible Failure Status            |
| :---:    | :---                        | :----                                  | :---:          | :---                               |
| `HEAD`   | `/health`                   | Service health check                   | `200 OK`       | `*Any*`                            |
| `GET`    | `/customers`                | List of all customers                  | `200 OK`       | `*Any*`                            |
| `GET`    | `/customers/only-active`    | List of active customers               | `200 OK`       | `*Any*`                            |
| `GET`    | `/customers/:id`            | Get customer details and its addresses | `200 OK`       | `404 Not Found`                    |
| `POST`   | `/customers`                | Create a customer record in the system | `200 Created`  | `400 Bad Request`                  |
| `PATCH`  | `/customers/:id/deactivate` | Deactivate a given customer            | `200 OK`       | `404 Not Found`                    |
| `PATCH`  | `/customers/:id/activate`   | Activate a given customer              | `200 OK`       | `404 Not Found`                    |
| `DELETE` | `/customers/:id`            | Delete a customer and its addresses    | `200 NoContent`| `404 Not Found`                    |
| `POST`   | `/addresses/:customerId`    | Create a address record for a customer | `200 Created`  | `400 Bad Request`, `404 Not Found` |
| `GET`    | `/addresses/:id`            | Get details for an address             | `200 OK`       | `404 Not Found`                    |
| `DELETE` | `/addresses/:id`            | Delete an address                      | `200 NoContent`| `400 Bad Request`, `404 Not Found` |

## 🏗️ Implementation details

We are using C# as programming language for the implementation of the API operations. And the database is a container in Postgres to stored locally in development and test environments.

There is a [continuous integration workflow][ci-cd-pipeline] that runs in [GitHub Actions][github-actions] which is responsible to build the API, run the database migrations, unit tests and integration tests, then it generates coverage report.

## 📚 References

* [.NET Core Documentation][dotnet-core]
* [PostgreSQL data types][postgresql-data-types]
* [Entity Framework Documentation][ef-docs]
* [xUnite Documentation][xunit-docs]
* [Moq Documentation][moq-docs]
* [GitHub Actions Documentation][github-actions-docs]

---

[what-is-api]: aws.amazon.com/what-is/api
[what-is-jwt]: https://jwt.io/introduction
[docker]: https://www.docker.com
[docker-hub]: https://hub.docker.com
[white-pages-repo]: https://github.com/zatarain/white-pages
[dotnet-core]: https://learn.microsoft.com/en-gb/dotnet/core/introduction
[postgresql]: https://www.postgresql.org/docs/
[postgresql-data-types]: https://www.postgresql.org/docs/current/datatype.html
[ef-docs]: https://learn.microsoft.com/en-gb/ef/
[moq-docs]: https://github.com/devlooped/moq
[xunit-docs]: https://xunit.net/#documentation
[github-actions]: https://github.com/features/actions
[github-actions-docs]: https://docs.github.com/en/actions
[codecov-whitepages]: https://app.codecov.io/gh/zatarain/white-pages
[codecov-sunburst]: https://codecov.io/gh/zatarain/white-pages/branch/main/graphs/sunburst.svg?token=55VMMF1IUP
[codecov-grid]: https://codecov.io/gh/zatarain/white-pages/branch/main/graphs/tree.svg?token=55VMMF1IUP
[codecov-icicle]: https://codecov.io/gh/zatarain/white-pages/branch/main/graphs/icicle.svg?token=55VMMF1IUP
[whitepages-actions]: https://github.com/zatarain/white-pages/actions
[ci-cd-pipeline]: https://github.com/zatarain/white-pages/actions/workflows/api.yml
[compose-yml]: https://github.com/zatarain/white-pages/blob/main/docker-compose.yml
[postman-website]: https://www.postman.com
[acid-transactions]: https://en.wikipedia.org/wiki/ACID
[data-driven-testing]: https://en.wikipedia.org/wiki/Data-driven_testing
[test-data-json]: https://github.com/zatarain/white-pages/tree/main/test
[sqlite-viewer]: https://marketplace.visualstudio.com/items?itemName=qwtel.sqlite-viewer

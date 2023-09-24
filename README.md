# Project `white-pages`

[![License](https://img.shields.io/badge/License-BSD_3--Clause-blue.svg)](https://opensource.org/licenses/BSD-3-Clause) [![Continuous Integration Pipeline](https://github.com/zatarain/white-pages/actions/workflows/api.yml/badge.svg)](https://github.com/zatarain/white-pages/actions/workflows/api.yml) [![codecov](https://codecov.io/gh/zatarain/white-pages/graph/badge.svg?token=55VMMF1IUP)](https://codecov.io/gh/zatarain/white-pages)

This project aims to be an API to manage customer contacts and their addresses. The project is an exercise to discuss about software engineering technical topics like software development, testing,  continuos integration, possible deployment, etcetera. More specifically, to discuss the development of an [API (Application Programming Interface)][what-is-api] to **manage customer addresses** written in [.NET Core with C#][dotnet-core]. The name of the whole solution is `White Pages` after the old fashion directory book, so this might be `The Modern White Pages`.

## 📂 Table of content

* 📹 [Overview](#📹-overview)
  - ☑️ [Requirements](#-requirements)
  - 🤔 [Assumptions](#-assumptions)
* 📐 [Design](#-design)
  - 📊 [Data model](#-data-model)
    * 🎞️ [Video](#-video)
    * ✍🏽 [Annotation](#-annotation)
    * 👤 [User](#-user)
  - 🔀 [Workflows](#-workflows)
    * 🔀 [User sign up](#-user-sign-up)
    * 🔀 [User login](#-user-login)
    * 🔀 [Authorised requests](#-authorised-requests)
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
* **Delete annotations.** The API should provide a mechanism to delete addresses, but cannot delete the last one, a customer MUST have at least one address.

### 🤔 Assumptions

This just is a small example and either consider some assumptions and/or it's not taking care about some corner case scenarios like following:

* The data transfer objects for the input of all the request and successful responses is JSON or empty.
* The environment variables and secrets (e. g. `POSTGRES_USERNAME` to connect to the database) for API configuration are set in the host machine (see [Running section](#-running) below for more information). In the real world the secrets should be stored and provisioned by an external system (e. g. AWS Secret Manager).
* The storage will be a [PostgreSQL][postgresql] relational database using a [Docker][docker] container with a volume for development.
* The customer and address records are created via separated requests for different end-points. So, when we created a customer is assumed active and without any address.
* In order to create an address we send a URL parameter for `customerId` to the end-point, in the reality this should be taken from the user who is currently authorised via some mechanism like [JSON Web Token][what-is-jwt] (which is not implemented) and sends the request.
* When the main address for a customer is deleted, some of the secondary addresses of the customer will become the new main address, but this one is arbitrarily selected (like just the first result of the query).

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
[white-pages-image]: https://hub.docker.com/repository/docker/zatarain/white-pages/tags
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
[ci-cd-pipeline]: https://github.com/zatarain/white-pages/blob/main/.github/workflows/pipeline.yml
[compose-yml]: https://github.com/zatarain/white-pages/blob/main/compose.yml
[postman-website]: https://www.postman.com
[acid-transactions]: https://en.wikipedia.org/wiki/ACID
[data-driven-testing]: https://en.wikipedia.org/wiki/Data-driven_testing
[test-data-json]: https://github.com/zatarain/white-pages/tree/main/test
[sqlite-viewer]: https://marketplace.visualstudio.com/items?itemName=qwtel.sqlite-viewer

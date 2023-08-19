# Hubtel.eCommerce.Cart.Api :shopping_cart:

This repository contains the startup configuration for the Hubtel eCommerce Cart API. It configures various services, middlewares, and components required for the API to function properly.

## Table of Contents

- :memo: [Introduction](#introduction)
- :computer: [Installation](#installation)
- :wrench: [Configuration](#configuration)
- :rocket: [Usage](#usage)
- :raising_hand: [Contributing](#contributing)
- :page_with_curl: [License](#license)

## Introduction

The Hubtel.eCommerce.Cart.Api project is responsible for setting up and configuring the necessary services, authentication, and routing for the Hubtel eCommerce Cart API. It leverages various libraries and components to achieve its functionality.

## Installation

1. :octocat: Clone this repository to your local machine.
2. :wrench: Open the project using your preferred development environment (Visual Studio, Visual Studio Code, etc.).
3. :package: Make sure you have the required dependencies installed, such as .NET Core SDK and other referenced packages.
4. :gear: Configure the connection string in the `appsettings.json` or `appsettings.Development.json` file to point to your desired database.

## Configuration

The configuration of the Hubtel.eCommerce.Cart.Api project involves several steps to set up various services, database connections, and authentication providers. Key configuration areas include:

- **Database Connection:** Set up the database connection using the connection string provided in the `appsettings.json` file.
- **Identity Services:** Configure identity settings, including password requirements, lockout settings, and token providers.
- **Entity Framework:** Configure the Entity Framework database context and migrations assembly.
- **Caching:** Add caching services for improved performance.
- **Swagger Documentation:** Set up Swagger and SwaggerUI for API documentation in development.
- **Authentication:** Configure JWT Bearer authentication for securing API endpoints.
- **Authorization:** Configure authorization policies for different user roles.

## Usage

1. :package: Install the required dependencies and configure the connection string as mentioned in the [Installation](#installation) section.
2. :rocket: Run the application using your preferred development environment or command line tool.
3. :book: During development, the SwaggerUI documentation can be accessed at `/swagger` to explore and test the available API endpoints.
4. :shopping_bags: The API exposes various endpoints related to eCommerce cart functionality.

## Contributing

Contributions to this project are welcome. If you find a bug or want to suggest an enhancement, feel free to open an issue or submit a pull request. Please ensure that your contributions align with the coding standards and conventions used in the project.

## License

This project is licensed under the [MIT License](LICENSE), which means you can use, modify, and distribute the code as long as you retain the original license and copyright notice.

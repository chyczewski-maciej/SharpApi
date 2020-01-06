# SharpApi

SharpApi is an application that allows you to host REST API based on simple yaml configuration file.

## Who is this for?

SharpApi is intended for developers that need a simplistic, out-of-the-box and easily configurable REST API for prototyping.

## Supported databases

Currently only in-memory storage is supported. All data will be lost on application exit.

## Getting started

In order to start SharpApi you need to start **SharpApi.exe** with a path to your configuration file as an argument.

Here's an example:

```console
./SharpApi.exe -c api.yaml
```

## Configuration file

Configuration file is used to determine the shape of your REST API.
Each endpoint is represented by following properties:

- Name
- Supported HTTP actions
- Fields and their types
- Key - indicates which of the fields should be the primary key of the object

Supported field types: **Boolean**, **Float**, **Integer**, **String**
Supported HTTP actions: **DELETE**, **GET**, **HEAD**, **POST**, **PUT**

</br>
Example:

```yaml
Documents:
- Name: Products
  Actions:
    - HEAD
    - GET
    - POST
    - DELETE
    - PUT
  Key: Id
  Fields:
  - Name: Id
    Type: Integer
  - Name: Name
    Type: String
  - Name: Price
    Type: Integer
  - Name: OnSale
    Type: Boolean
- Name: Customers
  Actions:
    - GET
    - POST
  Key: SSN
  Fields:
  - Name: SSN
    Type: Integer
  - Name: Name
    Type: String
  - Name: Surname
    Type: String
  ```
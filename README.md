# Infra.Template

## Overview
This repo is a template microservice project for Infra.NET infrastructure framework [Infra.NET](https://github.com/HassanHashemi/infra)

## Project Structure
Here is the hierarchical structure of the project:

    infra.template/
    ├── .editorconfig
    ├── .gitignore
    ├── CHANGELOG.md
    ├── README.md
    ├── deploy_local.sh
    ├── test_local_new.sh
    ├── campaign-prod.yml
    ├── campaign-stage.yml
    ├── Campaign.sln
    └── src/
        ├── Campaign.Api/
        │   ├── Campaign.Api.csproj
        │   ├── Controllers
        │   ├── helm
        │   ├── appsettings.json
        │   ├── Dockerfile
        │   └── ...
        ├── Campaign.Data/
        │   ├── Campaign.Data.csproj
        │   └── ...
        ├── Campaign.Domain/
        │   ├── Campaign.Domain.csproj
        │   ├── Entities
        │   ├── DomainEvents
        │   ├── Enums
        │   └── ...
        ├── Campaign.Handlers/
        │   ├── Campaign.Handlers.csproj
        │   ├── Commands
        │   ├── Queries
        │   ├── EventHandlers   
        │   └── ...
        ├── Campaign.Integration/
        │   ├── Campaign.Integration.csproj
        │   └── ...
        └── ...

## Installation
Just clone this repo and start your project!

## License
This repo has free license ;)
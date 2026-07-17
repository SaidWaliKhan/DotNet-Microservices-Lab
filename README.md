# 🚀 Microservices Learning Lab

![.NET](https://img.shields.io/badge/.NET-8%2B-purple)
![CSharp](https://img.shields.io/badge/C%23-Backend-blue)
![Docker](https://img.shields.io/badge/Docker-Containerization-blue)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Messaging-orange)
![Redis](https://img.shields.io/badge/Redis-Caching-red)
![Architecture](https://img.shields.io/badge/Architecture-Microservices-green)

A practical **.NET / C# / ASP.NET Core microservices learning laboratory** focused on designing, building, and understanding **production-style distributed systems**.

This repository is not a single application.

It is an **umbrella workspace containing multiple independent microservices projects**, where each project explores a specific architecture pattern, technology, or real-world backend problem.

Every project is:

- ✅ Self-contained
- ✅ Independently runnable
- ✅ Dockerized
- ✅ Documented
- ✅ Designed for learning and extension


---

# 🎯 Purpose

The purpose of this repository is to move beyond theory and build practical experience with the technologies and patterns used in modern backend systems.

Each project represents a real-world microservices concept:

- How services communicate
- How data consistency is handled
- How failures are managed
- How systems scale
- How applications are monitored
- How production systems are designed


The goal is not only to write working code, but to understand:

- Why a pattern exists
- When to use it
- What problem it solves
- What trade-offs it introduces


---

# 🏗️ Repository Philosophy

Modern backend systems are not built as a single API.

They are composed of independent services communicating through reliable and scalable patterns.

This repository follows the journey:

```
Monolith Thinking

        ↓

Clean Architecture

        ↓

Modular Applications

        ↓

Microservices

        ↓

Event Driven Systems

        ↓

Cloud Native Applications

        ↓

Production Distributed Systems
```


---

# 🗺️ Learning Roadmap

The repository is organized as a progressive learning path.


## 🟢 Level 1 — Foundations

Learn the basics of building production-ready services.

Projects:

| Project | Concepts |
|---|---|
| Authentication Service | JWT, Identity, Refresh Tokens |
| Product Service | REST API, EF Core, SQL Server |
| Clean Architecture API | DDD, CQRS, MediatR |


---

## 🟡 Level 2 — Service Communication

Learn how services communicate with each other.

Projects:

| Project | Concepts |
|---|---|
| REST Communication Demo | HTTP communication |
| gRPC Communication Demo | High performance communication |
| RabbitMQ Demo | Message queues, events |


---

## 🟠 Level 3 — Distributed Systems

Learn patterns used in real microservices.

Projects:

| Project | Concepts |
|---|---|
| Order Processing System | Event Driven Architecture |
| Notification Service | Background Workers |
| Saga Pattern Demo | Distributed Transactions |
| Outbox Pattern Demo | Reliable Messaging |


---

## 🔴 Level 4 — Production Ready Systems

Advanced projects.

Projects:

| Project | Concepts |
|---|---|
| E-Commerce Microservices | Complete distributed system |
| API Gateway | YARP, Routing |
| Observability Platform | Logging, Metrics, Tracing |
| Kubernetes Deployment | Cloud Native Architecture |


---

# 📚 Core Areas Covered

## 🏛️ Architecture & Design Patterns

- Microservices Architecture
- Service Decomposition
- Bounded Contexts
- Clean Architecture
- Domain-Driven Design (DDD)
- CQRS
- Event Sourcing
- API Gateway Pattern
- Backend For Frontend (BFF)
- Saga Pattern
- Outbox Pattern


---

# 🔄 Communication

Different projects explore multiple communication styles:

### Synchronous Communication

- REST APIs
- gRPC
- HTTP Clients


### Asynchronous Communication

- RabbitMQ
- Kafka
- Publish / Subscribe
- Event Driven Architecture


### Real-Time Communication

- SignalR
- WebSockets


---

# 💾 Data & Persistence

Topics explored:

- Database per Service
- Entity Framework Core
- SQL Server
- PostgreSQL
- NoSQL Databases
- Redis Distributed Cache
- Data Consistency Strategies
- Transactions


---

# 🛡️ Reliability & Resilience

Production systems fail.

These projects explore:

- Retry Policies
- Circuit Breakers
- Timeout Handling
- Polly
- Health Checks
- Graceful Degradation
- Rate Limiting
- Fault Handling


---

# 🐳 Infrastructure & DevOps

Every project is designed with a **container-first approach**.

Each microservice contains its own:

- Dockerfile
- Docker Compose configuration
- Environment configuration
- Required dependencies


Example:

```
Order-Service

        |
        |
Docker Container

        |
        |
RabbitMQ Container

        |
        |
SQL Server Container
```


## Why Docker?

Because every project can run consistently on any machine:

```
Developer Machine

        ↓

Docker Compose

        ↓

Same Environment Everywhere
```


No matter if you use:

- Windows
- Linux
- macOS


The project runs the same way.


Example:

```bash
git clone repository

cd project-folder

docker compose up --build
```


The entire environment starts automatically.


---

# 🔐 Security

Security concepts explored:

- JWT Authentication
- Authorization
- Role Based Access Control
- OAuth 2.0
- OpenID Connect
- Secure Service Communication
- API Security Practices


---

# 📊 Observability

Modern systems need visibility.

Projects explore:

## Logging

- Serilog
- Structured Logging


## Metrics

- Prometheus
- Grafana


## Distributed Tracing

- OpenTelemetry


Example:

```
Request

 ↓

API Gateway

 ↓

Order Service

 ↓

Payment Service

 ↓

Notification Service


Complete trace available
```


---

# 🛠️ Technology Stack


## Backend

- C#
- .NET 8+
- ASP.NET Core Web API
- Entity Framework Core
- MediatR
- FluentValidation


## Databases

- SQL Server
- PostgreSQL
- Redis
- MongoDB


## Messaging

- RabbitMQ
- Kafka


## Infrastructure

- Docker
- Docker Compose
- Kubernetes


## Gateway

- YARP
- Ocelot


## Monitoring

- Serilog
- OpenTelemetry
- Prometheus
- Grafana


---

# 📂 Repository Structure


```
Microservices-Learning-Lab

│
├── README.md
│
├── projects
│
│   ├── 01-authentication-service
│   │       ├── src
│   │       ├── docker-compose.yml
│   │       └── README.md
│   │
│   ├── 02-product-service
│   │       ├── src
│   │       ├── docker-compose.yml
│   │       └── README.md
│   │
│   ├── 03-order-service
│   │       ├── src
│   │       ├── docker-compose.yml
│   │       └── README.md
│
│
├── docs
│
│   ├── diagrams
│   ├── architecture-notes
│   └── learning-notes
│
└── LICENSE
```


---

# 🚀 Running Projects


Every project contains its own instructions.

General workflow:


Clone repository:

```bash
git clone <repository-url>
```


Navigate:

```bash
cd projects/project-name
```


Start everything:

```bash
docker compose up --build
```


Docker will start:

- API services
- Databases
- Message brokers
- Required infrastructure


---

# 📈 Learning Progress


## Completed ✅

- REST API Architecture
- Clean Architecture
- Dependency Injection
- JWT Authentication
- Entity Framework Core
- Redis Caching
- RabbitMQ Messaging


## Currently Learning 🚧

- Something new ....


## Future Goals 📌

- Kubernetes Deployment
- Cloud Hosting
- Event Sourcing
- Service Mesh
- Advanced System Design


---

# 🤝 Contribution

This repository is mainly created for learning and experimentation.

Suggestions, discussions, and improvements are welcome.


---

# 📄 License

This repository is maintained for educational purposes.

Feel free to explore, learn, and use these examples as references.

---

# ⭐ Final Goal

Build a strong understanding of how real-world backend systems are designed:

```
Simple API

   ↓

Clean Architecture

   ↓

Distributed Services

   ↓

Event Driven Systems

   ↓

Cloud Native Applications

   ↓

Production Grade Backend Engineering
```

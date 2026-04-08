# InvoiceAPI 💸📄

<p align="center">
  <em>Gerenciamento de faturas com arquitetura em camadas, API REST e cálculo automatizado de valores.</em>
</p>

<p align="center">
  <a href="#-visão-geral">Visão Geral</a> •
  <a href="#-arquitetura">Arquitetura</a> •
  <a href="#-como-rodar">Como Rodar</a> •
  <a href="#-endpoints">Endpoints</a> •
  <a href="#-relatórios">Relatórios</a>
</p>

---

## ✨ Visão Geral

> A **InvoiceAPI** é uma API interna projetada para centralizar e organizar o fluxo de emissão, controle e consulta de faturas em um sistema financeiro.

> O projeto simula um cenário real de backoffice, onde múltiplas operações precisam ser consistentes, auditáveis e confiáveis desde o cadastro de itens até o cálculo final da fatura.

> Mais do que apenas CRUD, a API aplica regras de negócio essenciais para garantir integridade dos dados e previsibilidade no cálculo financeiro, como:

- cálculo automático de valores por item;
- agregação do valor total da fatura;
- controle de status da fatura;
- padronização do fluxo de criação e atualização.

> O objetivo é demonstrar uma estrutura sólida de backend, com foco em **arquitetura limpa**, **manutenibilidade** e **separação de responsabilidades**.

---

## 🧱 Arquitetura

A solução foi organizada em camadas para isolar responsabilidades e facilitar evolução:

- **Invoice.API.Internal**
  - Host da API, controllers, Swagger e configurações web
- **Invoice.API.Application**
  - Serviços de aplicação e regras de negócio
- **Invoice.API.Domain**
  - Entidades centrais e contratos
- **Invoice.API.Database**
  - `DbContext`, repositórios e migrations (EF Core)
- **ServiceCollectionExtensions**
  - Registro de dependências (IoC)
- **Invoice.Shared / DbConfig**
  - Configurações e modelos compartilhados

---

## 🛠️ Stack Tecnológica

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **Swagger (OpenAPI)**
- **QuestPDF** *(planejado / em integração)*

---

## 🚀 Como Rodar

### Pré-requisitos

- SDK do **.NET 8+**
- Banco de dados configurado
- (Opcional) CLI do Entity Framework

---

### 1) Restaurar e buildar

```bash
dotnet restore
dotnet build

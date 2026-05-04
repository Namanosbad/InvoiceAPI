# InvoiceAPI 💸📄

<p align="center">
  <em>API REST para gerenciamento de faturas com regras de negócio centralizadas, cálculo automático e arquitetura em camadas.</em>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512bd4?style=flat-square&logo=dotnet">
  <img src="https://img.shields.io/badge/Entity%20Framework-Core-512bd4?style=flat-square">
  <img src="https://img.shields.io/badge/SQL%20Server-Database-red?style=flat-square&logo=microsoft-sql-server">
  <img src="https://img.shields.io/badge/Architecture-Clean-green?style=flat-square">
</p>

---

## 📷 Preview

<table align="center">
  <tr>
    <td align="center">
      <strong>Exemplo de Fatura</strong><br/>
      <img src="https://github.com/user-attachments/assets/113bdd5a-cf39-4b47-9358-248b3d4053bd" width="700"/>
    </td>
    <td align="center">
      <strong>Swagger / Endpoints</strong><br/>
      <img src="https://github.com/user-attachments/assets/7ee70d92-afb5-47b3-84c4-ad11326d9e64" width="950"/>
    </td>
  </tr>
</table>

---

## 🎯 Visão Geral

A **InvoiceAPI** é uma API backend para gerenciamento do ciclo completo de faturas em cenários financeiros.

O projeto foi desenvolvido com foco em:

- consistência de dados  
- previsibilidade de cálculos  
- separação de responsabilidades  
- manutenção e escalabilidade  

Toda a lógica crítica é centralizada fora dos controllers, garantindo controle e previsibilidade no domínio.

---

## ⚙️ Funcionalidades

- gestão completa de faturas  
- gerenciamento de itens  
- cálculo automático de valores  
- agregação do total da fatura  
- controle de status (aberta, fechada, paga)  
- base preparada para geração de PDF (QuestPDF)  

---

## 🧠 Decisões Técnicas

- regras de negócio centralizadas na camada de aplicação/domínio  
- controllers responsáveis apenas pela camada HTTP  
- domínio desacoplado da infraestrutura  
- uso de Entity Framework Core com migrations  
- injeção de dependência para baixo acoplamento  

---

## 🔄 Fluxo de Exemplo

**Criação de uma fatura:**

1. Controller recebe a requisição HTTP  
2. Dados são enviados para a camada de aplicação  
3. Entidades de domínio são instanciadas  
4. Regras de cálculo são aplicadas (itens + total)  
5. Dados são persistidos via repositório (EF Core)  
6. Resposta é retornada ao cliente  

---

## 🧱 Arquitetura

- **Invoice.API.Internal** → controllers, Swagger e configuração  
- **Invoice.API.Application** → casos de uso e serviços  
- **Invoice.API.Domain** → entidades e regras de negócio  
- **Invoice.API.Database** → DbContext, repositórios e migrations  
- **ServiceCollectionExtensions** → injeção de dependência  
- **Shared / Config** → componentes reutilizáveis  

---

## 🛠️ Stack

- .NET 8  
- ASP.NET Core Web API  
- Entity Framework Core  
- SQL Server  
- Swagger (OpenAPI)  
- QuestPDF  

---

## 🚀 Como Rodar

### Pré-requisitos

- .NET 8+  
- SQL Server  

### Execução

```bash
git clone https://github.com/seu-usuario/InvoiceAPI.git
cd InvoiceAPI
dotnet restore
dotnet build
dotnet ef database update
dotnet run --project Invoice.API.Internal

```
Acesse: https://localhost:xxxx/swagger
---

## 🔌 Endpoints

### Faturas

- `GET /invoices`
- `GET /invoices/{id}`
- `POST /invoices`
- `PUT /invoices/{id}`
- `DELETE /invoices/{id}`

### Itens

- `POST /invoices/{id}/items`
- `PUT /items/{id}`
- `DELETE /items/{id}`

---

## 📊 Regras de Negócio

- valores de itens são calculados automaticamente  
- total da fatura é sempre derivado  
- qualquer alteração em itens recalcula o total  
- validação antes da persistência  
- controle de estados da fatura  

---

## 📌 Diferenciais

- não é CRUD simples (há lógica de domínio aplicada)  
- separação real de responsabilidades  
- código preparado para evolução  
- arquitetura alinhada a boas práticas de backend  

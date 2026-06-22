# StockWise — UML документация системы складского учёта

**Стек:** C# Avalonia Desktop | .NET 10 | EF Core + Dapper | SQL Server LocalDB | MVVM | QuestPDF | ZXing.Net

---

## 1. Use Case диаграмма

```mermaid
graph TB
    Manager((Менеджер)) -->|Авторизация| Login
    Manager -->|Управление номенклатурой| ManageItems
    Manager -->|Создать товар| CreateItem
    Manager -->|Редактировать товар| EditItem
    Manager -->|Просмотр остатков| ViewStock
    Manager -->|Приход товара| CreateIncome
    Manager -->|Расход товара| CreateOutcome
    Manager -->|Перемещение между складами| CreateTransfer
    Manager -->|Создание заказа клиенту| CreateOrder
    Manager -->|Резервирование товара| ReserveItem
    Manager -->|Отгрузка заказа| ShipOrder
    Manager -->|Просмотр отчётов| ViewReports
    Manager -->|ABC-анализ| AbcAnalysis
    Manager -->|Оборотно-сальдовая| TurnoverReport
    Manager -->|Подбор товара по штрихкоду| ScanBarcode

    Warehouse((Кладовщик)) -->|Авторизация| Login
    Warehouse -->|Инвентаризация| DoInventory
    Warehouse -->|Создать инвентаризацию| CreateInventory
    Warehouse -->|Ввести фактические остатки| EnterFact
    Warehouse -->|Подтвердить расхождения| ConfirmDiff
    Warehouse -->|Перемещение| CreateTransfer
    Warehouse -->|Просмотр остатков| ViewStock

    Admin((Администратор)) -->|Все действия Manager| AllManagerActions
    Admin -->|Управление пользователями| ManageUsers
    Admin -->|Управление складами| ManageWarehouses
    Admin -->|Управление категориями| ManageCategories
    Admin -->|Управление ценовыми группами| ManagePriceGroups
    Admin -->|Управление поставщиками| ManageSuppliers
    Admin -->|Управление контрагентами| ManageCustomers
    Admin -->|Просмотр аудит-лога| ViewAuditLog

    Viewer((Наблюдатель)) -->|Авторизация| Login
    Viewer -->|Просмотр остатков| ViewStock
    Viewer -->|Просмотр номенклатуры| ViewItems
    Viewer -->|Просмотр отчётов| ViewReports

    subgraph "Аутентификация"
        Login
    end

    subgraph "Номенклатура и остатки"
        ManageItems
        CreateItem
        EditItem
        ViewStock
        ScanBarcode
    end

    subgraph "Документооборот"
        CreateIncome
        CreateOutcome
        CreateTransfer
    end

    subgraph "Заказы и резервы"
        CreateOrder
        ReserveItem
        ShipOrder
    end

    subgraph "Инвентаризация"
        DoInventory
        CreateInventory
        EnterFact
        ConfirmDiff
    end

    subgraph "Отчёты и аналитика"
        ViewReports
        AbcAnalysis
        TurnoverReport
    end

    subgraph "Администрирование"
        ManageUsers
        ManageWarehouses
        ManageCategories
        ManagePriceGroups
        ManageSuppliers
        ManageCustomers
        ViewAuditLog
    end
```

---

## 2. Диаграмма классов (Domain Model)

```mermaid
classDiagram
    class User {
        +int Id
        +string FirstName
        +string LastName
        +string Login
        +string PasswordHash
        +string Role
        +string GetFullName()
        +bool HasPermission(string permission)
    }

    class Item {
        +int Id
        +string Name
        +string Article
        +string Unit
        +decimal MinStock
        +decimal MaxStock
        +bool IsBatch
        +string Barcode
        +string ImagePath
        +DateTime CreatedAt
        +bool IsBelowMin()
        +bool IsAboveMax()
    }

    class Category {
        +int Id
        +string Name
        +int? ParentId
        +int SortOrder
        +List~Category~ Children
    }

    class Warehouse {
        +int Id
        +string Name
        +string Address
        +bool IsActive
    }

    class StockBalance {
        +long Id
        +decimal Quantity
        +decimal ReservedQty
        +string BatchNo
        +Date? ExpiryDate
        +decimal Price
        +DateTime UpdatedAt
        +decimal AvailableQty
        +bool IsExpired()
    }

    class Transaction {
        +long Id
        +string Type
        +decimal Quantity
        +string Direction
        +int? RefDocId
        +string RefDocType
        +decimal Price
        +string BatchNo
        +Date? ExpiryDate
        +DateTime CreatedAt
    }

    class IncomeInvoice {
        +int Id
        +string Number
        +DateTime Date
        +string Status
        +decimal TotalAmount
        +List~IncomeLine~ Lines
        +Post()
        +Cancel()
    }

    class IncomeLine {
        +int Id
        +decimal Quantity
        +decimal Price
        +decimal Amount
        +string BatchNo
        +Date? ExpiryDate
    }

    class OutgoingInvoice {
        +int Id
        +string Number
        +DateTime Date
        +string Status
        +decimal TotalAmount
        +List~OutgoingLine~ Lines
        +Post()
        +Cancel()
    }

    class Transfer {
        +int Id
        +string Number
        +int FromWarehouseId
        +int ToWarehouseId
        +DateTime Date
        +string Status
        +List~TransferLine~ Lines
    }

    class Inventory {
        +int Id
        +string Number
        +int WarehouseId
        +DateTime Date
        +string Status
        +List~InventoryLine~ Lines
        +decimal TotalDiff
        +Confirm()
        +Cancel()
    }

    class InventoryLine {
        +int Id
        +decimal ExpectedQty
        +decimal ActualQty
        +decimal Diff
        +decimal Price
        +string BatchNo
    }

    class Order {
        +int Id
        +string Number
        +int CustomerId
        +string Status
        +decimal TotalAmount
        +DateTime CreatedAt
        +List~OrderLine~ Lines
        +Confirm()
        +Ship()
        +Cancel()
    }

    class OrderLine {
        +int Id
        +decimal Quantity
        +decimal Price
        +decimal Amount
        +decimal ShippedQty
    }

    class Reservation {
        +long Id
        +decimal Quantity
        +DateTime CreatedAt
        +string Status
        +Release()
    }

    class PriceGroup {
        +int Id
        +string Name
    }

    class ItemPrice {
        +int Id
        +decimal Price
        +Date ValidFrom
        +Date? ValidTo
        +bool IsActive()
    }

    class Supplier {
        +int Id
        +string Name
        +string INN
        +string ContactPerson
        +string Phone
        +string Email
    }

    class Customer {
        +int Id
        +string Name
        +string INN
        +string ContactPerson
        +string Phone
        +string Email
    }

    class AuditLog {
        +long Id
        +string EntityType
        +int EntityId
        +string Action
        +string OldValue
        +string NewValue
        +DateTime CreatedAt
    }

    %% Связи
    Category "1" --> "*" Category : parent
    Category "1" --> "*" Item : содержит
    Item "1" --> "*" StockBalance : имеет остатки
    Item "1" --> "*" ItemPrice : имеет цены
    Item "1" --> "*" Transaction : участвует
    Warehouse "1" --> "*" StockBalance : хранит
    Warehouse "1" --> "*" Transaction : источник
    Warehouse "1" --> "*" Inventory : инвентаризация
    PriceGroup "1" --> "*" ItemPrice : определяет
    IncomeInvoice "1" --> "*" IncomeLine : состоит из
    IncomeLine "1" --> "1" Item : товар
    OutgoingInvoice "1" --> "*" OutgoingLine : состоит из
    Order "1" --> "*" OrderLine : состоит из
    OrderLine "1" --> "1" Item : товар
    Order "1" --> "*" Reservation : резервирует
    Reservation "1" --> "1" StockBalance : уменьшает
    Supplier "1" --> "*" IncomeInvoice : поставляет
    Customer "1" --> "*" OutgoingInvoice : получает
    Customer "1" --> "*" Order : заказывает
    User "1" --> "*" AuditLog : создаёт
    User "1" --> "*" Transaction : проводит
```

---

## 3. Диаграмма компонентов (Desktop — Clean Architecture)

```mermaid
graph TB
    subgraph StockWise Desktop
        UI["Views / Windows (Avalonia)"]
        VM[ViewModels]
        S[Services]
        R[Repositories]
        DbCtx[DbContext]
    end

    subgraph Business Layer
        Auth[AuthService]
        Stock[StockService]
        Reserve[ReservationService]
        Barcode[BarcodeService]
        Pdf[PdfService]
        Report[ReportService]
        Audit[AuditService]
    end

    subgraph Data Layer
        Dapper[Dapper]
        EF[EF Core]
    end

    subgraph Database
        DB[("SQL Server LocalDB")]
    end

    UI --> VM
    VM --> S
    S --> Auth
    S --> Stock
    S --> Reserve
    S --> Barcode
    S --> Pdf
    S --> Report
    S --> Audit
    S --> R
    R --> Dapper
    R --> EF
    R --> DbCtx
    EF --> DB
    Dapper --> DB
    DbCtx --> DB
```

---

## 4. ER-диаграмма базы данных

```mermaid
erDiagram
    Users {
        int Id PK
        varchar FirstName
        varchar LastName
        varchar Login
        varchar PasswordHash
        varchar Role
    }

    Categories {
        int Id PK
        varchar Name
        int ParentId FK
        int SortOrder
    }

    Items {
        int Id PK
        varchar Name
        varchar Article
        varchar Unit
        decimal MinStock
        decimal MaxStock
        boolean IsBatch
        varchar Barcode
        varchar ImagePath
        timestamp CreatedAt
        int CategoryId FK
    }

    Warehouses {
        int Id PK
        varchar Name
        varchar Address
        boolean IsActive
    }

    StockBalances {
        int Id PK
        decimal Quantity
        decimal ReservedQty
        varchar BatchNo
        date ExpiryDate
        decimal Price
        timestamp UpdatedAt
        int ItemId FK
        int WarehouseId FK
    }

    Transactions {
        int Id PK
        varchar Type
        decimal Quantity
        varchar Direction
        decimal Price
        varchar BatchNo
        date ExpiryDate
        timestamp CreatedAt
        int ItemId FK
        int WarehouseId FK
        int RefDocId
        varchar RefDocType
        int CreatedBy FK
    }

    Invoices {
        int Id PK
        varchar Number
        date Date
        varchar Status
        varchar Type
        decimal TotalAmount
        int SupplierId FK
        int CustomerId FK
        int CreatedBy FK
        timestamp CreatedAt
    }

    InvoiceLines {
        int Id PK
        decimal Quantity
        decimal Price
        decimal Amount
        varchar BatchNo
        date ExpiryDate
        int InvoiceId FK
        int ItemId FK
    }

    Transfers {
        int Id PK
        varchar Number
        date Date
        varchar Status
        int FromWarehouseId FK
        int ToWarehouseId FK
        int CreatedBy FK
        timestamp CreatedAt
    }

    TransferLines {
        int Id PK
        decimal Quantity
        varchar BatchNo
        date ExpiryDate
        int TransferId FK
        int ItemId FK
    }

    Inventories {
        int Id PK
        varchar Number
        date Date
        varchar Status
        int WarehouseId FK
        int CreatedBy FK
        timestamp CreatedAt
    }

    InventoryLines {
        int Id PK
        decimal ExpectedQty
        decimal ActualQty
        decimal Price
        varchar BatchNo
        int InventoryId FK
        int ItemId FK
    }

    Orders {
        int Id PK
        varchar Number
        varchar Status
        decimal TotalAmount
        int CustomerId FK
        int CreatedBy FK
        timestamp CreatedAt
    }

    OrderLines {
        int Id PK
        decimal Quantity
        decimal Price
        decimal Amount
        decimal ShippedQty
        int OrderId FK
        int ItemId FK
    }

    Reservations {
        int Id PK
        decimal Quantity
        varchar Status
        timestamp CreatedAt
        int StockBalanceId FK
        int OrderId FK
    }

    PriceGroups {
        int Id PK
        varchar Name
    }

    ItemPrices {
        int Id PK
        decimal Price
        date ValidFrom
        date ValidTo
        int ItemId FK
        int PriceGroupId FK
    }

    Suppliers {
        int Id PK
        varchar Name
        varchar INN
        varchar ContactPerson
        varchar Phone
        varchar Email
    }

    Customers {
        int Id PK
        varchar Name
        varchar INN
        varchar ContactPerson
        varchar Phone
        varchar Email
    }

    RolePermissions {
        int Id PK
        varchar Role
        varchar Permission
    }

    AuditLogs {
        int Id PK
        varchar EntityType
        int EntityId
        varchar Action
        varchar OldValue
        varchar NewValue
        int UserId FK
        timestamp CreatedAt
    }

    Items }o--|| Categories : belongs_to
    Items ||--o{ StockBalances : has
    Items ||--o{ ItemPrices : priced_in
    Items ||--o{ InvoiceLines : appears_in
    Items ||--o{ TransferLines : moved_in
    Items ||--o{ InventoryLines : counted_in
    Items ||--o{ OrderLines : ordered_in
    Warehouses ||--o{ StockBalances : stores
    Warehouses ||--o{ Transfers : ships_from
    Warehouses ||--o{ Transfers : receives_to
    Warehouses ||--o{ Inventories : audited_at
    PriceGroups ||--o{ ItemPrices : defines
    Invoices ||--o{ InvoiceLines : contains
    Suppliers ||--o{ Invoices : supplies
    Customers ||--o{ Invoices : receives
    Customers ||--o{ Orders : places
    Orders ||--o{ OrderLines : contains
    Orders ||--o{ Reservations : reserves
    StockBalances ||--o{ Reservations : allocated_to
    StockBalances ||--o{ Transactions : movement_history
    Users ||--o{ Transactions : initiated_by
    Users ||--o{ AuditLogs : logged_by
```

---

## 5. Диаграмма последовательности

```mermaid
sequenceDiagram
    actor M as Менеджер
    participant UI as Avalonia App
    participant S as StockService
    participant R as Repository
    participant DB as SQL Server

    Note over M,DB: Создание приходной накладной
    M->>UI: Заполняет форму прихода
    UI->>UI: Выбор товара, кол-во, цена
    M->>UI: Нажимает "Провести"
    UI->>S: PostIncomeInvoiceAsync(invoice)
    S->>R: BeginTransaction()
    S->>R: Insert Invoice + Lines
    R->>DB: INSERT INTO Invoices
    R->>DB: INSERT INTO InvoiceLines
    S->>S: Для каждой строки:
    loop Каждая позиция
        S->>R: GetStockBalance(itemId, warehouseId)
        R->>DB: SELECT FROM StockBalances
        S->>R: UpsertStockBalance(qty +)
        R->>DB: MERGE StockBalances
        S->>R: CreateTransaction(Income, +)
        R->>DB: INSERT INTO Transactions
    end
    S->>R: Commit()
    S->>R: CreateAuditLog(Invoice, Create)
    R->>DB: INSERT INTO AuditLogs
    UI-->>M: Накладная проведена (№ INV-001)

    Note over M,DB: Расход + резервы
    M->>UI: Создаёт заказ
    UI->>S: CreateOrderAsync(order)
    S->>R: Insert Order + Lines
    R->>DB: INSERT INTO Orders
    S->>R: Для каждой строки:
    loop Резервирование
        S->>R: GetAvailableStock(itemId)
        R->>DB: SELECT Quantity-ReservedQty FROM StockBalances
        S->>S: Проверка достаточности
        S->>R: CreateReservation(qty)
        R->>DB: INSERT INTO Reservations
        S->>R: UpdateReservedQty(+)
        R->>DB: UPDATE StockBalances SET ReservedQty+=@qty
    end
    UI-->>M: Заказ № ORD-001 создан

    Note over M,DB: Отгрузка заказа
    M->>UI: Выбирает заказ → "Отгрузить"
    UI->>S: ShipOrderAsync(orderId)
    S->>R: GetOrderLines with Reservations
    S->>S: Для каждой строки:
    loop Списание
        S->>R: UpdateReservationStatus(Shipped)
        R->>DB: UPDATE Reservations SET Status='Shipped'
        S->>R: UpdateReservedQty(-)
        R->>DB: UPDATE StockBalances SET ReservedQty-=@qty
        S->>R: UpdateShippedQty(+)
        R->>DB: UPDATE OrderLines SET ShippedQty+=@qty
        S->>R: CreateTransaction(Outcome, -)
        R->>DB: INSERT INTO Transactions
    end
    S->>R: UpdateOrderStatus(Shipped)
    R->>DB: UPDATE Orders SET Status='Shipped'
    UI-->>M: Заказ отгружен

    Note over M,DB: Инвентаризация
    Warehouse->>UI: Создаёт инвентаризацию
    UI->>S: CreateInventoryAsync(warehouseId)
    S->>R: GetStockByWarehouse(warehouseId)
    R->>DB: SELECT FROM StockBalances WHERE WarehouseId=@id
    S->>R: CreateInventory + Lines с ExpectedQty
    R->>DB: INSERT INTO Inventories + InventoryLines
    Warehouse->>UI: Вводит фактические остатки
    UI->>S: UpdateActualQty(lineId, actualQty)
    S->>R: UPDATE InventoryLines SET ActualQty=@qty
    R->>DB: UPDATE InventoryLines
    Warehouse->>UI: Подтверждает
    UI->>S: ConfirmInventory(inventoryId)
    S->>R: GetLines with Diff
    S->>S: Для каждой строки с Diff != 0:
    loop Корректировка
        S->>R: UpdateStockBalance(diff)
        R->>DB: UPDATE StockBalances SET Quantity+=@diff
        S->>R: CreateTransaction(WriteOff, +/-)
        R->>DB: INSERT INTO Transactions
    end
    S->>R: UpdateInventoryStatus(Confirmed)
    S->>R: CreateAuditLog(Inventory, Confirm, diff)
    UI-->>Warehouse: Инвентаризация завершена
```

---

## 6. Диаграмма развёртывания (Deployment)

```mermaid
graph TB
    subgraph "Рабочая станция (Windows)"
        App[StockWise Desktop<br/>Avalonia UI<br/>.NET 10]
        LocalDB[(SQL Server LocalDB<br/>БД на локальной машине)]
        App --> LocalDB
    end

    subgraph "Опционально: Файловое хранилище"
        Avatars[avatars/<br/>Изображения товаров]
        Pdf[exports/<br/>Сгенерированные PDF]
        App --> Avatars
        App --> Pdf
    end

    subgraph "Внешние устройства"
        Scanner[USB Barcode Scanner<br/>Эмулирует клавиатуру]
    end

    Scanner -->|Ввод штрихкода| App
```

---

## 7. Диаграмма состояний

```mermaid
stateDiagram-v2
    state "Накладная (Приход/Расход)" as Invoice
    [*] --> Draft : Создана
    Draft --> Posted : Проведена
    Draft --> Cancelled : Отменена
    Posted --> [*]
    Cancelled --> [*]

    state "Перемещение" as Transfer
    [*] --> Draft : Создано
    Draft --> Posted : Проведено
    Draft --> Cancelled : Отменено
    Posted --> [*]
    Cancelled --> [*]

    state "Инвентаризация" as Inventory
    [*] --> Draft : Создана
    Draft --> InProgress : Ввод остатков
    InProgress --> Confirmed : Подтверждена
    InProgress --> Draft : Вернуть в черновик
    Draft --> Cancelled : Отменена
    Confirmed --> [*]
    Cancelled --> [*]

    state "Заказ" as Order
    [*] --> New : Создан
    New --> Confirmed : Подтверждён
    Confirmed --> Shipped : Отгружен
    New --> Cancelled : Отменён
    Confirmed --> Cancelled : Отменён
    Shipped --> [*]
    Cancelled --> [*]

    state "Резерв" as Reservation
    [*] --> Active : Зарезервирован
    Active --> Shipped : Отгружен
    Active --> Released : Освобождён
    Shipped --> [*]
    Released --> [*]
```

---

## 8. Ключевые сервисы (API приложения)

| Метод | Сервис | Описание | Роль |
|-------|--------|----------|------|
| GetItemsAsync | ItemService | Список номенклатуры (фильтр, пагинация) | Manager, Viewer |
| CreateItemAsync | ItemService | Создать товар | Manager |
| UpdateItemAsync | ItemService | Редактировать товар | Manager |
| GetStockAsync | StockService | Остатки по складу | Все |
| GetAvailableStockAsync | StockService | Свободный остаток | Manager |
| PostIncomeInvoiceAsync | DocumentService | Провести приход | Manager |
| PostOutcomeInvoiceAsync | DocumentService | Провести расход | Manager |
| CreateTransferAsync | DocumentService | Создать перемещение | Manager, Warehouse |
| PostTransferAsync | DocumentService | Провести перемещение | Manager |
| CreateInventoryAsync | InventoryService | Создать инвентаризацию | Warehouse |
| ConfirmInventoryAsync | InventoryService | Подтвердить расхождения | Warehouse |
| CreateOrderAsync | OrderService | Создать заказ клиента | Manager |
| ShipOrderAsync | OrderService | Отгрузить заказ (списать резервы) | Manager |
| ReserveItemAsync | ReservationService | Зарезервировать товар | Manager |
| ReleaseReservationAsync | ReservationService | Освободить резерв | Manager |
| GeneratePdfAsync | PdfService | Сформировать PDF накладной | Manager |
| GenerateBarcodeAsync | BarcodeService | Сгенерировать штрихкод | Manager |
| GetAbcAnalysisAsync | ReportService | ABC-анализ за период | Manager |
| GetTurnoverReportAsync | ReportService | Оборотно-сальдовая | Manager |
| GetAuditLogAsync | AuditService | Аудит-лог | Admin |

---

## 9. Структура проекта

```
StockWise/
├── StockWise.sln
│
└── src/
    └── StockWise.App/                              # Avalonia Desktop App
        ├── Program.cs                               # DI-контейнер, точка входа
        ├── App.axaml / App.axaml.cs                 # ServiceProvider, тема
        ├── MainWindow.axaml / .cs                   # Shell-окно (сайдбар + workspace)
        ├── appsettings.json                         # Подключение к БД, настройки
        │
        ├── Data/
        │   ├── StockDb.cs                           # DbContext (EF Core)
        │   └── Configurations/                      # Fluent API конфигурации
        │       ├── ItemConfiguration.cs
        │       ├── StockBalanceConfiguration.cs
        │       ├── InvoiceConfiguration.cs
        │       └── OrderConfiguration.cs
        │
        ├── Models/                                  # EF Core сущности + ViewModel-модели
        │   ├── Item.cs
        │   ├── Category.cs
        │   ├── Warehouse.cs
        │   ├── StockBalance.cs
        │   ├── Transaction.cs
        │   ├── IncomeInvoice.cs / IncomeLine.cs
        │   ├── OutgoingInvoice.cs / OutgoingLine.cs
        │   ├── Transfer.cs / TransferLine.cs
        │   ├── Inventory.cs / InventoryLine.cs
        │   ├── Order.cs / OrderLine.cs
        │   ├── Reservation.cs
        │   ├── PriceGroup.cs / ItemPrice.cs
        │   ├── Supplier.cs / Customer.cs
        │   ├── User.cs
        │   ├── RolePermission.cs
        │   └── AuditLog.cs
        │
        ├── Repositories/                            # Dapper (read) + EF (write)
        │   ├── ItemRepository.cs
        │   ├── CategoryRepository.cs
        │   ├── WarehouseRepository.cs
        │   ├── StockRepository.cs
        │   ├── TransactionRepository.cs
        │   ├── InvoiceRepository.cs
        │   ├── TransferRepository.cs
        │   ├── InventoryRepository.cs
        │   ├── OrderRepository.cs
        │   ├── ReservationRepository.cs
        │   ├── PriceRepository.cs
        │   ├── SupplierRepository.cs
        │   ├── CustomerRepository.cs
        │   ├── UserRepository.cs
        │   └── AuditRepository.cs
        │
        ├── Services/                                # Бизнес-логика
        │   ├── AuthService.cs                       # BCrypt + RBAC проверка
        │   ├── StockService.cs                      # Double-entry проводки
        │   ├── DocumentService.cs                    # Накладные, перемещения
        │   ├── OrderService.cs                      # Заказы + резервы
        │   ├── InventoryService.cs                  # Инвентаризация
        │   ├── ReportService.cs                     # ABC, оборотка
        │   ├── BarcodeService.cs                    # ZXing.Net
        │   ├── PdfService.cs                        # QuestPDF
        │   ├── AuditService.cs                      # JSON-лог
        │   ├── ThemeService.cs                      # Светлая/тёмная тема
        │   └── NotificationService.cs               # Уведомления
        │
        ├── ViewModels/
        │   ├── MainViewModel.cs                     # Навигация, состояние
        │   ├── LoginViewModel.cs
        │   ├── ItemListViewModel.cs                 # Список номенклатуры
        │   ├── ItemEditViewModel.cs                 # Карточка товара
        │   ├── StockViewModel.cs                    # Остатки по складам
        │   ├── IncomeViewModel.cs                   # Приходная накладная
        │   ├── OutcomeViewModel.cs                  # Расходная накладная
        │   ├── TransferViewModel.cs                 # Перемещение
        │   ├── InventoryViewModel.cs                # Инвентаризация
        │   ├── OrderListViewModel.cs                # Список заказов
        │   ├── OrderEditViewModel.cs                # Карточка заказа
        │   ├── PurchaseOrderViewModel.cs            # Заказы поставщикам
        │   ├── ReportsViewModel.cs                  # Отчёты
        │   ├── DashboardViewModel.cs                # Дашборд
        │   ├── AdminViewModel.cs                    # Управление пользователями/складами
        │   └── AuditViewModel.cs                    # Аудит-лог
        │
        ├── Views/                                   # Avalonia окна и страницы
        │   ├── ShellView.axaml                      # Сайдбар (дерево категорий + навигация)
        │   ├── LoginView.axaml
        │   ├── ItemListView.axaml
        │   ├── ItemEditView.axaml
        │   ├── StockView.axaml
        │   ├── IncomeView.axaml
        │   ├── OutcomeView.axaml
        │   ├── TransferView.axaml
        │   ├── InventoryView.axaml
        │   ├── OrderListView.axaml
        │   ├── OrderEditView.axaml
        │   ├── ReportsView.axaml
        │   ├── DashboardView.axaml
        │   ├── AdminView.axaml
        │   ├── AuditView.axaml
        │   ├── Dialogs/
        │   │   ├── ItemSelectorDialog.axaml         # Выбор товара (поиск + штрихкод)
        │   │   ├── WarehouseSelectorDialog.axaml
        │   │   ├── ConfirmPostDialog.axaml          # Подтверждение проводки
        │   │   └── BatchInputDialog.axaml           # Ввод партии/срока годности
        │   └── Controls/
        │       ├── StockCard.axaml                  # Карточка остатка
        │       ├── TransactionGrid.axaml            # Таблица проводок
        │       └── BarcodeControl.axaml             # Штрихкод генератор
        │
        ├── Converters/
        │   ├── BoolToVisibilityConverter.cs
        │   ├── StatusToColorConverter.cs
        │   └── DecimalToCurrencyConverter.cs
        │
        └── Commands/
            ├── RelayCommand.cs
            └── RelayCommandT.cs
```

---

## 10. Стек технологий

| Компонент | Технология |
|-----------|-----------|
| Frontend | Avalonia 11.3.12 (Desktop, .NET 10) |
| ORM (write) | Entity Framework Core |
| ORM (read) | Dapper |
| DB | SQL Server LocalDB |
| Auth | BCrypt.Net-Next (пароли) + RBAC (роли) |
| PDF | QuestPDF |
| Barcode | ZXing.Net |
| DI | Microsoft.Extensions.DependencyInjection |
| Тема | DynamicResource + программная смена |

---

## 11. Примеры данных (начальные)

### Категории
| Уровень | Название |
|---------|----------|
| 1 | Электроника |
| 2 | ├── Бытовая техника |
| 2 | ├── Компьютеры |
| 2 | ├── Телефоны |
| 1 | Строительные материалы |
| 2 | ├── Электрика |
| 2 | ├── Сантехника |
| 2 | ├── Отделочные материалы |
| 1 | Канцелярия |
| 2 | ├── Бумага |
| 2 | ├── Ручки |

### Склады
| Название | Адрес |
|----------|-------|
| Основной склад | ул. Ленина, 10 |
| Склад №2 | ул. Промышленная, 5 |
| Мелкооптовый | ТЦ "Гигант", пав. 12 |

### Роли и права
| Роль | Разрешённые действия |
|------|---------------------|
| Admin | Всё |
| Manager | CRUD номенклатуры, создание/проведение документов, заказы, отчёты |
| Warehouse | Просмотр остатков, инвентаризация, перемещения |
| Viewer | Только просмотр: остатки, номенклатура, отчёты |

### Номенклатура (пример)
| Артикул | Название | Категория | Ед. | Мин. | Цена |
|---------|----------|-----------|-----|------|------|
| EL-001 | Кабель HDMI 2м | Электроника | шт | 5 | 350₽ |
| EL-002 | Розетка двойная | Электрика | шт | 10 | 120₽ |
| ST-001 | Цемент М500 50кг | Строительные | меш | 20 | 550₽ |
| KN-001 | Бумага А4 CLASSIC | Бумага | пач | 50 | 320₽ |

---

## 12. Ключевые бизнес-правила

| # | Правило |
|---|---------|
| 1 | Нельзя провести расход, если свободный остаток < количество в документе |
| 2 | Резерв уменьшает свободный остаток, но не списывает товар |
| 3 | Инвентаризация создаёт корректирующие проводки только при расхождении |
| 4 | При отмене проведённого документа создаются обратные проводки |
| 5 | У одного товара может быть несколько цен (по ценовым группам) |
| 6 | Артикул товара уникален |
| 7 | Номер документа уникален в пределах типа (INV-001, TRF-001, ORD-001) |
| 8 | Категории — дерево, удаление категории удаляет все подкатегории |
| 9 | Аудитный лог пишется на каждое изменение сущности (Create/Update/Delete/Post/Cancel) |
| 10 | ABC-анализ: A (80%), B (15%), C (5%) от общей стоимости расхода |

---

## 13. Формула ABC-анализа (SQL)

```sql
-- ABC-анализ за период
WITH Sales AS (
    SELECT
        i.Id,
        i.Name,
        SUM(t.Quantity * t.Price) AS TotalSales
    FROM Transactions t
    JOIN Items i ON i.Id = t.ItemId
    WHERE t.Direction = '-'
      AND t.Type = 'Outcome'
      AND t.CreatedAt BETWEEN @startDate AND @endDate
    GROUP BY i.Id, i.Name
),
Ranked AS (
    SELECT
        Id, Name, TotalSales,
        TotalSales * 1.0 / SUM(TotalSales) OVER() AS SharePct,
        SUM(TotalSales) OVER(ORDER BY TotalSales DESC) / SUM(TotalSales) OVER() AS CumulativePct
    FROM Sales
)
SELECT
    Id, Name, TotalSales,
    ROUND(SharePct * 100, 2) AS SharePct,
    ROUND(CumulativePct * 100, 2) AS CumulativePct,
    CASE
        WHEN CumulativePct <= 0.80 THEN 'A'
        WHEN CumulativePct <= 0.95 THEN 'B'
        ELSE 'C'
    END AS AbcClass
FROM Ranked
ORDER BY TotalSales DESC;
```

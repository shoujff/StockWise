# План дальнейшей разработки StockWise

## Условные обозначения
- ✅ Реализовано
- 🔄 В процессе
- ⬜ Не реализовано

---

## Текущее состояние (что уже сделано)

| Раздел | Статус |
|--------|--------|
| Базовая архитектура: DI, Program.cs, конфигурация | ✅ |
| DbContext (StockDb) с базовыми сущностями | ✅ |
| Миграции EF Core (InitialCreate, AddRolePermissions) | ✅ |
| Модели: User, Item, Category, Warehouse, StockBalance, Transaction, RolePermission, Order, OrderLine, Reservation, PriceGroup, ItemPrice, Supplier, Customer, Inventory, InventoryLine, AuditLog | ✅ |
| Конфигурации EF: Item, Category, StockBalance, Transaction, User, Warehouse, RolePermission | ✅ |
| Репозитории: User, Category, Item, Warehouse, StockBalance, Transaction, RolePermission, BaseRepository | ✅ |
| DapperContext для быстрых чтений | ✅ |
| Сервисы: AuthService (BCrypt + RBAC), ItemService (CRUD), CategoryService (дерево) | ✅ |
| ViewModels: Login, Register, MainWindow | ✅ |
| Views: LoginView, LoginWindow, RegisterView, MainWindow (оболочка с сайдбаром) | ✅ |
| App.axaml, AppStyles.axaml | ✅ |

---

## План по этапам

### Этап 1. 📦 Номенклатура и категории (UI) — 10%
1. **ItemListView.axaml + ItemListViewModel** — таблица товаров с поиском и фильтром по категории
2. **ItemEditView.axaml + ItemEditViewModel** — карточка товара (создание/редактирование)
3. **Shell → навигация через MainWindowViewModel** — переключение между страницами
4. **TreeView категорий** в сайдбаре Shell

### Этап 2. 🏭 Склады и остатки (UI) — 10%
1. **Warehouse CRUD** — View/ViewModel для управления складами
2. **StockView.axaml + StockViewModel** — просмотр остатков по складу (таблица + фильтры)
3. **Свободный остаток** = Quantity - ReservedQty (отображение)

### Этап 3. 📄 Документооборот — приход/расход (30%)
1. **StockService** — double-entry проводки (Income/Outcome/Transfer + отмена)
2. **DocumentService** — бизнес-логика накладных
3. **IncomeView.axaml + IncomeViewModel** — приходная накладная (шапка + строки)
4. **OutcomeView.axaml + OutcomeViewModel** — расходная накладная
5. **TransferView.axaml + TransferViewModel** — перемещение между складами
6. **Статусы документов**: Draft → Posted → Cancelled
7. **Проверка остатка** при расходе

### Этап 4. 📋 Заказы и резервы — 15%
1. **OrderService** — создание, подтверждение, отгрузка, отмена
2. **ReservationService** — управление резервами
3. **OrderListView.axaml + OrderListViewModel** — список заказов
4. **OrderEditView.axaml + OrderEditViewModel** — карточка заказа (создание, отгрузка)
5. **Резервирование** при создании заказа
6. **Частичная отгрузка** (ShippedQty)

### Этап 5. 🔄 Инвентаризация — 10%
1. **InventoryService** — создание, ввод факта, подтверждение
2. **InventoryView.axaml + InventoryViewModel** — форма инвентаризации
3. **Автоподтягивание ожидаемых остатков**
4. **Корректирующие проводки** при подтверждении расхождений

### Этап 6. 📊 Отчёты и аналитика — 10%
1. **ReportService** — ABC-анализ, оборотно-сальдовая
2. **ReportsView.axaml + ReportsViewModel** — страница отчётов
3. **DashboardView.axaml + DashboardViewModel** — дашборд (кол-во товаров, сумма остатков, документы)
4. **SQL запросы** для ABC и оборотки (Dapper)

### Этап 7. 🔧 Дополнительные функции — 5%
1. **BarcodeService** — генерация штрихкодов (ZXing.Net)
2. **PdfService** — печать накладных (QuestPDF)
3. **AuditService** — JSON-логирование изменений
4. **ItemSelectorDialog** — выбор товара с поиском и сканером

### Этап 8. 🛠 Администрирование — 5%
1. **AdminView.axaml + AdminViewModel** — управление пользователями, складами, категориями
2. **AuditView.axaml + AuditViewModel** — просмотр аудит-лога
3. **ThemeService** — переключение светлой/тёмной темы

### Этап 9. 🧪 Тестирование и доработки — 5%
1. Модульные тесты сервисов
2. Интеграционные тесты проводок
3. Seed-данные (категории, склады, товары)
4. Финальная проверка всех сценариев

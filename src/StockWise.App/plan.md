# StockWise — Полный план разработки

**Стек:** C# Avalonia 11.3 Desktop | .NET 10 | EF Core + Dapper | SQL Server LocalDB | CommunityToolkit.Mvvm | QuestPDF | ZXing.Net

---

## Условные обозначения
- ✅ — Выполнено
- 🔄 — В процессе
- ⬜ — Не реализовано
- ❌ — Отменено

---

## Текущее состояние (архитектура)

| Компонент | Статус |
|-----------|--------|
| DI-контейнер (Program.cs) | ✅ `Microsoft.Extensions.DependencyInjection` |
| EF Core DbContext + миграции | ✅ `StockDb`, `InitialCreate`, `AddRolePermissions` |
| DapperContext (быстрые чтения) | ✅ |
| Сущности (18 шт.) | ✅ |
| Репозитории (14 шт.) | ✅ |
| Сервисы: AuthService, ItemService, CategoryService | ✅ |
| ThemeService (светлая/тёмная) | ✅ |
| MainWindow Shell (сайдбар + workspace) | ✅ Включая ToastContainer + DialogOverlay |
| Particle System (частицы фона) | ✅ Только пульсация Opacity |

---

## ФАЗА 1: Атмосферные эффекты (DONE)

- [x] **ScanlineBrush** — DrawingBrush с горизонтальными линиями (альфа ~4%)
- [x] **VignetteGradient** — RadialGradientBrush с затемнением по краям (Radius=1)
- [x] **Анимации убраны** — RenderTransform нельзя анимировать через Style.Animations в Avalonia 11
- [x] **Частицы** — только пульсация прозрачности (без движения)

---

## ФАЗА 2: Компонентный Cyberpunk-полиш (DONE)

- [x] **DataGrid** — неоновые строки, стеклянный header, hover/selection glow
- [x] **Glass Cards v2** — глянцевый блик сверху, тонкая рамка-обводка
- [x] **Modal/Dialog** — overlay с размытием backdrop, glass-карточка, C# ViewModel
- [x] **Toast/Notification** — правый верхний угол, 4 типа (success/error/warn/info), конвертер цвета рамки
- [x] **Context Menu** — тёмный glass, неоновый divider, hover-подсветка
- [x] **Tooltip** — неоновая подложка, задержка появления

---

## ФАЗА 3: Авторизация и роли — 10%

| Задача | Статус | Примечание |
|--------|--------|------------|
| LoginView + LoginViewModel | ✅ | Форма входа |
| RegisterView + RegisterViewModel | ✅ | Форма регистрации |
| BCrypt-хэширование паролей | ✅ | Через BCrypt.Net-Next |
| 4 роли: Admin, Manager, Warehouse, Viewer | ✅ | Таблица RolePermissions |
| RBAC — проверка прав на каждое действие | 🔄 | Внедрить в каждый ViewModel/Service |
| Сессия пользователя | 🔄 | Хранить текущего юзера в MainWindowViewModel |
| Сброс пароля (опционально) | ⬜ | Требуется email-сервис |

| Роль | Действия |
|------|----------|
| Admin | Всё |
| Manager | CRUD номенклатуры, создание/проведение документов, заказы, отчёты |
| Warehouse | Просмотр остатков, инвентаризация, перемещения |
| Viewer | Только просмотр: остатки, номенклатура, отчёты |

---

## ФАЗА 4: Номенклатура и категории — 10%

### 4.1 ItemListView
- [x] DataGrid: Артикул, Наименование, Категория, Ед., Мин./Макс., Штрихкод
- [x] Поиск с debounce
- [x] Фильтр по категории (ComboBox)
- [x] Панель инструментов: Добавить, Редактировать, Удалить, Обновить
- [x] Toast-уведомления при сохранении/удалении

### 4.2 ItemEditView
- [x] Форма: Название, Артикул (уникальный), Категория (TreeView-пикер), Ед. изм., Мин./Макс. остаток, IsBatch, Штрихкод, Фото
- [x] Валидация (уникальность артикула, обязательные поля)
- [x] Сохранение → Toast «Товар сохранён» → навигация назад

### 4.3 Категории
- [x] TreeView в сайдбаре (иерархический)
- [ ] CRUD категорий — диалог создания/редактирования/удаления с drag-drop
- [ ] CategoryTreePicker — переиспользуемый контрол для ItemEditView

### 4.4 Сервисы
- [x] ItemService — CRUD + поиск + пагинация
- [x] CategoryService — дерево (GetTree, GetChildren, Move)

---

## ФАЗА 5: Склады и остатки — 10%

| Задача | Статус |
|--------|--------|
| CRUD складов (View + ViewModel) | ⬜ |
| StockView — остатки по складу (DataGrid) | ⬜ |
| Свободный остаток = Quantity – ReservedQty | ✅ Вычисляемое свойство |
| Выбор склада (ComboBox/Dialog) | ⬜ |
| Индикатор «ниже мин. остатка» (подсветка строк) | ⬜ |

```
┌─────────────────────────────────────────────────────────┐
│ [Склад: ▾ Основной]  [Поиск...]  [🔍]                    │
├──────┬──────────┬────────┬────────┬────────┬────────────┤
│ Товар│Кол-во    │Резерв  │Свободно│Партия  │Срок годн.  │
│ HDMI │ 100      │ 5      │ 95     │ B-001  │ 2026-12-31 │
└──────┴──────────┴────────┴────────┴────────┴────────────┘
```

```csharp
public class StockBalance {
    decimal Quantity;
    decimal ReservedQty;
    string? BatchNo;
    DateOnly? ExpiryDate;
    decimal Price;
    // Available = Quantity - ReservedQty
}
```

---

## ФАЗА 6: Двойные проводки (CORE) — 15%

| Задача | Описание |
|--------|----------|
| Transaction — сущность | Type, Direction (+/-), Quantity, Price, BatchNo, ExpiryDate, RefDocId, RefDocType |
| StockService — единый источник истины | Double-entry: каждое движение = 2+ строк Transaction в одной SQL-транзакции |
| PostIncomeAsync(warehouseId, lines) | +Qty на склад, создать проводки Income |
| PostOutcomeAsync(warehouseId, lines) | Проверить Available ≥ Qty, -Qty, создать проводки Outcome |
| PostTransferAsync(from, to, lines) | -Qty со склада A, +Qty на склад B, одна SQL-транзакция |
| CancelDocument(docId) | Обратные проводки (противоположный Direction), статус → Cancelled |
| Batch/Expiry FIFO | При расходе — диалог выбора партии |

```
Приход 100 HDMI на склад Основной:
  Transaction(Direction='+', Qty=100, Type='Income', RefDocId=INV-001)

Отгрузка 5 HDMI со склада Основной:
  Transaction(Direction='-', Qty=5, Type='Outcome', RefDocId=OUT-001)

Перемещение 10 HDMI со Склад №2 → Основной:
  Transaction(Direction='-', Qty=10, Type='Transfer', WarehouseId=2, RefDocId=TRF-001)
  Transaction(Direction='+', Qty=10, Type='Transfer', WarehouseId=1, RefDocId=TRF-001)
```

---

## ФАЗА 7: Документооборот — 15%

```
Документ (Invoice) ←── InvoiceLines
    ↑                    ↑
IncomeInvoice      IncomeLine / OutgoingLine
OutgoingInvoice
Transfer      ←── TransferLines
```

```
Draft ──→ Posted ──→ [*]
  │                    ↑
  └────→ Cancelled ────┘
```

| View | Фичи |
|------|------|
| IncomeView | Шапка (поставщик, дата, номер), строки (товар, кол-во, цена, партия, срок), кнопки Провести/Отменить |
| OutcomeView | Шапка (клиент, дата, номер), строки, проверка остатка, Провести/Отменить |
| TransferView | Откуда/Куда (склады), строки, Провести/Отменить |
| Список документов | Фильтр по статусу/дате/складу, двойной клик — просмотр |
| Нумерация | INV-001, OUT-001, TRF-001 (по типу + год) |
| PDF | QuestPDF — шаблон на каждую форму документа |

---

## ФАЗА 8: Заказы и резервы — 15%

```
New ──→ Confirmed ──→ Shipped
 │          │            ↑
 └──→ Cancelled ─────────┘
```

```
Order ──── OrderLines ──── Item
  │
  └─── Reservations ──── StockBalance
```

| Задача | Описание |
|--------|----------|
| Order | Number, CustomerId, Status, TotalAmount |
| OrderLine | Quantity, Price, Amount, ShippedQty |
| Reservation | Quantity, Status (Active/Shipped/Released), ссылка на StockBalance + Order |
| Создание заказа | Зарезервировать товар (ReservedQty +=), создать Reservation |
| Отгрузка | Снять резерв, -AvailableQty, +ShippedQty, проводки Outcome |
| Частичная отгрузка | ShippedQty per line, несколько отгрузок на один заказ |
| Отмена заказа | Освободить все резервы (ReservedQty -=), статус → Cancelled |
| OrderListView | Фильтр по статусу/клиенту/дате |
| OrderEditView | Добавить/удалить строки, изменить кол-во, пересчёт суммы |

---

## ФАЗА 9: Инвентаризация — 10%

```
Draft ──→ InProgress ──→ Confirmed
  │          │               ↑
  └────→ Cancelled ──────────┘
```

| Задача | Описание |
|--------|----------|
| Inventory | WarehouseId, Date, Status, TotalDiff |
| InventoryLine | ExpectedQty (из StockBalance), ActualQty (ввод), Diff, Price |
| Создание | Авто-заполнение строк текущими остатками по складу |
| Ввод факта | Редактируемая колонка ActualQty, Diff в реальном времени |
| Подтверждение | Для Diff ≠ 0: обновить StockBalance, создать WriteOff (Transaction), AuditLog |
| InventoryView | Progress bar, карточки: совпало / избыток / недостача |

---

## ФАЗА 10: Отчёты и аналитика — 10%

| Отчёт | Реализация |
|-------|------------|
| ABC-анализ | Dapper-запрос: ранжирование товаров по сумме расхода (A=80%, B=15%, C=5%) |
| Оборотно-сальдовая | Остаток нач. + Приход – Расход = Остаток кон. (за период, по товару + складу) |
| Дашборд | KPI: всего товаров, суммарный остаток, документов за месяц, товаров ниже мин. |
| ReportsView | Выбор периода, экспорт в PDF/Excel |

```sql
WITH Sales AS (
    SELECT i.Id, i.Name, SUM(t.Quantity * t.Price) AS TotalSales
    FROM Transactions t JOIN Items i ON i.Id = t.ItemId
    WHERE t.Direction = '-' AND t.Type = 'Outcome' AND t.CreatedAt BETWEEN @start AND @end
    GROUP BY i.Id, i.Name
), Ranked AS (
    SELECT *, TotalSales * 1.0 / SUM(TotalSales) OVER() AS SharePct,
           SUM(TotalSales) OVER(ORDER BY TotalSales DESC) / SUM(TotalSales) OVER() AS CumulativePct
    FROM Sales
)
SELECT *, CASE
    WHEN CumulativePct <= 0.80 THEN 'A'
    WHEN CumulativePct <= 0.95 THEN 'B'
    ELSE 'C'
END AS AbcClass
FROM Ranked ORDER BY TotalSales DESC;
```

---

## ФАЗА 11: Дополнительно — 5%

| Фича | Технология |
|------|-----------|
| Генерация штрихкодов | ZXing.Net → PNG, печать ценников |
| Сканирование штрихкода | USB-сканер = клавиатурный ввод, focus-aware TextBox |
| Печать PDF | QuestPDF — шаблоны Invoice, Order, Transfer, Inventory |
| Аудит-лог | JSON (OldValue/NewValue), EF Core SaveChangesInterceptor |
| ItemSelectorDialog | Поиск + сканер, возвращает ItemDto, выбор из DataGrid |

---

## ФАЗА 12: Администрирование — 5%

| View | Фичи |
|------|------|
| User Management | CRUD пользователей, назначение ролей, сброс пароля |
| Warehouse Management | CRUD складов, установка склада по умолчанию |
| Category Management | TreeView-редактор (drag-drop) |
| Price Groups | CRUD, привязка к клиентам |
| Supplier/Customer | CRUD, привязка к документам |
| AuditView | Фильтр по сущности/пользователю/дате, разворачивание JSON-diff |

---

## ФАЗА 13: UI-микроанимации

| Задача | Реализация |
|--------|-----------|
| Hover/press ripple на кнопках | Border + PointerPressed/PointerReleased + анимация Opacity |
| Slide+fade между страницами | ContentControl + TransitioningContentControl или кастом |
| Staggered появление списка | ItemsControl + задержка по индексу элемента |
| Skeleton loaders v2 | Пульсирующий glow (keyframe на градиенте) |
| Анимация смены цифр | Behavior / кастомный TextBlock с tween |

---

## ФАЗА 14: Типографика и иконки

| Задача | Описание |
|--------|----------|
| JetBrains Mono | .ttf в Assets, FontFamily="avares://StockWise.App/Assets/Fonts/#JetBrains Mono" |
| Набор иконок | Material Icons (Outlined) или Lucide — как Path-геометрии в ResourceDictionary |
| IconButton | Reusable-компонент: Icon + ToolTip.Tip + hover glow |
| StatusBadge | Цветная пилюля: Success (зелёный), Warning (янтарь), Danger (красный), Info (циан) |

---

## ФАЗА 15: Оптимизация и производительность

| Задача | Описание |
|--------|----------|
| Virtualizing StackPanel | VirtualizingStackPanel на всех больших DataGrid / ItemsControl |
| Lazy load ViewModel | Загрузка при первой навигации, кеширование |
| Composition API для частиц | ElementCompositionPreview — GPU-трансформ частиц |
| Скелетоны для загрузки | Per-view skeleton template |
| Аудит compiled bindings | Исправить все AVLN3001 (добавить x:DataType везде) |

---

## Прогресс архитектуры

| Слой | Всего | Готово | Осталось |
|------|-------|--------|----------|
| Models (EF сущности) | 18 | 18 | 0 |
| Repositories | 14 | 14 | 0 |
| Services (бизнес-логика) | 9 | 3 | 6 |
| ViewModels | 14 | 3 | 11 |
| Views | 18 + диалоги | 4 | 14+ |
| Converters | 4+ | 2 | 2+ |
| Dialogs | 4 | 0 | 4 |
| Custom Controls | 3 | 0 | 3 |

---

## Временна́я оценка

| Фаза | % | Примерно дней |
|------|---|---------------|
| 3. Auth & RBAC | 10% | 2-3 |
| 4. Номенклатура | 10% | 2-3 |
| 5. Склады | 10% | 2-3 |
| 6. Проводки (core) | 15% | 4-5 |
| 7. Документы | 15% | 4-5 |
| 8. Заказы + резервы | 15% | 4-5 |
| 9. Инвентаризация | 10% | 3-4 |
| 10. Отчёты | 10% | 3-4 |
| 11. Дополнительно | 5% | 2-3 |
| 12. Администрирование | 5% | 2-3 |
| Функциональные всего | 100% | ~30-40 дней |
| UI-фазы 13-15 (анимации+произв.) | — | 5-7 дней |
| Итого | — | ~35-47 дней |

---

## Известные проблемы

| Проблема | Описание |
|----------|----------|
| NETSDK1057 | .NET 10 preview — ожидаемо |
| NU1903 | Tmds.DBus.Protocol (предсуществующая уязвимость) |
| CS8602 / CS8604 | Nullable warnings в ThemeService, ItemService, ItemListViewModel |
| AVLN3001 | Compiled bindings, не влияет на работу |
| RenderTransform | Не анимируется через Style.Animations — будет исправлено в Phase 15 Composition API |

---

## Вопросы для уточнения

1. Навигация: оставить текущий Page-switch через MainWindowViewModel, или перейти на TabView?
2. Партии/сроки годности: отдельный диалог BatchInputDialog при приходе/расходе — утвердить UX-флоу?
3. Отчёты: отдельная вкладка Reports (ABC + Оборотка) или встроить в Дашборд?
4. PDF: QuestPDF code-first или HTML-to-PDF?
5. Сканер штрихкода: выделенный «Режим сканирования» (overlay) или просто focus-чувствительный TextBox?
6. Без авторизации: стартовый экран — Login или сразу MainWindow?

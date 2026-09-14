# OneForAll-自动数据清理与同步服务

> 一个用于自动清理过期数据和跨库同步表数据的后台服务，专为解决 OneForAll 数据库快速增长问题而设计。

随着 OneForAll 系统运行时间增长，数据库容量迅速膨胀，影响性能与维护成本。本程序通过定时删除历史数据、同步关键表数据，实现数据库容量控制与数据迁移目标。

---

## 功能概览

### 🗑️ 一、自动数据删除
支持对指定数据表进行安全、可控的历史数据清理。

- ✅ **灵活指定目标表**：可配置多个需清理的表。
- ✅ **按条件删除数据**：
  - 按时间字段（如 `CreateTime`）保留指定天数内的数据（`KeepDays`）。
  - 支持自定义 SQL 条件（`CustonWhere`），实现复杂逻辑过滤。
- ✅ **分批删除机制**：每次删除限定数量（`MaxDelCount`），避免大事务锁表。
- ✅ **断点续删支持**：记录上次删除的主键 ID（`LastDelId`），防止重复扫描（要求主键与时间严格同序，如自增 ID）。
- ✅ **删除模式可配置**：可针对每个表停用 `LastDelId` 限制，每次直接按查询条件删除一批数据，适用于雪花 ID 等非严格递增 ID，避免漏删。
- ✅ **启停控制**：可通过配置启用/禁用特定表的删除任务。

### 📁 二、自动文件删除
支持对指定目录进行定时过期文件清理，适用于日志文件、录像文件等场景。

- ✅ **多目录配置**：可配置多个需清理的目录，每个目录独立设置策略。
- ✅ **按保留天数清理**：根据文件最后修改时间，删除超过 `KeepDays` 天的文件。
- ✅ **分批删除控制**：通过 `MaxDeleteCount` 限制每次最大删除数量，避免 IO 峰值。
- ✅ **子目录递归**：可配置 `IncludeSubDirectories` 控制是否递归扫描子目录。
- ✅ **流式枚举**：采用延迟枚举方式扫描文件，目录文件量大时不会大量占用内存。
- ✅ **多节点隔离**：通过 `ClientCode` 区分不同服务节点，支持分布式部署下共用配置库。
- ✅ **启停控制**：可通过 `IsEnabled` 启用/禁用特定目录的删除任务。

### 📦 三、自动文件迁移
支持对指定目录的过期文件进行定时迁移，适用于日志归档、文件备份等场景。

- ✅ **多目录配置**：可配置多个需迁移的目录，每个目录独立设置源目录与目标目录。
- ✅ **按保留天数迁移**：根据文件最后修改时间，迁移超过 `KeepDays` 天的文件。
- ✅ **分批迁移控制**：通过 `MaxMoveCount` 限制每次最大迁移数量，避免 IO 峰值。
- ✅ **子目录递归**：可配置 `IncludeSubDirectories` 控制是否递归迁移（递归迁移时，会按照原文件所在目录层级迁移）。
- ✅ **流式枚举**：采用延迟枚举方式扫描文件，目录文件量大时不会大量占用内存。
- ✅ **目标目录自动创建**：目标目录不存在时自动创建（递归迁移时同时自动创建对应层级子目录）。
- ✅ **同名文件覆盖**：迁移时若目标目录已存在同名文件，会先删除再迁移。
- ✅ **多节点隔离**：通过 `ClientCode` 区分不同服务节点，支持分布式部署下共用配置库。
- ✅ **启停控制**：可通过 `IsEnabled` 启用/禁用特定目录的迁移任务。

### 🔁 四、数据表同步
支持将源数据库中的数据增量同步至目标数据库。

- ✅ **指定同步表**：配置需要同步的表及字段。
- ✅ **增量同步**：基于时间或主键记录上次同步位置（`LastSynId`, `LastSynTime`）。
- ✅ **分批同步**：每次同步限定数量（`MaxSynCount`），保障系统稳定性。
- ✅ **支持删除同步**：可在同步时同步删除操作（需业务逻辑配合）。
- ✅ **自定义同步条件**：通过 `CustonWhere` 灵活筛选同步数据。

---

## 数据库表结构说明

### 1. `DelConfig` - 数据删除配置表

| 字段名 | 类型 | 说明 |
|-------|------|------|
| `Id` | int | 主键 |
| `TableName` | nvarchar(200) | 要删除数据的表名 |
| `KeepDays` | int | 保留最近 N 天的数据（优先级高于 `KeepId`） |
| `DateFieldName` | nvarchar(max) | 时间字段名（如 `CreateTime`） |
| `CustonWhere` | nvarchar(2000) | 自定义删除条件（额外 WHERE 子句） |
| `KeepId` | bigint | 保留主键 ID 大于该值的数据（可选） |
| `MaxDelCount` | int | 每次删除最大行数 |
| `IsLastDelIdEnabled` | bit | 是否启用 `LastDelId` 断点续删：1=启用（按 ID 区间逐批推进，要求主键与时间严格同序，如自增 ID）；0=停用（每次直接按查询条件删除，适用于雪花 ID 等非严格递增 ID） |
| `LastDelId` | bigint | 上次删除的最大主键 ID（用于断点续删） |
| `LastDelTime` | datetime2 | 上次执行删除时间 |
| `WaitingTime` | datetime2 | 下次执行等待时间（可用于延迟执行） |
| `IsEnabled` | bit | 是否启用该删除任务 |

---

### 2. `SyncConfig` - 数据同步配置表

| 字段名 | 类型 | 说明 |
|-------|------|------|
| `Id` | int | 主键 |
| `TableName` | nvarchar(200) | 同步的表名 |
| `TableFeldStr` | nvarchar(max) | 同步字段列表（如 `Id,Name,CreateTime`） |
| `DateFieldName` | nvarchar(max) | 时间字段名（用于增量判断） |
| `CustonWhere` | nvarchar(2000) | 自定义同步条件 |
| `MaxSynCount` | int | 每次同步最大行数 |
| `LastSynId` | bigint | 上次同步的最大主键 ID |
| `LastSynTime` | datetime2 | 上次同步时间 |
| `WaitingTime` | datetime2 | 下次同步等待时间 |
| `IsEnabled` | bit | 是否启用该同步任务 |

---

### 3. `GlobalConfig` - 全局配置表

| 字段名 | 类型 | 说明 |
|-------|------|------|
| `Id` | int | 主键 |
| `MaxDelCount` | int | 全局默认每次删除最大行数 |
| `MaxSyncCount` | int | 全局默认每次同步最大行数 |
| `SourceConn` | nvarchar(1000) | 源数据库连接字符串 |
| `TargetConn` | nvarchar(1000) | 目标数据库连接字符串（同步用） |
| `IsEnabledDel` | bit | 是否启用整体删除功能 |
| `IsEnabledSync` | bit | 是否启用整体同步功能 |
| `IsEnabled` | bit | 是否启用本服务 |

> ⚠️ 注意：表中字段 `CustonWhere` 应为 `CustomWhere` 的拼写错误，建议后续修正。

---

### 4. `DeleteFileConfig` - 文件删除配置表

| 字段名 | 类型 | 说明 |
|-------|------|------|
| `Id` | int | 主键 |
| `ClientCode` | nvarchar(200) | 客户端代码（区分不同服务节点） |
| `Path` | nvarchar(500) | 要清理的目录路径 |
| `KeepDays` | int | 保留最近 N 天的文件（按最后修改时间） |
| `MaxDeleteCount` | int | 每次最大删除文件数（0 表示不限制） |
| `IncludeSubDirectories` | bit | 是否递归扫描子目录 |
| `IsEnabled` | bit | 是否启用该删除任务 |

---

### 5. `MoveFileConfig` - 文件迁移配置表

| 字段名 | 类型 | 说明 |
|-------|------|------|
| `Id` | int | 主键 |
| `ClientCode` | nvarchar(200) | 客户端代码（区分不同服务节点） |
| `SourcePath` | nvarchar(500) | 源目录路径 |
| `TargetPath` | nvarchar(500) | 目标目录路径 |
| `KeepDays` | int | 保留最近 N 天的文件（按最后修改时间） |
| `MaxMoveCount` | int | 每次最大迁移文件数（0 表示不限制） |
| `IncludeSubDirectories` | bit | 是否递归迁移（按原目录层级迁移） |
| `IsEnabled` | bit | 是否启用该迁移任务 |

---

### 6. `RunningLog` - 运行日志表

记录程序执行过程中的操作日志，便于排查问题。

| 字段名 | 类型 | 说明 |
|-------|------|------|
| `Id` | bigint | 主键 |
| `TypeName` | nvarchar(50) | 日志类型（如 "Delete", "Sync"） |
| `TableName` | nvarchar(200) | 涉及的表名 |
| `Description` | nvarchar(2000) | 详细描述（如执行 SQL、结果） |
| `CreateTime` | datetime | 日志生成时间 |

---

## 使用说明

1. **配置数据库连接**  
   在 `GlobalConfig` 表中设置 `SourceConn` 和 `TargetConn`（同步时使用）。

2. **配置文件删除任务**
   向 `DeleteFileConfig` 插入记录，设置 `ClientCode`（与 `appsettings.json` 中 `Auth.ClientCode` 一致）、目录路径、保留天数等。

3. **配置文件迁移任务**
   向 `MoveFileConfig` 插入记录，设置 `ClientCode`（与 `appsettings.json` 中 `Auth.ClientCode` 一致）、源目录路径、目标目录路径、保留天数等。

4. **配置数据删除任务**
   向 `DelConfig` 插入记录，设置表名、保留天数、删除条件等。

5. **配置同步任务**
   向 `SyncConfig` 插入记录，设置表名、字段、同步条件等。

6. **启用任务**
   将对应配置的 `IsEnabled` 设置为 `1`，并确保 `GlobalConfig` 中的功能开关已开启。

7. **启动服务**  
   运行程序，服务将根据配置周期性执行任务。

---

## 安全建议

- 建议在低峰期执行删除/同步任务。
- 所有删除操作建议先在测试环境验证。
- 定期备份重要数据，防止误删。
- 使用连接池与事务控制，确保数据一致性。

---

## 联系方式

如有问题，请联系维护团队或提交 Issue。

📅 最后更新：2025年10月11日
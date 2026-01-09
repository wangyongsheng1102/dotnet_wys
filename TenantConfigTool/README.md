# 租户配置生成工具

一个基于 .NET 8 和 Avalonia 的桌面应用程序，用于根据基础租户配置自动生成新的租户配置文件。

## 功能特性

- ✅ 递归扫描基础项目目录，查找包含租户代码的配置文件
- ✅ 支持的文件类型：`.properties`, `.yml`, `.yaml`, `.xml`
- ✅ 自动替换文件名和文件内容中的租户代码（保持大小写）
- ✅ 预览功能：在生成前查看将要创建的文件列表
- ✅ 日志记录：实时显示处理进度和错误信息
- ✅ 覆盖选项：可选择是否覆盖已存在的文件

## 技术栈

- .NET 8
- Avalonia UI 11.0.7
- CommunityToolkit.Mvvm 8.2.2
- MVVM 架构模式
- 依赖注入 (Microsoft.Extensions.DependencyInjection)

## 项目结构

```
TenantConfigTool/
├── Models/
│   └── FileMapping.cs          # 文件映射模型
├── Services/
│   ├── IFileScanService.cs     # 文件扫描服务接口
│   ├── FileScanService.cs      # 文件扫描服务实现
│   ├── ITenantReplaceService.cs # 租户代码替换服务接口
│   ├── TenantReplaceService.cs  # 租户代码替换服务实现
│   ├── IFileCopyService.cs     # 文件复制服务接口
│   └── FileCopyService.cs      # 文件复制服务实现
├── ViewModels/
│   └── MainViewModel.cs        # 主视图模型
├── Views/
│   ├── MainWindow.axaml        # 主窗口 XAML
│   └── MainWindow.axaml.cs     # 主窗口代码后置
├── App.axaml                   # 应用程序 XAML
├── App.axaml.cs                # 应用程序代码后置
├── Program.cs                  # 程序入口
└── TenantConfigTool.csproj     # 项目文件
```

## 使用方法

### 1. 构建项目

```bash
dotnet restore
dotnet build
```

### 2. 运行应用

```bash
dotnet run
```

### 3. 使用步骤

1. **选择基础项目路径**：点击"浏览..."按钮，选择包含基础租户配置的项目目录
2. **选择目标项目路径**：点击"浏览..."按钮，选择要生成新配置的目标项目目录
3. **输入基础租户代码**：例如 `aaa`
4. **输入新租户代码**：例如 `bbb`
5. **选择是否覆盖**：勾选"覆盖已存在的文件"选项（可选）
6. **预览**：点击"预览"按钮查看将要生成的文件列表
7. **生成**：点击"生成"按钮开始生成配置文件

## 替换规则

### 文件名替换

- `application-aaa.properties` → `application-bbb.properties`
- `datasource-aaa.properties` → `datasource-bbb.properties`
- `redis-aaa.yml` → `redis-bbb.yml`

### 文件内容替换

文件内容中的租户代码会被替换，并保持原有的大小写格式：

- `aaa` → `bbb`
- `AAA` → `BBB`
- `Aaa` → `Bbb`

## 示例

假设基础项目中有以下文件：

```
baseProject/
└── config/
    ├── application-aaa.properties
    ├── datasource-aaa.properties
    └── db/
        └── redis-aaa.yml
```

使用工具将 `aaa` 替换为 `bbb` 后，目标项目将生成：

```
targetProject/
└── config/
    ├── application-bbb.properties
    ├── datasource-bbb.properties
    └── db/
        └── redis-bbb.yml
```

## 系统要求

- .NET 8 SDK
- Windows 操作系统（目标平台）

## 许可证

本项目为示例项目，可根据需要修改和使用。

# CommunityToolkit.Mvvm.Extension

> **语言**: **中文** · [English](README.md)

`CommunityToolkit.Mvvm.Extension` 是一款为 **CommunityToolkit.Mvvm** 量身打造的功能扩展骨架包，在保留 CommunityToolkit.Mvvm 轻量、现代、强类型优势的同时，为 WPF 应用补上企业级 MVVM 框架所需的三大核心能力：**导航（Navigation）**、**对话框（Dialog）** 与 **模块化（Module / MEF）**。

它借鉴了 Prism 等成熟框架的优秀设计理念与命名习惯，但**不引入 Prism 的依赖与复杂度**，直接构建在 `Microsoft.Extensions.DependencyInjection` 之上，真正做到开箱即用、零侵入、渐进式采用。

> 一句话概括：**用 CommunityToolkit.Mvvm 的轻量写法，享受 Prism 级别的框架能力。**

---

## 目录

- [核心特性](#核心特性)
- [为什么选择本扩展包](#为什么选择本扩展包)
- [项目结构](#项目结构)
- [快速开始](#快速开始)
- [功能详解一：导航 Navigation](#功能详解一导航-navigation)
- [功能详解二：对话框 Dialog](#功能详解二对话框-dialog)
- [功能详解三：模块化 Module / MEF](#功能详解三模块化-module--mef)
- [依赖注入增强](#依赖注入增强)
- [应用基类 Application](#应用基类-application)
- [完整示例](#完整示例)
- [API 速查表](#api-速查表)
- [常见问题 FAQ](#常见问题-faq)
- [兼容性](#兼容性)

---

## 核心特性

| 能力模块 | 说明 | 亮点 |
| --- | --- | --- |
| **导航 Navigation** | 具名区域导航，支持前进 / 后退、参数传递、视图复用、生命周期回调 | 区域自动适配、导航日志、离开确认 |
| **对话框 Dialog** | 模态 / 非模态对话框服务，支持参数进出与关闭回调 | 与 MVVM 无缝结合、可替换窗口实现 |
| **模块化 Module** | 模块发现、按需加载、依赖注入与启动初始化编排 | 支持程序集扫描、目录扫描、延迟加载 |
| **依赖注入** | 贴近 Prism 命名的 DI 扩展方法 | `Register` / `RegisterSingleton` / `Resolve` / 具名注册 |
| **应用基类** | 抽象 `Application` 类，编排完整的启动流程 | 一站式完成容器、Shell、模块、导航初始化 |

---

## 为什么选择本扩展包

### 1. 与 CommunityToolkit.Mvvm 完美融合
本扩展包直接命名在 `CommunityToolkit.Mvvm` 命名空间下，与官方 `ObservableObject`、`RelayCommand`、`ObservableProperty` 等特性完全兼容，**无需改造现有 ViewModel** 即可获得框架能力。

### 2. 零额外依赖，只依赖官方 DI 容器
整个扩展包仅依赖 `Microsoft.Extensions.DependencyInjection`，不引入任何重量级框架，**项目体积小、启动快、可预测性强**。

### 3. Prism 风格的开发体验
`RegisterForNavigation`、`RegisterForDialog`、`ConfirmNavigationRequest`、`OnNavigatedTo` 等 API 在命名与语义上都贴近 Prism，**熟悉 Prism 的开发者可以无学习成本上手**。

### 4. 完整的 MVVM 能力闭环
- **视图注册 → 区域导航 → 生命周期回调 → 历史记录 → 离开确认** 全链路打通；
- **对话框打开 → 参数传递 → 结果返回 → 关闭清理** 全流程可控；
- **模块发现 → 服务注册 → 自动加载 → 按需激活** 全阶段管理。

### 5. 面向真实企业级应用设计
内置了视图缓存复用、单活动区域语义、导航回滚、事件自动解绑、区域随窗口关闭自动释放等工程化细节，**让应用在复杂场景下依然稳定可靠**。

### 6. 渐进式采用
可以只用导航、只用对话框、只用模块，也可以全部启用；**不必一次性重构整个项目**，按需引入即可获得收益。

### 7. 多目标框架支持
同一套 API 覆盖从传统 .NET Framework 到最新 .NET 的广泛平台，**存量项目与新建项目可共用同一套代码范式**。

---

## 项目结构

```
CommunityToolkit.Mvvm.Extension/
├── src/
│   ├── Code/
│   │   ├── FilexUI.CommunityToolkit.Mvvm.Extension/          # 核心抽象层（跨平台无关）
│   │   │   ├── Attributes/
│   │   │   │   └── ModuleAttribute.cs                        # 模块特性（名称/描述/自动加载）
│   │   │   ├── Extensions/
│   │   │   │   ├── MEFExtension.cs                           # 模块系统扩展方法 UseMEF / AddModule
│   │   │   │   └── ServiceCollectionExtension.cs             # Prism 风格 DI 扩展方法
│   │   │   ├── Interfaces/                                   # 契约定义
│   │   │   │   ├── IConfirmNavigationRequest.cs              # 导航离开确认
│   │   │   │   ├── IDialogAware.cs                           # 对话框生命周期
│   │   │   │   ├── IDialogResult.cs / IDialogService.cs      # 对话框结果与服务
│   │   │   │   ├── IDialogWindow.cs                          # 对话框窗口抽象
│   │   │   │   ├── IMEFService.cs / IModule.cs / IModuleInfo.cs   # 模块系统契约
│   │   │   │   ├── INavigationAdapter.cs                     # 区域适配器契约
│   │   │   │   ├── INavigationAware.cs                       # 导航生命周期
│   │   │   │   ├── INavigationJournal.cs                     # 导航日志
│   │   │   │   ├── INavigationJournalEntry.cs                # 导航日志项
│   │   │   │   ├── INavigationLifecycle.cs                   # 创建 / 销毁生命周期
│   │   │   │   ├── INavigationLifetime.cs                    # 页面保留策略
│   │   │   │   ├── INavigationService.cs                     # 导航服务
│   │   │   │   └── IRegion.cs                                # 区域抽象
│   │   │   ├── Models/                                       # 默认实现
│   │   │   │   ├── AllActiveRegion.cs / SingleActiveRegion.cs / Region.cs
│   │   │   │   ├── ButtonResult.cs / DialogResult.cs
│   │   │   │   ├── DialogViewRegistration.cs / NavigationViewRegistration.cs
│   │   │   │   ├── ModuleInfo.cs
│   │   │   │   ├── NavigationContext.cs / NavigationJournalEntry.cs
│   │   │   │   └── Parameter.cs                              # 导航 / 对话框参数容器
│   │   │   ├── Options/
│   │   │   │   └── MEFOptions.cs                             # 模块发现配置
│   │   │   └── Services/
│   │   │       └── MEFService.cs                             # 模块管理器默认实现
│   │   │
│   │   └── FilexUI.CommunityToolkit.Mvvm.Extension.WPF/      # WPF 平台实现层
│   │       ├── Adapters/                                     # 区域适配器
│   │       │   ├── NavigationAdapterBase.cs
│   │       │   ├── ContentControlAdapter.cs
│   │       │   ├── ItemsControlAdapter.cs
│   │       │   └── TabControlAdapter.cs
│   │       ├── Extensions/
│   │       │   ├── DialogExtension.cs                        # UseDialog / RegisterForDialog
│   │       │   └── NavigationExtension.cs                    # UseNavigation / RegisterForNavigation
│   │       ├── Services/
│   │       │   ├── DialogService.cs / DialogWindow.cs        # 对话框服务与默认窗口
│   │       │   ├── NavigationJournal.cs                      # 导航日志实现
│   │       │   └── NavigationService.cs                      # 导航服务实现
│   │       └── Application.cs                                # 应用启动基类
│   │
│   └── Demo/
│       └── CommunityToolkit.Mvvm.Extension.WPFShell/         # 示例工程（Shell 启动范式）
```

---

## 快速开始

### 1. 引用

在 WPF 项目中引用以下程序集（或通过 NuGet 包引用）：

```
FilexUI.CommunityToolkit.Mvvm.Extension
FilexUI.CommunityToolkit.Mvvm.Extension.WPF
```

### 2. 继承应用基类

```csharp
using CommunityToolkit.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MyApp
{
    public partial class App : Application
    {
        // 1) 创建 Shell 窗口
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        // 2) 注册服务与视图
        protected override void RegisterServices(IServiceCollection services)
        {
            services.RegisterForNavigation<MainWindow>();
            // ... 其他注册
        }

        // 3)（可选）配置模块扫描
        protected override void CreateModule(MEFOptions options)
        {
            options.AddModule<CoreModule>();
        }
    }
}
```

### 3. 一行代码启用全部能力

`Application` 基类已在启动流程中自动完成：

```csharp
services.UseNavigation().UseDialog().UseMEF(CreateModule);
```

即：**注册导航服务 + 注册对话框服务 + 启用模块系统**，随后构建容器、创建 Shell 并初始化自动加载模块。

### 4. 运行

Shell 窗口显示后，即可在任意 ViewModel 中通过构造函数注入使用 `INavigationService`、`IDialogService`、`IMEFService`。

---

## 功能详解一：导航 Navigation

### 4.1 定义区域（Region）

区域是承载视图的"插槽"，通过附加属性 `RegionName` 在 XAML 上声明：

```xml
<Window x:Class="MyApp.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:mvvm="clr-namespace:CommunityToolkit.Mvvm;assembly=FilexUI.CommunityToolkit.Mvvm.Extension.WPF">
    <Grid>
        <!-- 内容区：一次显示一个视图 -->
        <ContentControl mvvm:NavigationService.RegionName="ContentRegion" />
    </Grid>
</Window>
```

内置三种区域适配器，**自动匹配对应控件类型**：

| 控件类型 | 适配器 | 区域模型 | 场景 |
| --- | --- | --- | --- |
| `ContentControl` | `ContentControlAdapter` | `SingleActiveRegion` | 主内容区、单视图切换 |
| `TabControl` | `TabControlAdapter` | `SingleActiveRegion` | 多标签页导航（自动同步选中项） |
| `ItemsControl` | `ItemsControlAdapter` | `SingleActiveRegion` | 列表式视图容器 |

### 4.2 注册视图

```csharp
// 使用视图类型名作为导航名称（推荐约定）
services.RegisterForNavigation<HomeView>();

// 自定义导航名称
services.RegisterForNavigation<HomeView>("Home");

// 显式指定视图模型
services.RegisterForNavigation<HomeView, HomeViewModel>("Home");

// 自定义视图模型解析规则
services.RegisterForNavigation<HomeView>(
    "Home",
    viewType => typeof(HomeViewModel));
```

**命名约定自动关联 ViewModel**：当视图位于 `*.Views.*` 命名空间时，框架会自动查找对应的 `*.ViewModels.*` 命名空间下的同名 ViewModel（`XxxView` → `XxxViewModel`），并自动绑定到 `DataContext`，**无需手写一行绑定代码**。

### 4.3 执行导航

```csharp
public class HomeViewModel
{
    private readonly INavigationService navigationService;

    public HomeViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }

    // 简单导航
    public void GoDetail() => navigationService.Navigate("ContentRegion", "Detail");

    // 带参数导航
    public void GoDetailWithArgs()
    {
        var parameter = new Parameter();
        parameter.Add("Id", 1001);
        parameter.Add("Title", "订单详情");
        navigationService.Navigate("ContentRegion", "Detail", parameter);
    }

    // 异步导航（可感知是否被取消）
    public async Task GoDetailAsync()
    {
        var parameter = new Parameter();
        parameter.Add("Id", 1001);
        bool succeeded = await navigationService.NavigateAsync("ContentRegion", "Detail", parameter);
    }
}
```

### 4.4 导航日志（前进 / 后退）

```csharp
navigationService.CanGoBack("ContentRegion");     // 是否可后退
navigationService.CanGoForward("ContentRegion");  // 是否可前进
navigationService.GoBack("ContentRegion");        // 后退
navigationService.GoForward("ContentRegion");     // 前进

var journal = navigationService.GetJournal("ContentRegion");
var current = journal.CurrentEntry;               // 当前日志项
journal.Clear();                                  // 清空历史
```

日志采用双堆栈设计，**普通导航会清空前进堆栈**，历史重放会复用已有视图实例（不会重复创建）。

### 4.5 生命周期回调

视图或其 ViewModel 实现以下接口即可参与导航生命周期（**优先使用 `DataContext`，即 ViewModel 优先**）：

```csharp
public class DetailViewModel : INavigationAware, INavigationLifecycle, INavigationLifetime
{
    // ---- INavigationAware：每次导航都会被调用 ----
    public bool IsNavigationTarget(NavigationContext context)
    {
        // 返回 true 表示可复用当前实例（例如同一订单仅刷新数据）
        return context.Parameter.GetValue<int>("Id") == currentId;
    }

    public void OnNavigatedTo(NavigationContext context)
    {
        var id = context.Parameter.GetValue<int>("Id");
        // 加载数据...
    }

    public void OnNavigatedFrom(NavigationContext context)
    {
        // 保存状态、取消订阅...
    }

    // ---- INavigationLifecycle：仅创建 / 销毁时调用一次 ----
    public void OnInitialized(NavigationContext context)
    {
        // 首次创建后的初始化
    }

    public void OnDestroyed()
    {
        // 被移出区域、不再复用时清理资源
    }

    // ---- INavigationLifetime：控制页面是否常驻 ----
    public bool KeepAlive => true;   // false 表示离开区域后即移除
}
```

| 接口 | 调用时机 | 典型用途 |
| --- | --- | --- |
| `INavigationAware.OnNavigatedTo` | 每次成为活动视图后 | 加载 / 刷新数据 |
| `INavigationAware.OnNavigatedFrom` | 每次离开前 | 保存状态、取消订阅 |
| `INavigationLifecycle.OnInitialized` | 实例首次创建后（仅一次） | 一次性初始化 |
| `INavigationLifecycle.OnDestroyed` | 实例被移出区域（仅一次） | 释放资源 |
| `INavigationLifetime.KeepAlive` | 离开区域时判定 | 控制视图缓存复用 |

### 4.6 离开确认（拦截导航）

实现 `IConfirmNavigationRequest` 可以在离开前拦截导航（例如"有未保存内容，确定离开吗？"）：

```csharp
public class EditorViewModel : IConfirmNavigationRequest
{
    public bool HasUnsavedChanges { get; set; }

    public void ConfirmNavigationRequest(NavigationContext context, Action<bool> continuationCallback)
    {
        if (!HasUnsavedChanges)
        {
            continuationCallback(true);   // 允许继续
            return;
        }

        var result = MessageBox.Show("有未保存的修改，确定离开吗？", "提示",
                                     MessageBoxButton.YesNo, MessageBoxImage.Question);

        continuationCallback(result == MessageBoxResult.Yes);   // 由用户决定
    }
}
```

回调为异步设计，**可以是"先弹对话框、用户点击确认后再继续"** 的任意异步流程；未被确认时导航会被安全取消，且不会污染导航日志。

### 4.7 释放区域

```csharp
navigationService.RemoveRegion("ContentRegion");
```

会解除区域注册、解除所有事件订阅、清空视图与日志引用，**在窗口关闭时自动触发**，无需手动管理。

---

## 功能详解二：对话框 Dialog

### 5.1 启用对话框服务

```csharp
services.UseDialog();
```

### 5.2 注册对话框视图

```csharp
services.RegisterForDialog<EditDialogView>();             // 使用类型名
services.RegisterForDialog<EditDialogView>("EditDialog");  // 自定义名称
```

### 5.3 对话框视图模型

实现 `IDialogAware` 即可拥有完整的对话框生命周期：

```csharp
public class EditDialogViewModel : IDialogAware
{
    public string Title => "编辑信息";

    public event Action<IDialogResult> RequestClose;

    public bool CanCloseDialog() => !HasUnsavedChanges;   // 保护未保存内容

    public void OnDialogOpened(Parameter parameter)
    {
        var id = parameter.GetValue<int>("Id");
        // 初始化编辑数据...
    }

    public void OnDialogClosed()
    {
        // 清理订阅、释放资源...
    }

    // 由界面上的"确定"按钮调用
    public void Confirm()
    {
        var result = new Parameter();
        result.Add("Saved", true);
        RequestClose?.Invoke(new DialogResult(ButtonResult.OK, result));
    }

    // 由界面上的"取消"按钮调用
    public void Cancel() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
}
```

### 5.4 打开对话框

```csharp
public class MainViewModel
{
    private readonly IDialogService dialogService;

    public MainViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;
    }

    // 模态：阻塞并直接返回结果
    public void ShowModal()
    {
        var parameter = new Parameter();
        parameter.Add("Id", 1001);

        IDialogResult result = dialogService.ShowDialog("EditDialog", parameter);
        if (result.Result == ButtonResult.OK)
        {
            bool saved = result.Parameter.GetValue<bool>("Saved");
        }
    }

    // 非模态：通过回调获取结果
    public void ShowNonModal()
    {
        dialogService.Show("EditDialog", null, result =>
        {
            if (result.Result == ButtonResult.OK)
            {
                // 处理结果...
            }
        });
    }
}
```

| 方法 | 行为 | 返回 |
| --- | --- | --- |
| `Show(name)` | 非模态打开 | 无 |
| `Show(name, parameter)` | 非模态打开并传参 | 无 |
| `Show(name, parameter, callback)` | 非模态打开并在关闭后回调 | 无 |
| `ShowDialog(name)` | 模态打开 | `IDialogResult` |
| `ShowDialog(name, parameter)` | 模态打开并传参 | `IDialogResult` |

### 5.5 自定义对话框窗口

默认使用内置 `DialogWindow`（自动居中于主窗口、自适应内容尺寸）。如需自定义外观：

```csharp
services.RegisterDialogWindow<MyCustomDialogWindow>();

public class MyCustomDialogWindow : Window, IDialogWindow
{
    object IDialogWindow.Owner
    {
        get => Owner;
        set => Owner = value as Window;
    }
}
```

也可通过附加属性为单个对话框视图指定窗口样式：

```xml
<UserControl ... mvvm:DialogService.Style="{StaticResource DialogStyle}" />
```

### 5.6 对话框结果语义

```csharp
public enum ButtonResult
{
    None,    // 未提供明确结果
    OK,      // 已确认
    Cancel,  // 已取消
    Yes,     // "是"
    No       // "否"
}
```

模态对话框会将 `OK` / `Yes` 映射为窗口 `DialogResult = true`，`Cancel` / `No` 映射为 `false`，**与 WPF 原生对话框语义保持一致**。

---

## 功能详解三：模块化 Module / MEF

### 6.1 定义模块

```csharp
[Module(Name = "Core", Description = "核心业务模块", AutoLoad = true)]
public class CoreModule : IModule
{
    // 在根服务提供程序构建前调用：可注册任何依赖
    public void RegisterServices(IServiceCollection services)
    {
        services.RegisterSingleton<IOrderService, OrderService>();
        services.RegisterForNavigation<OrderView, OrderViewModel>("Order");
    }

    // 在容器构建完成后调用：适合加载数据、发起首次导航
    public void OnInitialized(IServiceProvider provider)
    {
        var navigation = provider.Resolve<INavigationService>();
        navigation.Navigate("ContentRegion", "Order");
    }
}
```

| `ModuleAttribute` 成员 | 说明 | 默认值 |
| --- | --- | --- |
| `Name` | 模块显示名称 | 类型名称 |
| `Description` | 模块描述 | 空 |
| `AutoLoad` | 是否启动时自动初始化 | `true` |

**关键特性**：无论是否自动加载，**所有已发现模块的 `RegisterServices` 都会在容器构建前执行**，因此模块间依赖可以自由交叉注册。

### 6.2 配置模块发现

```csharp
services.UseMEF(options =>
{
    // 方式一：扫描指定程序集
    options.AddModule<CoreModule>();
    options.AddModule(typeof(PluginModule).Assembly);

    // 方式二：扫描指定目录（插件热插拔场景）
    options.AddDirectory(@"C:\MyApp\Modules");
    options.AddDirectory("Modules");          // 相对应用程序基目录

    // 自定义文件匹配模式
    options.SearchPattern = "*.dll";

    // 发现完成回调：可用于日志、诊断
    options.OnModulesDiscovered(modules =>
    {
        foreach (var module in modules)
        {
            Console.WriteLine($"发现模块：{module.Name}");
        }
    });
});
```

**零配置默认行为**：未指定任何扫描来源时，默认扫描**应用程序基目录**及其下的 **`Modules` 目录**，开箱即用。

### 6.3 管理模块

```csharp
var mef = Container.Resolve<IMEFService>();

mef.DiscoveredModules;   // 所有已发现的模块
mef.AutoLoadModules;     // 将自动初始化的模块
mef.Modules;             // 已初始化完成的模块

mef.ModuleLoaded += info => Console.WriteLine($"模块已加载：{info.Name}");

// 按需激活（AutoLoad = false 的模块）
mef.LoadModule("Plugin");
mef.LoadModule<PluginModule>();
mef.LoadModule(typeof(PluginModule));

// 直接从代码注册模块（跳过扫描）
services.AddModule<CoreModule>();
```

该模块系统是**轻量级模块管理器**（非官方 .NET MEF 实现），聚焦于模块发现、服务注册与启动编排，**逻辑清晰、行为可预期**。

---

## 依赖注入增强

`ServiceCollectionExtension` 提供了贴近 Prism 命名的 DI 便捷方法，让注册与解析代码更简洁统一。

### 注册服务

```csharp
// 瞬态注册
services.Register<IMyService, MyService>();
services.Register<MyService>();

// 单例注册
services.RegisterSingleton<IMyService, MyService>();
services.RegisterSingleton<MyService>();

// 使用已有实例注册
services.RegisterInstance<IMyService>(myService);

// 具名注册（同类型多实现场景）
services.Register<IMyService, MyService>("A");
services.RegisterSingleton<IMyService, MyService>("B");
services.RegisterInstance<IMyService>(myService, "C");
```

### 解析服务

```csharp
var service = provider.Resolve<IMyService>();           // 解析（未注册会抛出明确异常）
var named   = provider.Resolve<IMyService>("A");        // 按名称解析
var ok      = provider.IsRegistered<IMyService>();      // 是否已注册
var okNamed = provider.IsRegistered<IMyService>("A");   // 具名是否已注册
```

### 检查注册状态

```csharp
services.IsRegistered<IMyService>();
services.IsRegistered<IMyService>("A");
services.IsRegistered(typeof(IMyService), "A");
```

所有方法均支持**链式调用**（返回同一 `IServiceCollection`），也内置了**完整的参数校验**（空值、类型可赋值性、名称非空等），让配置错误在启动阶段就能被明确发现。

---

## 应用基类 Application

`CommunityToolkit.Mvvm.Application` 抽象类编排了标准的 WPF 启动流程：

```
OnStartup
   └─ InitIoc()
        ├─ RegisterServices(services)                    ← 子类注册服务
        ├─ services.UseNavigation().UseDialog().UseMEF(CreateModule)
        └─ BuildServiceProvider()                        ← 构建容器
   ├─ Initialize()
   │     ├─ CreateShell()                                ← 子类创建 Shell（抽象）
   │     └─ InitializeShell(shell)                       ← 默认设为 MainWindow（可重写）
   ├─ Container.UseMEF()                                 ← 初始化自动加载模块
   └─ OnInitialized()                                    ← 默认显示主窗口（可重写）
```

可重写扩展点：

| 成员 | 说明 | 是否必须实现 |
| --- | --- | --- |
| `RegisterServices(IServiceCollection)` | 注册应用服务与视图 | **是**（抽象） |
| `CreateShell()` | 创建主窗口 | **是**（抽象） |
| `InitializeShell(Window)` | 决定主窗口如何呈现 | 否 |
| `CreateModule(MEFOptions)` | 配置模块扫描 | 否 |
| `OnInitialized()` | 启动完成后的收尾逻辑 | 否 |
| `Container` 属性 | 全局根服务提供程序 | — |

**启动顺序经过精心编排**：Shell 构造期间注册导航区域，模块初始化在容器就绪后执行，因此**模块可以在 `OnInitialized` 中安全地发起首次导航**。

---

## 完整示例

### 场景：主窗口内导航到订单页，并弹出编辑对话框

**1) App.xaml.cs**

```csharp
public partial class App : CommunityToolkit.Mvvm.Application
{
    protected override Window CreateShell() => Container.Resolve<MainWindow>();

    protected override void RegisterServices(IServiceCollection services)
    {
        services.RegisterForNavigation<MainWindow, MainWindowViewModel>();
        services.RegisterForNavigation<OrderView, OrderViewModel>("Order");
        services.RegisterForDialog<EditDialogView, EditDialogViewModel>("EditDialog");
    }

    protected override void CreateModule(MEFOptions options)
    {
        options.AddModule<CoreModule>();
    }
}
```

**2) MainWindow.xaml（定义区域）**

```xml
<Window x:Class="MyApp.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:mvvm="clr-namespace:CommunityToolkit.Mvvm;assembly=FilexUI.CommunityToolkit.Mvvm.Extension.WPF"
        Title="MyApp" Width="900" Height="600">
    <DockPanel>
        <StackPanel DockPanel.Dock="Top" Orientation="Horizontal">
            <Button Content="订单" Command="{Binding GoOrderCommand}" />
            <Button Content="后退" Command="{Binding GoBackCommand}" />
        </StackPanel>
        <ContentControl mvvm:NavigationService.RegionName="ContentRegion" />
    </DockPanel>
</Window>
```

**3) MainWindowViewModel.cs**

```csharp
public partial class MainWindowViewModel : ObservableObject
{
    private readonly INavigationService navigation;

    public MainWindowViewModel(INavigationService navigation)
    {
        this.navigation = navigation;
    }

    [RelayCommand]
    private void GoOrder() => navigation.Navigate("ContentRegion", "Order");

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void GoBack() => navigation.GoBack("ContentRegion");

    private bool CanGoBack() => navigation.CanGoBack("ContentRegion");
}
```

**4) OrderViewModel.cs（含对话框交互）**

```csharp
public partial class OrderViewModel : ObservableObject, INavigationAware
{
    private readonly IDialogService dialogService;

    public OrderViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;
    }

    public void OnNavigatedTo(NavigationContext context)
    {
        // 页面显示时加载数据
    }

    public void OnNavigatedFrom(NavigationContext context)
    {
        // 页面离开时保存状态
    }

    public bool IsNavigationTarget(NavigationContext context) => true;

    [RelayCommand]
    private void Edit()
    {
        var parameter = new Parameter();
        parameter.Add("OrderId", 1001);

        var result = dialogService.ShowDialog("EditDialog", parameter);
        if (result.Result == ButtonResult.OK)
        {
            // 处理保存结果
        }
    }
}
```

---

## API 速查表

### 服务集合扩展

| 方法 | 所属 | 作用 |
| --- | --- | --- |
| `UseNavigation()` | `NavigationExtension` | 注册导航服务与内置适配器 |
| `UseDialog()` | `DialogExtension` | 注册对话框服务与默认窗口 |
| `UseMEF(Action<MEFOptions>)` | `MEFExtension` | 启用模块发现与注册 |
| `UseMEF()` | `MEFExtension` | 启用模块发现（默认扫描目录） |
| `AddModule<T>()` | `MEFExtension` | 直接注册模块 |
| `RegisterForNavigation<TView>()` | `NavigationExtension` | 注册导航视图（约定查找 ViewModel） |
| `RegisterForNavigation<TView, TViewModel>(name)` | `NavigationExtension` | 注册导航视图并指定 ViewModel |
| `RegisterNavigationAdapter<T>(adapter)` | `NavigationExtension` | 注册 / 替换区域适配器 |
| `RegisterForDialog<TView>(name)` | `DialogExtension` | 注册对话框视图 |
| `RegisterDialogWindow<TWindow>()` | `DialogExtension` | 替换默认对话框窗口 |
| `Register` / `RegisterSingleton` / `RegisterInstance` | `ServiceCollectionExtension` | Prism 风格服务注册 |
| `Resolve` / `IsRegistered` | `ServiceCollectionExtension` | 服务解析与注册检查 |

### 运行时服务

| 接口 | 核心成员 | 用途 |
| --- | --- | --- |
| `INavigationService` | `Navigate` / `NavigateAsync` / `GoBack` / `GoForward` / `CanGoBack` / `CanGoForward` / `GetJournal` / `RemoveRegion` | 区域导航 |
| `IDialogService` | `Show` / `ShowDialog` | 对话框 |
| `IMEFService` | `DiscoveredModules` / `AutoLoadModules` / `Modules` / `ModuleLoaded` / `DiscoverModules` / `LoadModule` | 模块管理 |

### 生命周期接口

| 接口 | 成员 | 调用时机 |
| --- | --- | --- |
| `INavigationAware` | `IsNavigationTarget` / `OnNavigatedTo` / `OnNavigatedFrom` | 每次导航 |
| `INavigationLifecycle` | `OnInitialized` / `OnDestroyed` | 创建 / 销毁各一次 |
| `INavigationLifetime` | `KeepAlive` | 离开区域时 |
| `IConfirmNavigationRequest` | `ConfirmNavigationRequest` | 导航离开前 |
| `IDialogAware` | `Title` / `RequestClose` / `CanCloseDialog` / `OnDialogOpened` / `OnDialogClosed` | 对话框全生命周期 |

---

## 常见问题 FAQ

**Q1：必须使用内置的 `Application` 基类吗？**

不需要。基类只是把标准启动流程封装好，你完全可以在自己的 `App` 中手动调用 `UseNavigation()`、`UseDialog()`、`UseMEF()` 并按需构建容器。

**Q2：支持哪些区域承载控件？**

内置 `ContentControl`、`TabControl`、`ItemsControl` 三种适配器。若需其他控件，实现 `INavigationAdapter<T>` 后通过 `RegisterNavigationAdapter` 注册即可，**扩展方式简单统一**。

**Q3：视图会被重复创建吗？**

不会。导航服务会优先复用区域中的既有实例，并通过 `IsNavigationTarget` 判定复用条件；历史前进 / 后退同样复用实例。

**Q4：如何让某个页面离开后不再保留？**

在 ViewModel 中实现 `INavigationLifetime`，令 `KeepAlive => false` 即可。

**Q5：模块的 `RegisterServices` 什么时候执行？**

在根服务提供程序构建**之前**，因此模块注册的服务可被其他模块和宿主应用共同使用。

**Q6：如何设置对话框的标题？**

ViewModel 实现 `IDialogAware`，在 `Title` 属性中返回；未指定时使用注册名称。

**Q7：ViewModel 需要实现导航接口吗，还是 View 实现？**

框架会**先检查 `DataContext`（即 ViewModel），再检查视图本身**，推荐在 ViewModel 中实现，更符合 MVVM 原则。

---

## 兼容性

本扩展包提供广泛的多目标框架支持：

- **.NET Framework**：`net4.5` / `net4.5.2` / `net4.6` / `net4.6.2` / `net4.7` / `net4.7.2` / `net4.8`
- **.NET Core / .NET**：`netcoreapp3.0` / `netcoreapp3.1` / `net5.0` / `net6.0` / `net7.0` / `net8.0`（WPF 部分为对应 `-windows` 目标）

**依赖项**：

| 依赖 | 版本 |
| --- | --- |
| `CommunityToolkit.Mvvm` | 8.4.2 |
| `Microsoft.Extensions.DependencyInjection` | 按目标框架自动适配 |

> 说明：不同目标框架的兼容表现可能受具体系统环境影响，跨版本使用前建议在目标环境中实测验证。

---

## 授权与反馈

- 作者：wenke
- 仓库：https://github.com/Coolkeke/CommunityToolkit.Mvvm.Extension

欢迎提交 Issue 与 Pull Request，共同完善这个面向 CommunityToolkit.Mvvm 的扩展生态。

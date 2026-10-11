# CommunityToolkit.Mvvm.Extension

> **Language**: **English** · [Chinese](README_zh.md)

`CommunityToolkit.Mvvm.Extension` is a feature-extension scaffolding package built specifically for **CommunityToolkit.Mvvm**. While preserving the lightweight, modern, and strongly-typed strengths of CommunityToolkit.Mvvm, it adds the three core capabilities an enterprise-grade MVVM framework needs for WPF apps: **Navigation**, **Dialog**, and **Modularization (Module / MEF)**.

It borrows the proven design philosophy and naming conventions of mature frameworks such as Prism, but **without pulling in Prism's dependencies or complexity**. It is built directly on top of `Microsoft.Extensions.DependencyInjection`, delivering a truly out-of-the-box, zero-intrusion, and progressively-adoptable experience.

> In one sentence: **write with the lightweight style of CommunityToolkit.Mvvm, and enjoy Prism-level framework capabilities.**

---

## Table of Contents

- [Core Features](#core-features)
- [Why Choose This Extension Package](#why-choose-this-extension-package)
- [Project Structure](#project-structure)
- [Quick Start](#quick-start)
- [Feature Deep Dive 1: Navigation](#feature-deep-dive-1-navigation)
- [Feature Deep Dive 2: Dialog](#feature-deep-dive-2-dialog)
- [Feature Deep Dive 3: Module / MEF](#feature-deep-dive-3-module--mef)
- [Dependency Injection Enhancements](#dependency-injection-enhancements)
- [Application Base Class](#application-base-class)
- [Complete Example](#complete-example)
- [API Quick Reference](#api-quick-reference)
- [FAQ](#faq)
- [Compatibility](#compatibility)

---

## Core Features

| Capability | Description | Highlights |
| --- | --- | --- |
| **Navigation** | Named-region navigation with forward/back, parameter passing, view reuse, and lifecycle callbacks | Automatic region adaptation, navigation journal, leave confirmation |
| **Dialog** | Modal / non-modal dialog service with parameter in/out and close callbacks | Seamless MVVM integration, replaceable window implementation |
| **Module** | Module discovery, on-demand loading, DI registration, and startup orchestration | Assembly scanning, directory scanning, lazy loading |
| **Dependency Injection** | DI extension methods with Prism-like naming | `Register` / `RegisterSingleton` / `Resolve` / named registration |
| **Application Base Class** | Abstract `Application` class that orchestrates the full startup flow | One-stop initialization of container, Shell, modules, and navigation |

---

## Why Choose This Extension Package

### 1. Perfect Integration with CommunityToolkit.Mvvm
This extension package is declared directly under the `CommunityToolkit.Mvvm` namespace and is fully compatible with the official `ObservableObject`, `RelayCommand`, and `ObservableProperty` features. You get framework capabilities **without modifying any existing ViewModel**.

### 2. Zero Extra Dependencies — Only the Official DI Container
The entire extension package depends solely on `Microsoft.Extensions.DependencyInjection`, introducing no heavyweight framework. This means **smaller project footprint, faster startup, and highly predictable behavior**.

### 3. Prism-Style Development Experience
APIs such as `RegisterForNavigation`, `RegisterForDialog`, `ConfirmNavigationRequest`, and `OnNavigatedTo` closely match Prism in both naming and semantics, so **developers familiar with Prism can get started with zero learning curve**.

### 4. Complete MVVM Capability Loop
- The full chain of **view registration → region navigation → lifecycle callbacks → history → leave confirmation** is wired end to end;
- The complete flow of **dialog open → parameter passing → result return → close cleanup** is fully controllable;
- The full lifecycle of **module discovery → service registration → auto-loading → on-demand activation** is managed at every stage.

### 5. Designed for Real Enterprise Applications
Built-in engineering details such as view caching and reuse, single-active-region semantics, navigation rollback, automatic event unbinding, and automatic region release on window close **keep your app stable and reliable even in complex scenarios**.

### 6. Progressive Adoption
You can enable only navigation, only dialogs, only modules, or all of them at once. There is **no need to refactor the entire project at once** — just bring in what you need to get the benefit.

### 7. Multi-Target Framework Support
The same API covers a wide range of platforms from legacy .NET Framework to the latest .NET, so **both existing and new projects can share the same code paradigm**.

---

## Project Structure

```
CommunityToolkit.Mvvm.Extension/
├── src/
│   ├── Code/
│   │   ├── FilexUI.CommunityToolkit.Mvvm.Extension/          # Core abstraction layer (platform-agnostic)
│   │   │   ├── Attributes/
│   │   │   │   └── ModuleAttribute.cs                        # Module attribute (name/description/auto-load)
│   │   │   ├── Extensions/
│   │   │   │   ├── MEFExtension.cs                           # Module system extensions: UseMEF / AddModule
│   │   │   │   └── ServiceCollectionExtension.cs             # Prism-style DI extension methods
│   │   │   ├── Interfaces/                                   # Contract definitions
│   │   │   │   ├── IConfirmNavigationRequest.cs              # Navigation leave confirmation
│   │   │   │   ├── IDialogAware.cs                           # Dialog lifecycle
│   │   │   │   ├── IDialogResult.cs / IDialogService.cs      # Dialog result and service
│   │   │   │   ├── IDialogWindow.cs                          # Dialog window abstraction
│   │   │   │   ├── IMEFService.cs / IModule.cs / IModuleInfo.cs   # Module system contracts
│   │   │   │   ├── INavigationAdapter.cs                     # Region adapter contract
│   │   │   │   ├── INavigationAware.cs                       # Navigation lifecycle
│   │   │   │   ├── INavigationJournal.cs                     # Navigation journal
│   │   │   │   ├── INavigationJournalEntry.cs                # Navigation journal entry
│   │   │   │   ├── INavigationLifecycle.cs                   # Create / destroy lifecycle
│   │   │   │   ├── INavigationLifetime.cs                    # Page retention policy
│   │   │   │   ├── INavigationService.cs                     # Navigation service
│   │   │   │   └── IRegion.cs                                # Region abstraction
│   │   │   ├── Models/                                       # Default implementations
│   │   │   │   ├── AllActiveRegion.cs / SingleActiveRegion.cs / Region.cs
│   │   │   │   ├── ButtonResult.cs / DialogResult.cs
│   │   │   │   ├── DialogViewRegistration.cs / NavigationViewRegistration.cs
│   │   │   │   ├── ModuleInfo.cs
│   │   │   │   ├── NavigationContext.cs / NavigationJournalEntry.cs
│   │   │   │   └── Parameter.cs                              # Navigation / dialog parameter container
│   │   │   ├── Options/
│   │   │   │   └── MEFOptions.cs                             # Module discovery configuration
│   │   │   └── Services/
│   │   │       └── MEFService.cs                             # Module manager default implementation
│   │   │
│   │   └── FilexUI.CommunityToolkit.Mvvm.Extension.WPF/      # WPF platform implementation layer
│   │       ├── Adapters/                                     # Region adapters
│   │       │   ├── NavigationAdapterBase.cs
│   │       │   ├── ContentControlAdapter.cs
│   │       │   ├── ItemsControlAdapter.cs
│   │       │   └── TabControlAdapter.cs
│   │       ├── Extensions/
│   │       │   ├── DialogExtension.cs                        # UseDialog / RegisterForDialog
│   │       │   └── NavigationExtension.cs                    # UseNavigation / RegisterForNavigation
│   │       ├── Services/
│   │       │   ├── DialogService.cs / DialogWindow.cs        # Dialog service and default window
│   │       │   ├── NavigationJournal.cs                      # Navigation journal implementation
│   │       │   └── NavigationService.cs                      # Navigation service implementation
│   │       └── Application.cs                                # Application startup base class
│   │
│   └── Demo/
│       └── CommunityToolkit.Mvvm.Extension.WPFShell/         # Sample project (Shell startup paradigm)
```

---

## Quick Start

### 1. Reference

Reference the following assemblies in your WPF project (or via the NuGet package):

```
FilexUI.CommunityToolkit.Mvvm.Extension
FilexUI.CommunityToolkit.Mvvm.Extension.WPF
```

### 2. Inherit the Application Base Class

```csharp
using CommunityToolkit.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MyApp
{
    public partial class App : Application
    {
        // 1) Create the Shell window
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        // 2) Register services and views
        protected override void RegisterServices(IServiceCollection services)
        {
            services.RegisterForNavigation<MainWindow>();
            // ... other registrations
        }

        // 3) (Optional) Configure module scanning
        protected override void CreateModule(MEFOptions options)
        {
            options.AddModule<CoreModule>();
        }
    }
}
```

### 3. Enable Everything in One Line

The `Application` base class already performs the following automatically during startup:

```csharp
services.UseNavigation().UseDialog().UseMEF(CreateModule);
```

That is: **register the navigation service + register the dialog service + enable the module system**, then build the container, create the Shell, and initialize auto-load modules.

### 4. Run

After the Shell window is shown, you can inject and use `INavigationService`, `IDialogService`, and `IMEFService` through the constructor in any ViewModel.

---

## Feature Deep Dive 1: Navigation

### 4.1 Define a Region

A region is a "slot" that hosts views, declared on XAML via the attached property `RegionName`:

```xml
<Window x:Class="MyApp.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:mvvm="clr-namespace:CommunityToolkit.Mvvm;assembly=FilexUI.CommunityToolkit.Mvvm.Extension.WPF">
    <Grid>
        <!-- Content area: shows one view at a time -->
        <ContentControl mvvm:NavigationService.RegionName="ContentRegion" />
    </Grid>
</Window>
```

Three built-in region adapters **automatically match their corresponding control types**:

| Control Type | Adapter | Region Model | Scenario |
| --- | --- | --- | --- |
| `ContentControl` | `ContentControlAdapter` | `SingleActiveRegion` | Main content area, single-view switching |
| `TabControl` | `TabControlAdapter` | `SingleActiveRegion` | Multi-tab navigation (auto-syncs selected item) |
| `ItemsControl` | `ItemsControlAdapter` | `SingleActiveRegion` | List-style view container |

### 4.2 Register Views

```csharp
// Use the view type name as the navigation name (recommended convention)
services.RegisterForNavigation<HomeView>();

// Custom navigation name
services.RegisterForNavigation<HomeView>("Home");

// Explicitly specify the view model
services.RegisterForNavigation<HomeView, HomeViewModel>("Home");

// Custom view-model resolution rule
services.RegisterForNavigation<HomeView>(
    "Home",
    viewType => typeof(HomeViewModel));
```

**Naming convention automatically wires up the ViewModel**: when a view resides in the `*.Views.*` namespace, the framework automatically looks for the same-named ViewModel under the corresponding `*.ViewModels.*` namespace (`XxxView` → `XxxViewModel`) and binds it to `DataContext` — **no manual binding code required**.

### 4.3 Perform Navigation

```csharp
public class HomeViewModel
{
    private readonly INavigationService navigationService;

    public HomeViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }

    // Simple navigation
    public void GoDetail() => navigationService.Navigate("ContentRegion", "Detail");

    // Navigation with parameters
    public void GoDetailWithArgs()
    {
        var parameter = new Parameter();
        parameter.Add("Id", 1001);
        parameter.Add("Title", "Order Detail");
        navigationService.Navigate("ContentRegion", "Detail", parameter);
    }

    // Async navigation (aware of whether it was canceled)
    public async Task GoDetailAsync()
    {
        var parameter = new Parameter();
        parameter.Add("Id", 1001);
        bool succeeded = await navigationService.NavigateAsync("ContentRegion", "Detail", parameter);
    }
}
```

### 4.4 Navigation Journal (Forward / Back)

```csharp
navigationService.CanGoBack("ContentRegion");     // Whether back is available
navigationService.CanGoForward("ContentRegion");  // Whether forward is available
navigationService.GoBack("ContentRegion");        // Go back
navigationService.GoForward("ContentRegion");     // Go forward

var journal = navigationService.GetJournal("ContentRegion");
var current = journal.CurrentEntry;               // Current journal entry
journal.Clear();                                  // Clear history
```

The journal uses a dual-stack design: **a normal navigation clears the forward stack**, and history replay reuses existing view instances (no duplicate creation).

### 4.5 Lifecycle Callbacks

A view or its ViewModel can participate in the navigation lifecycle by implementing the following interfaces (**the `DataContext`, i.e. the ViewModel, is preferred**):

```csharp
public class DetailViewModel : INavigationAware, INavigationLifecycle, INavigationLifetime
{
    // ---- INavigationAware: called on every navigation ----
    public bool IsNavigationTarget(NavigationContext context)
    {
        // Return true to reuse the current instance (e.g. refresh data only for the same order)
        return context.Parameter.GetValue<int>("Id") == currentId;
    }

    public void OnNavigatedTo(NavigationContext context)
    {
        var id = context.Parameter.GetValue<int>("Id");
        // Load data...
    }

    public void OnNavigatedFrom(NavigationContext context)
    {
        // Save state, unsubscribe...
    }

    // ---- INavigationLifecycle: called once on create / destroy only ----
    public void OnInitialized(NavigationContext context)
    {
        // One-time initialization after first creation
    }

    public void OnDestroyed()
    {
        // Clean up resources when removed from the region and no longer reused
    }

    // ---- INavigationLifetime: control whether the page stays resident ----
    public bool KeepAlive => true;   // false means removed from the region after leaving
}
```

| Interface | When Called | Typical Use |
| --- | --- | --- |
| `INavigationAware.OnNavigatedTo` | After becoming the active view each time | Load / refresh data |
| `INavigationAware.OnNavigatedFrom` | Before leaving each time | Save state, unsubscribe |
| `INavigationLifecycle.OnInitialized` | After the instance is first created (once only) | One-time initialization |
| `INavigationLifecycle.OnDestroyed` | When the instance is removed from the region (once only) | Release resources |
| `INavigationLifetime.KeepAlive` | Evaluated when leaving the region | Control view cache reuse |

### 4.6 Leave Confirmation (Intercept Navigation)

Implement `IConfirmNavigationRequest` to intercept navigation before leaving (e.g. "You have unsaved changes, are you sure you want to leave?"):

```csharp
public class EditorViewModel : IConfirmNavigationRequest
{
    public bool HasUnsavedChanges { get; set; }

    public void ConfirmNavigationRequest(NavigationContext context, Action<bool> continuationCallback)
    {
        if (!HasUnsavedChanges)
        {
            continuationCallback(true);   // Allow to continue
            return;
        }

        var result = MessageBox.Show("You have unsaved changes. Leave anyway?",
                                     "Hint",
                                     MessageBoxButton.YesNo, MessageBoxImage.Question);

        continuationCallback(result == MessageBoxResult.Yes);   // Decided by the user
    }
}
```

The callback is designed to be asynchronous — **it can be any async flow where a dialog is shown first and continuation happens after the user confirms**. When not confirmed, navigation is safely canceled without polluting the navigation journal.

### 4.7 Releasing a Region

```csharp
navigationService.RemoveRegion("ContentRegion");
```

This unregisters the region, unbinds all event subscriptions, and clears view and journal references. It is **triggered automatically when the window closes**, so no manual management is needed.

---

## Feature Deep Dive 2: Dialog

### 5.1 Enable the Dialog Service

```csharp
services.UseDialog();
```

### 5.2 Register Dialog Views

```csharp
services.RegisterForDialog<EditDialogView>();             // Use the type name
services.RegisterForDialog<EditDialogView>("EditDialog");  // Custom name
```

### 5.3 Dialog View Model

Implement `IDialogAware` to get the complete dialog lifecycle:

```csharp
public class EditDialogViewModel : IDialogAware
{
    public string Title => "Edit Information";

    public event Action<IDialogResult> RequestClose;

    public bool CanCloseDialog() => !HasUnsavedChanges;   // Protect unsaved content

    public void OnDialogOpened(Parameter parameter)
    {
        var id = parameter.GetValue<int>("Id");
        // Initialize editing data...
    }

    public void OnDialogClosed()
    {
        // Clean up subscriptions, release resources...
    }

    // Called by the "OK" button on the UI
    public void Confirm()
    {
        var result = new Parameter();
        result.Add("Saved", true);
        RequestClose?.Invoke(new DialogResult(ButtonResult.OK, result));
    }

    // Called by the "Cancel" button on the UI
    public void Cancel() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
}
```

### 5.4 Open a Dialog

```csharp
public class MainViewModel
{
    private readonly IDialogService dialogService;

    public MainViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;
    }

    // Modal: blocks and returns the result directly
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

    // Non-modal: get the result via callback
    public void ShowNonModal()
    {
        dialogService.Show("EditDialog", null, result =>
        {
            if (result.Result == ButtonResult.OK)
            {
                // Process the result...
            }
        });
    }
}
```

| Method | Behavior | Returns |
| --- | --- | --- |
| `Show(name)` | Open non-modally | Nothing |
| `Show(name, parameter)` | Open non-modally with parameters | Nothing |
| `Show(name, parameter, callback)` | Open non-modally and callback after close | Nothing |
| `ShowDialog(name)` | Open modally | `IDialogResult` |
| `ShowDialog(name, parameter)` | Open modally with parameters | `IDialogResult` |

### 5.5 Custom Dialog Window

By default the built-in `DialogWindow` is used (auto-centered on the main window, auto-sized to content). To customize the appearance:

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

You can also assign a window style to a single dialog view via an attached property:

```xml
<UserControl ... mvvm:DialogService.Style="{StaticResource DialogStyle}" />
```

### 5.6 Dialog Result Semantics

```csharp
public enum ButtonResult
{
    None,    // No explicit result provided
    OK,      // Confirmed
    Cancel,  // Canceled
    Yes,     // "Yes"
    No       // "No"
}
```

A modal dialog maps `OK` / `Yes` to a window `DialogResult = true` and `Cancel` / `No` to `false`, **consistent with the native WPF dialog semantics**.

---

## Feature Deep Dive 3: Module / MEF

### 6.1 Define a Module

```csharp
[Module(Name = "Core", Description = "Core business module", AutoLoad = true)]
public class CoreModule : IModule
{
    // Called before the root service provider is built: register any dependency
    public void RegisterServices(IServiceCollection services)
    {
        services.RegisterSingleton<IOrderService, OrderService>();
        services.RegisterForNavigation<OrderView, OrderViewModel>("Order");
    }

    // Called after the container is built: good for loading data and initiating first navigation
    public void OnInitialized(IServiceProvider provider)
    {
        var navigation = provider.Resolve<INavigationService>();
        navigation.Navigate("ContentRegion", "Order");
    }
}
```

| `ModuleAttribute` member | Description | Default |
| --- | --- | --- |
| `Name` | Module display name | Type name |
| `Description` | Module description | Empty |
| `AutoLoad` | Whether to auto-initialize on startup | `true` |

**Key trait**: regardless of whether auto-load is enabled, **`RegisterServices` of every discovered module runs before the container is built**, so cross-module dependencies can register freely against each other.

### 6.2 Configure Module Discovery

```csharp
services.UseMEF(options =>
{
    // Option 1: scan specified assemblies
    options.AddModule<CoreModule>();
    options.AddModule(typeof(PluginModule).Assembly);

    // Option 2: scan specified directories (plugin hot-swap scenarios)
    options.AddDirectory(@"C:\MyApp\Modules");
    options.AddDirectory("Modules");          // Relative to the application base directory

    // Custom file match pattern
    options.SearchPattern = "*.dll";

    // Discovery-complete callback: useful for logging and diagnostics
    options.OnModulesDiscovered(modules =>
    {
        foreach (var module in modules)
        {
            Console.WriteLine($"Discovered module: {module.Name}");
        }
    });
});
```

**Zero-config default behavior**: when no scanning source is specified, it scans the **application base directory** and its **`Modules` subdirectory** by default — ready to use out of the box.

### 6.3 Manage Modules

```csharp
var mef = Container.Resolve<IMEFService>();

mef.DiscoveredModules;   // All discovered modules
mef.AutoLoadModules;     // Modules that will be auto-initialized
mef.Modules;             // Modules that have finished initializing

mef.ModuleLoaded += info => Console.WriteLine($"Module loaded: {info.Name}");

// On-demand activation (modules with AutoLoad = false)
mef.LoadModule("Plugin");
mef.LoadModule<PluginModule>();
mef.LoadModule(typeof(PluginModule));

// Register a module directly from code (skips scanning)
services.AddModule<CoreModule>();
```

This module system is a **lightweight module manager** (not the official .NET MEF implementation), focused on module discovery, service registration, and startup orchestration — **clear logic, predictable behavior**.

---

## Dependency Injection Enhancements

`ServiceCollectionExtension` provides Prism-like DI convenience methods, making registration and resolution code more concise and consistent.

### Register Services

```csharp
// Transient registration
services.Register<IMyService, MyService>();
services.Register<MyService>();

// Singleton registration
services.RegisterSingleton<IMyService, MyService>();
services.RegisterSingleton<MyService>();

// Register with an existing instance
services.RegisterInstance<IMyService>(myService);

// Named registration (multiple implementations of the same type)
services.Register<IMyService, MyService>("A");
services.RegisterSingleton<IMyService, MyService>("B");
services.RegisterInstance<IMyService>(myService, "C");
```

### Resolve Services

```csharp
var service = provider.Resolve<IMyService>();           // Resolve (throws a clear exception if not registered)
var named   = provider.Resolve<IMyService>("A");        // Resolve by name
var ok      = provider.IsRegistered<IMyService>();      // Whether already registered
var okNamed = provider.IsRegistered<IMyService>("A");   // Whether the named one is registered
```

### Check Registration Status

```csharp
services.IsRegistered<IMyService>();
services.IsRegistered<IMyService>("A");
services.IsRegistered(typeof(IMyService), "A");
```

All methods support **fluent chaining** (returning the same `IServiceCollection`), and also include **complete parameter validation** (null checks, type assignability, non-empty names, etc.), so misconfiguration is surfaced clearly at startup.

---

## Application Base Class

The `CommunityToolkit.Mvvm.Application` abstract class orchestrates the standard WPF startup flow:

```
OnStartup
   └─ InitIoc()
        ├─ RegisterServices(services)                    ← Subclass registers services
        ├─ services.UseNavigation().UseDialog().UseMEF(CreateModule)
        └─ BuildServiceProvider()                        ← Build the container
   ├─ Initialize()
   │     ├─ CreateShell()                                ← Subclass creates the Shell (abstract)
   │     └─ InitializeShell(shell)                       ← Defaults to MainWindow (overridable)
   ├─ Container.UseMEF()                                 ← Initialize auto-load modules
   └─ OnInitialized()                                    ← Defaults to showing the main window (overridable)
```

Overridable extension points:

| Member | Description | Required |
| --- | --- | --- |
| `RegisterServices(IServiceCollection)` | Register application services and views | **Yes** (abstract) |
| `CreateShell()` | Create the main window | **Yes** (abstract) |
| `InitializeShell(Window)` | Decide how the main window is presented | No |
| `CreateModule(MEFOptions)` | Configure module scanning | No |
| `OnInitialized()` | Finalization logic after startup completes | No |
| `Container` property | Global root service provider | — |

**The startup order is carefully orchestrated**: navigation regions are registered during Shell construction, and module initialization runs after the container is ready, so **modules can safely initiate the first navigation inside `OnInitialized`**.

---

## Complete Example

### Scenario: Navigate to the order page inside the main window and pop up an edit dialog

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

**2) MainWindow.xaml (define regions)**

```xml
<Window x:Class="MyApp.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:mvvm="clr-namespace:CommunityToolkit.Mvvm;assembly=FilexUI.CommunityToolkit.Mvvm.Extension.WPF"
        Title="MyApp" Width="900" Height="600">
    <DockPanel>
        <StackPanel DockPanel.Dock="Top" Orientation="Horizontal">
            <Button Content="Order" Command="{Binding GoOrderCommand}" />
            <Button Content="Back" Command="{Binding GoBackCommand}" />
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

**4) OrderViewModel.cs (with dialog interaction)**

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
        // Load data when the page is displayed
    }

    public void OnNavigatedFrom(NavigationContext context)
    {
        // Save state when the page leaves
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
            // Process the save result
        }
    }
}
```

---

## API Quick Reference

### Service Collection Extensions

| Method | Belongs To | Purpose |
| --- | --- | --- |
| `UseNavigation()` | `NavigationExtension` | Register the navigation service and built-in adapters |
| `UseDialog()` | `DialogExtension` | Register the dialog service and default window |
| `UseMEF(Action<MEFOptions>)` | `MEFExtension` | Enable module discovery and registration |
| `UseMEF()` | `MEFExtension` | Enable module discovery (default directory scan) |
| `AddModule<T>()` | `MEFExtension` | Register a module directly |
| `RegisterForNavigation<TView>()` | `NavigationExtension` | Register a navigation view (convention-based ViewModel lookup) |
| `RegisterForNavigation<TView, TViewModel>(name)` | `NavigationExtension` | Register a navigation view with an explicit ViewModel |
| `RegisterNavigationAdapter<T>(adapter)` | `NavigationExtension` | Register / replace a region adapter |
| `RegisterForDialog<TView>(name)` | `DialogExtension` | Register a dialog view |
| `RegisterDialogWindow<TWindow>()` | `DialogExtension` | Replace the default dialog window |
| `Register` / `RegisterSingleton` / `RegisterInstance` | `ServiceCollectionExtension` | Prism-style service registration |
| `Resolve` / `IsRegistered` | `ServiceCollectionExtension` | Service resolution and registration check |

### Runtime Services

| Interface | Core Members | Purpose |
| --- | --- | --- |
| `INavigationService` | `Navigate` / `NavigateAsync` / `GoBack` / `GoForward` / `CanGoBack` / `CanGoForward` / `GetJournal` / `RemoveRegion` | Region navigation |
| `IDialogService` | `Show` / `ShowDialog` | Dialogs |
| `IMEFService` | `DiscoveredModules` / `AutoLoadModules` / `Modules` / `ModuleLoaded` / `DiscoverModules` / `LoadModule` | Module management |

### Lifecycle Interfaces

| Interface | Members | When Called |
| --- | --- | --- |
| `INavigationAware` | `IsNavigationTarget` / `OnNavigatedTo` / `OnNavigatedFrom` | Every navigation |
| `INavigationLifecycle` | `OnInitialized` / `OnDestroyed` | Once each on create / destroy |
| `INavigationLifetime` | `KeepAlive` | When leaving the region |
| `IConfirmNavigationRequest` | `ConfirmNavigationRequest` | Before navigation leaves |
| `IDialogAware` | `Title` / `RequestClose` / `CanCloseDialog` / `OnDialogOpened` / `OnDialogClosed` | Full dialog lifecycle |

---

## FAQ

**Q1: Is the built-in `Application` base class required?**

No. The base class simply packages the standard startup flow; you can call `UseNavigation()`, `UseDialog()`, and `UseMEF()` manually in your own `App` and build the container as needed.

**Q2: Which region host controls are supported?**

Three built-in adapters: `ContentControl`, `TabControl`, and `ItemsControl`. For other controls, implement `INavigationAdapter<T>` and register it via `RegisterNavigationAdapter` — **a simple and uniform extension mechanism**.

**Q3: Will views be created repeatedly?**

No. The navigation service prefers reusing existing instances in the region and uses `IsNavigationTarget` to decide the reuse condition; history forward/back also reuses instances.

**Q4: How do I make a page no longer retained after leaving?**

Implement `INavigationLifetime` in the ViewModel and set `KeepAlive => false`.

**Q5: When does a module's `RegisterServices` execute?**

**Before** the root service provider is built, so services registered by a module can be used jointly by other modules and the host application.

**Q6: How do I set the dialog title?**

The ViewModel implements `IDialogAware` and returns it via the `Title` property; when not specified, the registration name is used.

**Q7: Should the navigation interface be implemented by the ViewModel or the View?**

The framework **checks the `DataContext` (i.e. the ViewModel) first, then the view itself**. Implementing it in the ViewModel is recommended as it better fits MVVM principles.

---

## Compatibility

This extension package provides broad multi-target framework support:

- **.NET Framework**: `net4.5` / `net4.5.2` / `net4.6` / `net4.6.2` / `net4.7` / `net4.7.2` / `net4.8`
- **.NET Core / .NET**: `netcoreapp3.0` / `netcoreapp3.1` / `net5.0` / `net6.0` / `net7.0` / `net8.0` (WPF parts target the corresponding `-windows` TFM)

**Dependencies**:

| Dependency | Version |
| --- | --- |
| `CommunityToolkit.Mvvm` | 8.4.2 |
| `Microsoft.Extensions.DependencyInjection` | Auto-adapted per target framework |

> Note: Compatibility across different target frameworks may be affected by the specific system environment. It is recommended to verify with a real test in the target environment before cross-version use.

---

## License & Feedback

- Author: wenke
- Repository: https://github.com/Coolkeke/CommunityToolkit.Mvvm.Extension

Issues and Pull Requests are welcome to help improve this extension ecosystem for CommunityToolkit.Mvvm.

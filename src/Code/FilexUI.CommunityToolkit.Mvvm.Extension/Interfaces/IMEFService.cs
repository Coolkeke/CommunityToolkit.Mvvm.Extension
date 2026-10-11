using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 提供对已发现模块的访问能力，并控制手动模块激活。
    /// </summary>
    /// <remarks>
    /// 该服务是轻量级模块管理器，不是官方 .NET MEF 实现。
    /// 所有已发现模块都会在宿主服务提供程序构建前注册服务。
    /// 只有自动加载模块会自动初始化；其他模块会在调用
    /// <see cref="LoadModule(Type)"/>、<see cref="LoadModule{T}"/> 或
    /// <see cref="LoadModule(string)"/> 时初始化。
    /// </remarks>
    public interface IMEFService
    {
        /// <summary>
        /// 在模块完成 <see cref="IModule.OnInitialized(IServiceProvider)"/> 后发生。
        /// </summary>
        event Action<IModuleInfo> ModuleLoaded;

        /// <summary>
        /// 获取从已配置模块程序集和目录中发现的所有模块。
        /// </summary>
        IReadOnlyList<IModuleInfo> DiscoveredModules { get; }

        /// <summary>
        /// 获取元数据允许自动初始化的已发现模块。
        /// </summary>
        IReadOnlyList<IModuleInfo> AutoLoadModules { get; }

        /// <summary>
        /// 获取已经完成初始化并处于活动状态的模块。
        /// </summary>
        IReadOnlyList<IModuleInfo> Modules { get; }

        /// <summary>
        /// 从指定程序集中发现模块类型。
        /// </summary>
        /// <param name="assemblies">要扫描具体 <see cref="IModule"/> 实现的程序集。</param>
        void DiscoverModules(params Assembly[] assemblies);

        /// <summary>
        /// 从指定目录中的托管程序集中发现模块类型。
        /// </summary>
        /// <param name="directory">包含模块程序集的目录。</param>
        void DiscoverModules(string directory);

        /// <summary>
        /// 按具体类型初始化此前发现的模块。
        /// </summary>
        /// <param name="moduleType">要初始化的具体模块类型。</param>
        /// <returns>找到并完成初始化时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        bool LoadModule(Type moduleType);

        /// <summary>
        /// 按泛型类型初始化此前发现的模块。
        /// </summary>
        /// <typeparam name="T">要初始化的具体模块类型。</typeparam>
        /// <returns>找到并完成初始化时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        bool LoadModule<T>() where T : class, IModule;

        /// <summary>
        /// 按模块名称初始化此前发现的模块。
        /// </summary>
        /// <param name="moduleName">
        /// <see cref="ModuleAttribute.Name"/> 声明的名称；未指定名称时使用类型名称。
        /// </param>
        /// <returns>找到并完成初始化时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        bool LoadModule(string moduleName);
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// <see cref="IDialogService"/> 的 WPF 实现。
    /// </summary>
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Dictionary<string, Type> dialogTypes;

        /// <summary>
        /// 初始化新的对话框服务。
        /// </summary>
        /// <param name="serviceProvider">用于解析对话框视图和窗口的服务提供程序。</param>
        /// <param name="registrations">已注册的对话框视图映射。</param>
        public DialogService(IServiceProvider serviceProvider, IEnumerable<DialogViewRegistration> registrations)
        {
            this.serviceProvider = serviceProvider;
            dialogTypes = registrations.ToDictionary(x => x.DialogName, x => x.ViewType, StringComparer.Ordinal);
        }

        /// <summary>
        /// 按名称显示非模态对话框。
        /// </summary>
        /// <param name="dialogName">已注册的对话框名称。</param>
        public void Show(string dialogName)
        {
            Show(dialogName, null, null);
        }

        /// <summary>
        /// 按名称显示非模态对话框，并向对话框生命周期目标传递参数。
        /// </summary>
        /// <param name="dialogName">已注册的对话框名称。</param>
        /// <param name="parameter">传递给对话框的参数。</param>
        public void Show(string dialogName, Parameter parameter)
        {
            Show(dialogName, parameter, null);
        }

        /// <summary>
        /// 显示非模态对话框，并在其关闭时调用回调。
        /// </summary>
        /// <param name="dialogName">已注册的对话框名称。</param>
        /// <param name="parameter">传递给对话框的参数。</param>
        /// <param name="callback">接收关闭结果的回调。</param>
        public void Show(string dialogName, Parameter parameter, Action<IDialogResult> callback)
        {
            var window = CreateDialogWindow(dialogName, parameter, callback, false);
            window.Show();
        }

        /// <summary>
        /// 按名称显示模态对话框。
        /// </summary>
        /// <param name="dialogName">已注册的对话框名称。</param>
        /// <returns>对话框关闭结果。</returns>
        public IDialogResult ShowDialog(string dialogName)
        {
            return ShowDialog(dialogName, null);
        }

        /// <summary>
        /// 按名称显示模态对话框，并向对话框生命周期目标传递参数。
        /// </summary>
        /// <param name="dialogName">已注册的对话框名称。</param>
        /// <param name="parameter">传递给对话框的参数。</param>
        /// <returns>对话框关闭结果。</returns>
        public IDialogResult ShowDialog(string dialogName, Parameter parameter)
        {
            IDialogResult result = new DialogResult();
            var window = CreateDialogWindow(dialogName, parameter, x => result = x, true);
            window.ShowDialog();
            return result;
        }

        /// <summary>
        /// 获取应用到目标视图承载对话框窗口的附加样式。
        /// </summary>
        /// <param name="obj">拥有该附加属性的对象。</param>
        /// <returns>对话框窗口样式。</returns>
        public static Style GetStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(StyleProperty);
        }

        /// <summary>
        /// 设置应用到目标视图承载对话框窗口的附加样式。
        /// </summary>
        /// <param name="obj">拥有该附加属性的对象。</param>
        /// <param name="value">要应用到对话框窗口的样式。</param>
        public static void SetStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(StyleProperty, value);
        }

        /// <summary>
        /// 标识用于设置对话框视图宿主窗口样式的附加属性。
        /// </summary>
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.RegisterAttached("Style", typeof(Style), typeof(DialogService), new PropertyMetadata(null));

        private IDialogWindow CreateDialogWindow(string dialogName, Parameter parameter, Action<IDialogResult> callback, bool isModal)
        {
            if (string.IsNullOrEmpty(dialogName))
            {
                throw new ArgumentNullException(nameof(dialogName));
            }

            if (!dialogTypes.TryGetValue(dialogName, out var viewType))
            {
                throw new InvalidOperationException($"Dialog '{dialogName}' has not been registered.");
            }

            var content = CreateDialogContent(viewType);
            var aware = GetDialogAware(content);
            var window = CreateWindow(content, aware, dialogName);
            ApplyWindowProperties(window, content);
            var ownsContent = !(content is IDialogWindow);
            IDialogResult closeResult = new DialogResult();
            var closeRequested = false;
            Action<IDialogResult> requestCloseHandler = null;
            CancelEventHandler closingHandler = null;
            EventHandler closedHandler = null;

            if (aware != null)
            {
                // 对话框感知内容通过触发 RequestClose 关闭；服务会将其转换为窗口状态。
                requestCloseHandler = result =>
                {
                    if (!aware.CanCloseDialog())
                    {
                        return;
                    }

                    closeRequested = true;
                    closeResult = result ?? new DialogResult();
                    if (isModal && TrySetWindowDialogResult(window, closeResult))
                    {
                        return;
                    }

                    window.Close();
                };

                aware.RequestClose += requestCloseHandler;
            }

            closingHandler = (sender, args) =>
            {
                // 直接关闭窗口时仍会遵守 CanCloseDialog，让视图模型可以保护未保存工作。
                if (aware != null && !closeRequested && !aware.CanCloseDialog())
                {
                    args.Cancel = true;
                }
            };

            closedHandler = (sender, args) =>
            {
                // 主动解绑事件处理程序，因为应用程序会话中可能会多次创建对话框。
                if (aware != null && requestCloseHandler != null)
                {
                    aware.RequestClose -= requestCloseHandler;
                }

                window.Closing -= closingHandler;
                window.Closed -= closedHandler;

                if (aware != null)
                {
                    aware.OnDialogClosed();
                }

                if (ownsContent)
                {
                    window.Content = null;
                }

                callback?.Invoke(closeResult);
            };

            window.Closing += closingHandler;
            window.Closed += closedHandler;

            aware?.OnDialogOpened(parameter ?? new Parameter());
            return window;
        }

        private object CreateDialogContent(Type viewType)
        {
            var view = serviceProvider.GetService(viewType) ?? Activator.CreateInstance(viewType);
            if (view == null)
            {
                throw new InvalidOperationException($"Dialog view '{viewType.FullName}' could not be created.");
            }

            return view;
        }

        private IDialogWindow CreateWindow(object content, IDialogAware aware, string dialogName)
        {
            var contentWindow = content as IDialogWindow;
            if (contentWindow != null)
            {
                SetOwner(contentWindow);
                if (aware != null && !string.IsNullOrEmpty(aware.Title))
                {
                    contentWindow.Title = aware.Title;
                }

                return contentWindow;
            }

            var window = serviceProvider.GetService<IDialogWindow>() ?? new DialogWindow();
            window.Content = content;
            window.Title = aware != null && !string.IsNullOrEmpty(aware.Title) ? aware.Title : dialogName;
            SetOwner(window);
            return window;
        }

        private static IDialogAware GetDialogAware(object content)
        {
            var frameworkElement = content as FrameworkElement;
            if (frameworkElement != null)
            {
                // 优先使用数据上下文，让 MVVM 视图模型拥有对话框生命周期。
                return frameworkElement.DataContext as IDialogAware ?? frameworkElement as IDialogAware;
            }

            return content as IDialogAware;
        }

        private static void ApplyWindowProperties(IDialogWindow dialogWindow, object content)
        {
            var window = dialogWindow as Window;
            var source = content as DependencyObject;
            if (window == null || source == null)
            {
                return;
            }

            var style = GetStyle(source);
            if (style != null)
            {
                window.Style = style;
            }
        }

        private static void SetOwner(IDialogWindow window)
        {
            if (System.Windows.Application.Current != null && System.Windows.Application.Current.MainWindow != null && System.Windows.Application.Current.MainWindow != window)
            {
                window.Owner = System.Windows.Application.Current.MainWindow;
            }
        }

        private static bool TrySetWindowDialogResult(IDialogWindow window, IDialogResult result)
        {
            if (window.IsActive)
            {
                var buttonResult = result == null ? ButtonResult.None : result.Result;
                if (buttonResult == ButtonResult.OK || buttonResult == ButtonResult.Yes)
                {
                    window.DialogResult = true;
                    return true;
                }
                else if (buttonResult == ButtonResult.Cancel || buttonResult == ButtonResult.No)
                {
                    window.DialogResult = false;
                    return true;
                }
            }

            return false;
        }
    }
}

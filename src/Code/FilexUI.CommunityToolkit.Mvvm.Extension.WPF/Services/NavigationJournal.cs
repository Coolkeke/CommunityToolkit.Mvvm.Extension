using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 单个具名区域的默认导航日志。
    /// </summary>
    public class NavigationJournal : INavigationJournal
    {
        private readonly string regionName;
        private readonly Action<INavigationJournalEntry, Action<bool>> navigationCallback;
        private readonly Action<IList<INavigationJournalEntry>> entriesRemovedCallback;
        private readonly Stack<INavigationJournalEntry> backStack = new Stack<INavigationJournalEntry>();
        private readonly Stack<INavigationJournalEntry> forwardStack = new Stack<INavigationJournalEntry>();
        private bool isHistoryNavigationPending;

        /// <summary>
        /// 初始化新的导航日志。
        /// </summary>
        /// <param name="regionName">该日志跟踪的区域名称。</param>
        /// <param name="navigationCallback">用于重放导航日志项的回调。</param>
        public NavigationJournal(string regionName, Action<INavigationJournalEntry> navigationCallback)
            : this(regionName, (entry, completed) =>
            {
                navigationCallback(entry);
                completed(true);
            }, null)
        {
            if (navigationCallback == null)
            {
                throw new ArgumentNullException(nameof(navigationCallback));
            }
        }

        /// <summary>
        /// 初始化支持异步确认和历史项释放的新导航日志。
        /// </summary>
        public NavigationJournal(
            string regionName,
            Action<INavigationJournalEntry, Action<bool>> navigationCallback,
            Action<IList<INavigationJournalEntry>> entriesRemovedCallback)
        {
            if (string.IsNullOrEmpty(regionName))
            {
                throw new ArgumentNullException(nameof(regionName));
            }

            if (navigationCallback == null)
            {
                throw new ArgumentNullException(nameof(navigationCallback));
            }

            this.regionName = regionName;
            this.navigationCallback = navigationCallback;
            this.entriesRemovedCallback = entriesRemovedCallback;
        }

        /// <summary>
        /// 获取一个值，指示是否存在上一条日志项。
        /// </summary>
        public bool CanGoBack => backStack.Count > 0;

        /// <summary>
        /// 获取一个值，指示是否存在下一条日志项。
        /// </summary>
        public bool CanGoForward => forwardStack.Count > 0;

        /// <summary>
        /// 获取当前导航日志项。
        /// </summary>
        public INavigationJournalEntry CurrentEntry { get; private set; }

        /// <summary>
        /// 记录一次普通导航，并清空前进堆栈。
        /// </summary>
        /// <param name="entry">要记录为当前项的日志项。</param>
        public void RecordNavigation(INavigationJournalEntry entry)
        {
            RecordNavigation(entry, CurrentEntry);
        }

        /// <summary>
        /// 使用导航前的当前项提交一次普通导航。
        /// </summary>
        /// <param name="entry">要记录为当前项的日志项。</param>
        /// <param name="previousEntry">开始导航前的当前项。</param>
        public void RecordNavigation(INavigationJournalEntry entry, INavigationJournalEntry previousEntry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            if (previousEntry != null)
            {
                backStack.Push(previousEntry);
            }

            CurrentEntry = entry;
            RemoveEntries(forwardStack.ToArray());
            forwardStack.Clear();
        }

        /// <summary>
        /// 替换当前日志项，但不改变后退或前进堆栈。
        /// </summary>
        /// <param name="entry">应成为当前项的日志项。</param>
        public void UpdateCurrentEntry(INavigationJournalEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            CurrentEntry = entry;
        }

        /// <summary>
        /// 在可后退时重放上一条日志项。
        /// </summary>
        public void GoBack()
        {
            if (!CanGoBack)
            {
                return;
            }

            NavigateHistory(backStack, forwardStack);
        }

        /// <summary>
        /// 在可前进时重放下一条日志项。
        /// </summary>
        public void GoForward()
        {
            if (!CanGoForward)
            {
                return;
            }

            NavigateHistory(forwardStack, backStack);
        }

        /// <summary>
        /// 清空后退和前进历史堆栈。
        /// </summary>
        public void Clear()
        {
            var removedEntries = backStack.Concat(forwardStack).ToList();
            backStack.Clear();
            forwardStack.Clear();
            RemoveEntries(removedEntries);
        }

        private void NavigateHistory(Stack<INavigationJournalEntry> source, Stack<INavigationJournalEntry> destination)
        {
            if (isHistoryNavigationPending || source.Count == 0)
            {
                return;
            }

            var target = source.Peek();
            var previousEntry = CurrentEntry;
            isHistoryNavigationPending = true;
            navigationCallback(target, succeeded =>
            {
                isHistoryNavigationPending = false;
                if (!succeeded || source.Count == 0 || !ReferenceEquals(source.Peek(), target))
                {
                    return;
                }

                source.Pop();
                if (previousEntry != null)
                {
                    destination.Push(previousEntry);
                }

                CurrentEntry = target;
            });
        }

        private void RemoveEntries(IEnumerable<INavigationJournalEntry> entries)
        {
            var removedEntries = entries.ToList();
            if (removedEntries.Count > 0 && entriesRemovedCallback != null)
            {
                entriesRemovedCallback(removedEntries);
            }
        }
    }
}

using System;
namespace Expandable
{
    public class ExpandChangedEventArgs : EventArgs
    {
        public ExpandChangedEventArgs(bool isExpand)
        {
            IsExpanded = isExpand;
        }

        public bool IsExpanded { get; }
    }
}

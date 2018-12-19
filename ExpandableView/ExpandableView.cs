using System;
using Xamarin.Forms;

namespace Expandable
{
    public class ExpandableView : StackLayout
    {
        public const string ExpandAnimationName = nameof(ExpandAnimationName);

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public static readonly BindableProperty PrimaryViewProperty = BindableProperty.Create(nameof(PrimaryView), typeof(View), typeof(ExpandableView), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as ExpandableView).SetPrimaryView(oldValue as View);
            (bindable as ExpandableView).OnTouchHandlerViewChanged();
        });

        public static readonly BindableProperty SecondaryViewTemplateProperty = BindableProperty.Create(nameof(SecondaryViewTemplate), typeof(DataTemplate), typeof(ExpandableView), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as ExpandableView).SetSecondaryView(true);
            (bindable as ExpandableView).OnIsExpandedChanged();
        });

        public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(ExpandableView), default(bool), BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as ExpandableView).SetSecondaryView();
            (bindable as ExpandableView).OnIsExpandedChanged();
        });

        public static readonly BindableProperty TouchHandlerViewProperty = BindableProperty.Create(nameof(TouchHandlerView), typeof(View), typeof(ExpandableView), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as ExpandableView).OnTouchHandlerViewChanged();
        });

        public static readonly BindableProperty IsTouchToExpandEnabledProperty = BindableProperty.Create(nameof(IsTouchToExpandEnabled), typeof(bool), typeof(ExpandableView), true);

        public static readonly BindableProperty SecondaryViewHeightRequestProperty = BindableProperty.Create(nameof(SecondaryViewHeightRequest), typeof(double), typeof(ExpandableView), -1.0);

        public static readonly BindableProperty ExpandAnimationLengthProperty = BindableProperty.Create(nameof(ExpandAnimationLength), typeof(uint), typeof(ExpandableView), 250u);

        public static readonly BindableProperty ExpandAnimationEasingProperty = BindableProperty.Create(nameof(ExpandAnimationEasing), typeof(Easing), typeof(ExpandableView), null);

        public static readonly BindableProperty StatusProperty = BindableProperty.Create(nameof(Status), typeof(ExpandStatus), typeof(ExpandableView), default(ExpandStatus), BindingMode.OneWayToSource);

        private readonly TapGestureRecognizer _defaultTapGesture;
        private bool _shouldIgnoreAnimation;
        private double _lastVisibleHeight = -1;
        private double _startHeight;
        private double _endHeight;
        private View _secondaryView;

        public ExpandableView()
        {
            _defaultTapGesture = new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    if (!IsTouchToExpandEnabled)
                    {
                        return;
                    }
                    IsExpanded = !IsExpanded;
                })
            };
        }

        public View PrimaryView
        {
            get => GetValue(PrimaryViewProperty) as View;
            set => SetValue(PrimaryViewProperty, value);
        }

        public DataTemplate SecondaryViewTemplate
        {
            get => GetValue(SecondaryViewTemplateProperty) as DataTemplate;
            set => SetValue(SecondaryViewTemplateProperty, value);
        }

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public View TouchHandlerView
        {
            get => GetValue(TouchHandlerViewProperty) as View;
            set => SetValue(TouchHandlerViewProperty, value);
        }

        public bool IsTouchToExpandEnabled
        {
            get => (bool)GetValue(IsTouchToExpandEnabledProperty);
            set => SetValue(IsTouchToExpandEnabledProperty, value);
        }

        public double SecondaryViewHeightRequest
        {
            get => (double)GetValue(SecondaryViewHeightRequestProperty);
            set => SetValue(SecondaryViewHeightRequestProperty, value);
        }

        public uint ExpandAnimationLength
        {
            get => (uint)GetValue(ExpandAnimationLengthProperty);
            set => SetValue(ExpandAnimationLengthProperty, value);
        }

        public Easing ExpandAnimationEasing
        {
            get => (Easing)GetValue(ExpandAnimationEasingProperty);
            set => SetValue(ExpandAnimationEasingProperty, value);
        }

        public ExpandStatus Status
        {
            get => (ExpandStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public View SecondaryView
        {
            get => _secondaryView;
            private set
            {
                if (_secondaryView != null)
                {
                    _secondaryView.SizeChanged -= OnSecondaryViewSizeChanged;
                    Children.Remove(_secondaryView);
                }
                if (value != null)
                {
                    if (value is Layout layout)
                    {
                        layout.IsClippedToBounds = true;
                    }
                    value.HeightRequest = 0;
                    value.IsVisible = false;
                    Children.Add(_secondaryView = value);
                }
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            _lastVisibleHeight = -1;
        }

        private void OnIsExpandedChanged()
        {
            if (SecondaryView == null)
            {
                return;
            }

            SecondaryView.SizeChanged -= OnSecondaryViewSizeChanged;

            var isExpanding = SecondaryView.AnimationIsRunning(ExpandAnimationName);
            SecondaryView.AbortAnimation(ExpandAnimationName);

            if (IsExpanded)
            {
                SecondaryView.IsVisible = true;
            }

            _startHeight = 0;
            _endHeight = SecondaryViewHeightRequest >= 0
                ? SecondaryViewHeightRequest
                : _lastVisibleHeight;

            var shouldInvokeAnimation = true;

            if (IsExpanded)
            {
                if (_endHeight <= 0)
                {
                    shouldInvokeAnimation = false;
                    SecondaryView.HeightRequest = -1;
                    SecondaryView.SizeChanged += OnSecondaryViewSizeChanged;
                }
            }
            else
            {
                _lastVisibleHeight = _startHeight = SecondaryViewHeightRequest >= 0
                        ? SecondaryViewHeightRequest
                            : !isExpanding
                                 ? SecondaryView.Height - (SecondaryView is Layout<View> l //https://github.com/AndreiMisiukevich/ExpandableView/issues/7
                                    ? l.Padding.Top + l.Padding.Bottom
                                    : 0)
                                  : _lastVisibleHeight;
                _endHeight = 0;
            }

            _shouldIgnoreAnimation = Height < 0;

            if (shouldInvokeAnimation)
            {
                InvokeAnimation();
            }
        }

        private void OnTouchHandlerViewChanged()
        {
            var touchHandlerView = TouchHandlerView ?? PrimaryView;
            touchHandlerView?.GestureRecognizers.Remove(_defaultTapGesture);
            PrimaryView?.GestureRecognizers.Remove(_defaultTapGesture);
            touchHandlerView?.GestureRecognizers.Add(_defaultTapGesture);
        }

        private void SetPrimaryView(View oldView)
        {
            if (oldView != null)
            {
                Children.Remove(oldView);
            }
            Children.Insert(0, PrimaryView);
        }

        private void SetSecondaryView(bool forceUpdate = false)
        {
            if (IsExpanded && (SecondaryView == null || forceUpdate))
            {
                SecondaryView = CreateSecondaryView();
            }
        }

        private View CreateSecondaryView()
        {
            var template = SecondaryViewTemplate;
            if (template is DataTemplateSelector selector)
            {
                template = selector.SelectTemplate(BindingContext, this);
            }
            return template?.CreateContent() as View;
        }

        private void OnSecondaryViewSizeChanged(object sender, EventArgs e)
        {
            if (SecondaryView.Height <= 0) return;
            SecondaryView.SizeChanged -= OnSecondaryViewSizeChanged;
            SecondaryView.HeightRequest = 0;
            _endHeight = SecondaryView.Height;
            InvokeAnimation();
        }

        private void InvokeAnimation()
        {
            RaiseStatusChanged(IsExpanded ? ExpandStatus.Expanding : ExpandStatus.Collapsing);

            if (_shouldIgnoreAnimation)
            {
                RaiseStatusChanged(IsExpanded ? ExpandStatus.Expanded : ExpandStatus.Collapsed);
                SecondaryView.HeightRequest = _endHeight;
                SecondaryView.IsVisible = IsExpanded;
                return;
            }

            new Animation(v => SecondaryView.HeightRequest = v, _startHeight, _endHeight)
                .Commit(SecondaryView, ExpandAnimationName, 16, ExpandAnimationLength, ExpandAnimationEasing, (value, interrupted) =>
                {
                    if (interrupted)
                    {
                        return;
                    }
                    if (!IsExpanded)
                    {
                        SecondaryView.IsVisible = false;
                        RaiseStatusChanged(ExpandStatus.Collapsed);
                        return;
                    }
                    RaiseStatusChanged(ExpandStatus.Expanded);
                });
        }

        private void RaiseStatusChanged(ExpandStatus status)
        {
            Status = status;
            StatusChanged?.Invoke(this, new StatusChangedEventArgs(status));
        }
    }
}

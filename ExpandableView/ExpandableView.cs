using System;
using Xamarin.Forms;

namespace Expandable
{
	public class ExpandableView : StackLayout
	{
		public const string ExpandAnimationName = "expandAnimation";

		public static readonly BindableProperty PrimaryViewProperty = BindableProperty.Create(nameof(PrimaryView), typeof(View), typeof(ExpandableView), null, propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as ExpandableView).SetPrimaryView(oldValue as View);
			(bindable as ExpandableView).OnShouldHandleTapToExpandChanged();
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

		public static readonly BindableProperty ShouldHandleTapToExpandProperty = BindableProperty.Create(nameof(ShouldHandleTapToExpand), typeof(bool), typeof(ExpandableView), true, propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as ExpandableView).OnShouldHandleTapToExpandChanged();
		});

		public static readonly BindableProperty SecondaryViewHeightRequestProperty = BindableProperty.Create(nameof(SecondaryViewHeightRequest), typeof(double), typeof(ExpandableView), 0.0);

        public static readonly BindableProperty ExpandCommandViewProperty = BindableProperty.Create(nameof(ExpandCommandView), typeof(View), typeof(ExpandableView), null);

        public static readonly BindableProperty ShouldRotateProperty = BindableProperty.Create(nameof(ShouldRotate), typeof(bool), typeof(ExpandableView), false);

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
				Command = new Command(() => IsExpanded = !IsExpanded)
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

		public bool ShouldHandleTapToExpand
		{
			get => (bool)GetValue(ShouldHandleTapToExpandProperty);
			set => SetValue(ShouldHandleTapToExpandProperty, value);
		}

		public double SecondaryViewHeightRequest
		{
			get => (double)GetValue(SecondaryViewHeightRequestProperty);
			set => SetValue(SecondaryViewHeightRequestProperty, value);
		}

        public View ExpandCommandView
        {
            get => GetValue(ExpandCommandViewProperty) as View;
            set => SetValue(ExpandCommandViewProperty, value);
        }

        public bool ShouldRotate
        {
            get => (bool)GetValue(ShouldRotateProperty);
            set => SetValue(ShouldRotateProperty, value);
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
			_endHeight = SecondaryViewHeightRequest > 0
				? SecondaryViewHeightRequest
				: _lastVisibleHeight;

			var shouldInvokeAnimation = true;

			if(IsExpanded)
			{
				if(_endHeight <= 0)
				{
					shouldInvokeAnimation = false;
					SecondaryView.HeightRequest = -1;
					SecondaryView.SizeChanged += OnSecondaryViewSizeChanged;
				}
			}
			else
			{
				_lastVisibleHeight = _startHeight = SecondaryViewHeightRequest > 0
						? SecondaryViewHeightRequest
							: !isExpanding
						 ? SecondaryView.Height
								  : _lastVisibleHeight;
				_endHeight = 0;
			}

			_shouldIgnoreAnimation = Height < 0;

			if(shouldInvokeAnimation)
			{
				InvokeAnimation();
			}
		}

		private void OnShouldHandleTapToExpandChanged()
		{
			if(PrimaryView == null)
			{
				return;
			}

            var viewToAttachTapGesture = ExpandCommandView ?? PrimaryView;

            viewToAttachTapGesture.GestureRecognizers.Remove(_defaultTapGesture);

            if (ShouldHandleTapToExpand)
            {
                viewToAttachTapGesture.GestureRecognizers.Add(_defaultTapGesture);
            }
		}

		private void SetPrimaryView(View oldView)
		{
			if(oldView != null)
			{
				Children.Remove(oldView);
			}
			Children.Insert(0, PrimaryView);
		}

		private void SetSecondaryView(bool forceUpdate = false)
		{
			if(IsExpanded && (SecondaryView == null || forceUpdate))
			{
				SecondaryView = CreateSecondaryView();
			}
		}

		private View CreateSecondaryView()
		{
			var template = SecondaryViewTemplate;
			if(template is DataTemplateSelector selector)
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
			if (_shouldIgnoreAnimation)
			{
				SecondaryView.HeightRequest = _endHeight;
				SecondaryView.IsVisible = IsExpanded;
				return;
			}

            var parentAnimation = new Animation();
            var expandAnimation = new Animation(v => SecondaryView.HeightRequest = v, _startHeight, _endHeight, Easing.Linear);

            parentAnimation.Add(0, 1, expandAnimation);

            if(ShouldRotate)
            {
                var startRotation = 0;
                var endRotation = 180;
                if (!IsExpanded)
                {
                    startRotation = 180;
                    endRotation = 0;
                }

                var rotateAnimation = new Animation(v => ExpandCommandView.Rotation = v, startRotation, endRotation, Easing.BounceIn);
                parentAnimation.Add(0, 1, rotateAnimation);
            }

            parentAnimation.Commit(SecondaryView,
					ExpandAnimationName,
					finished: (v, r) =>
					{
						if (!IsExpanded)
						{
							SecondaryView.IsVisible = false;
						}
					});
		}
	}
}

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace Console.UI.Behaviors
{
    /// <summary>
    /// A WPF behaviour that allows for multitouch (touchscreen) manipulation of an attached FrameworkElement.
    /// Supports translation, scaling and rotation.
    /// </summary>
    public class MultitouchTransformBehavior : Behavior<FrameworkElement>
    {
        private readonly MultitouchTransformation _multitouch;

        public MultitouchTransformBehavior()
        {
            _multitouch = new MultitouchTransformation();
        }

        #region Behavior Overrides

        protected override void OnAttached()
        {
            // Multitouch properties are only updated on attachment.
            // Note that changing these properties on the behaviour at runtime will
            // not affect the multitouch behaviour. You may need to add additional code to
            // achieve that.
            _multitouch.AllowTranslateX = this.AllowTranslateX;
            _multitouch.AllowTranslateY = this.AllowTranslateY;
            _multitouch.AllowRotate = this.AllowRotate;
            _multitouch.AllowScale = this.AllowScale;
            _multitouch.MaximumScale = this.MaximumScale;
            _multitouch.MinimumScale = this.MinimumScale;
            _multitouch.AnimationDuration = this.Duration;
            _multitouch.AnimationEasingFunction = this.AnimationEasingFunction;

            _multitouch.Attach(this.AssociatedObject);
        }

        protected override void OnDetaching()
        {
            _multitouch.Detach();
        }

        #endregion Behavior Overrides

        public void Reset()
        {
            _multitouch.Reset(false);
        }

        public void ResetWithAnimation()
        {
            _multitouch.Reset(true);
        }

        public void ZoomInStep()
        {
            _multitouch.ZoomInStep();
        }

        public void ZoomOutStep()
        {
            _multitouch.ZoomOutStep();
        }
        

        #region Dependency Properties

        [Category("Common"),
       Description("The maximum scale that the element can be scaled up to")]
        public double MaximumScale
        {
            get { return (double)GetValue(MaximumScaleProperty); }
            set { SetValue(MaximumScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximumScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumScaleProperty =
            DependencyProperty.Register("MaximumScale", typeof(double), typeof(MultitouchTransformBehavior), new PropertyMetadata(10.0));

        [Category("Common"),
        Description("The minimum scale that the element can be scaled down to")]
        public double MinimumScale
        {
            get { return (double)GetValue(MinimumScaleProperty); }
            set { SetValue(MinimumScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimumScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumScaleProperty =
            DependencyProperty.Register("MinimumScale", typeof(double), typeof(MultitouchTransformBehavior), new PropertyMetadata(1.0));

        [Category("Common"),
        Description("Allow the element to be translated on the X axis")]
        public bool AllowTranslateX
        {
            get { return (bool)GetValue(AllowTranslateXProperty); }
            set { SetValue(AllowTranslateXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowTranslateX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowTranslateXProperty =
            DependencyProperty.Register("AllowTranslateX", typeof(bool), typeof(MultitouchTransformBehavior), new PropertyMetadata(true));

        [Category("Common"),
        Description("Allow the element to be translated on the Y axis")]
        public bool AllowTranslateY
        {
            get { return (bool)GetValue(AllowTranslateYProperty); }
            set { SetValue(AllowTranslateYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowTranslateY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowTranslateYProperty =
            DependencyProperty.Register("AllowTranslateY", typeof(bool), typeof(MultitouchTransformBehavior), new PropertyMetadata(true));

        [Category("Common"),
        Description("Allow the element to be scaled")]
        public bool AllowScale
        {
            get { return (bool)GetValue(AllowScaleProperty); }
            set { SetValue(AllowScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowScaleProperty =
            DependencyProperty.Register("AllowScale", typeof(bool), typeof(MultitouchTransformBehavior), new PropertyMetadata(true));

        [Category("Common"),
        Description("Allow the element to be rotated")]
        public bool AllowRotate
        {
            get { return (bool)GetValue(AllowRotateProperty); }
            set { SetValue(AllowRotateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowRotate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowRotateProperty =
            DependencyProperty.Register("AllowRotate", typeof(bool), typeof(MultitouchTransformBehavior), new PropertyMetadata(true));

        public IEasingFunction AnimationEasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        public static readonly DependencyProperty EasingFunctionProperty = 
            DependencyProperty.Register("AnimationEasingFunction", 
                typeof(IEasingFunction), typeof(MultitouchTransformBehavior), 
                new UIPropertyMetadata(new CubicEase() { EasingMode = EasingMode.EaseInOut }));

        public Duration Duration
        {
            get { return (Duration)GetValue(MyPropertyDurationProperty); }
            set { SetValue(MyPropertyDurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyPropertyDuration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyDurationProperty =
            DependencyProperty.Register("MyPropertyDuration", typeof(Duration), typeof(MultitouchTransformBehavior), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0.6))));

        #endregion Dependency Properties

    }
}
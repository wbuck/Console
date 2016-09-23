using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Console.UI.Behaviors
{
    public class MultitouchTransformation
    {
        private FrameworkElement _element; // The element the behaviour is attached to

        //private FrameworkElement _container; // A grid wrapper/container that is added by this behavior.
        private Matrix? _previousMatrix; // The matrix from any previous transformation.

        private DateTime _lastTouchDown; // The time of the last touch-down event.
        private LinearMatrixAnimation _animation; // An instance of the LinearMatrixAnimation timeline.

        private const double ZOOM_IN_SCALE = 1.5;
        private const double ZOOM_OUT_SCALE = 0.7;

        public MultitouchTransformation()
        {
            // Set detault property values
            this.MaximumScale = 5.0;
            this.MinimumScale = 1.0;
            this.AllowTranslateX = true;
            this.AllowTranslateY = true;
            this.AllowRotate = true;
            this.AllowScale = true;
            this.AnimationEasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
            this.AnimationDuration = new Duration(TimeSpan.FromSeconds(0.6));

            _animation = new LinearMatrixAnimation();
        }

        #region Properties and Events

        public event EventHandler TranslationCompleted;

        public double MaximumScale { get; set; }

        public double MinimumScale { get; set; }

        public bool AllowTranslateX { get; set; }

        public bool AllowTranslateY { get; set; }

        public bool AllowRotate { get; set; }

        public bool AllowScale { get; set; }

        public IEasingFunction AnimationEasingFunction { get; set; }

        public Duration AnimationDuration { get; set; }

        #endregion Properties and Events

        #region Public Methods

        public Matrix Reset(bool animate)
        {
            return TransformToMatrix(new MatrixTransform().Matrix, animate: animate);
        }

        /// <summary>
        /// Attaches event handlers to the FrameworkElement. Effectively starts the transformation
        /// behaviour by monitoring for touch events.
        /// </summary>
        public void Attach(FrameworkElement targetElement)
        {
            _element = targetElement;

            if (targetElement != null)
            {
                // Enable manipulation to allow multitouch events on the target element.
                targetElement.IsManipulationEnabled = true;

                // Wire up manipulation events for multitouch support.
                targetElement.ManipulationStarting += ManipulationStartingHandler;
                targetElement.ManipulationDelta += ManipulationDeltaHandler;
                targetElement.ManipulationCompleted += ManipulationCompletedHandler;

                // Wire up touch-down event, to add double-tap support.
                targetElement.TouchDown += TouchDownHandler;
            }
        }

        /// <summary>
        /// Detaches event handlers from the FrameworkElement. Effectively stops the tranformation
        /// behaviour. Should be called prior to disposing of this object.
        /// </summary>
        public void Detach()
        {
            var targetElement = _element;

            if (targetElement != null)
            {
                // Detach manipulation events.
                targetElement.ManipulationStarting -= ManipulationStartingHandler;
                targetElement.ManipulationDelta -= ManipulationDeltaHandler;
                targetElement.ManipulationCompleted -= ManipulationCompletedHandler;

                // Detach touch-down event.
                targetElement.TouchDown -= TouchDownHandler;
            }

            _element = null;
        }

        #endregion Public Methods

        #region Private Methods

        private Matrix TransformToMatrix(Matrix toMatrix, bool animate = true)
        {
            var matrixTransform = _element.RenderTransform as MatrixTransform;
            var matrix = matrixTransform.Matrix;

            if (matrixTransform.IsFrozen)
            {
                matrixTransform = new MatrixTransform(matrix);
                _element.RenderTransform = matrixTransform;
            }

            if (animate)
            {
                _animation.EasingFunction = this.AnimationEasingFunction;
                _animation.From = matrix;
                _animation.To = toMatrix;
                _animation.Duration = this.AnimationDuration;

                matrixTransform.BeginAnimation(MatrixTransform.MatrixProperty, _animation);
            }
            else
            {
                _element.RenderTransform = new MatrixTransform(toMatrix);
            }

            return matrix;
        }

        private bool Zoom(double scaleStep, Point centerPoint)
        {
            var matrixTransform = _element.RenderTransform as MatrixTransform;
            var matrix = matrixTransform.Matrix;

            if (matrixTransform.IsFrozen)
            {
                matrixTransform = new MatrixTransform(matrix);
                _element.RenderTransform = matrixTransform;
            }

            var newMatrix = new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);

            newMatrix.ScaleAt(scaleStep, scaleStep, centerPoint.X, centerPoint.Y);

            if (newMatrix.Determinant <= this.MinimumScale + 0.3)
            {
                Reset(true);
                return true;
            }
            else if (newMatrix.Determinant >= this.MaximumScale)
            {
                return false;
            }

            _animation.EasingFunction = this.AnimationEasingFunction;
            _animation.From = matrix;
            _animation.To = newMatrix;
            _animation.Duration = this.AnimationDuration;
            matrixTransform.BeginAnimation(MatrixTransform.MatrixProperty, _animation);

            _animation.Completed += (s, e) =>
            {
                if (!Utilities.IsElementVisibleWithinParent(_element))
                {
                    TransformToMatrix(matrix, true);
                }
            };

            return true;
        }

        public void ZoomInStep()
        {
            Zoom(ZOOM_IN_SCALE, new Point());
        }

        public void ZoomOutStep()
        {
            Zoom(ZOOM_OUT_SCALE, new Point());
        }

        private Point DoubleTapZoom(Point touchCentre)
        {
            Utilities.Delay(100).ContinueWith(t =>
            {
                if (!Zoom(ZOOM_IN_SCALE * 1.5, touchCentre))
                {
                    Reset(true);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            return touchCentre;
        }

        #endregion Private Methods

        #region Event Handlers

        private void ManipulationStartingHandler(object sender, System.Windows.Input.ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = ((FrameworkElement)e.Source).Parent as FrameworkElement; //e.ManipulationContainer = _container;
        }

        private void ManipulationDeltaHandler(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            var sourceElement = e.Source as FrameworkElement;
            if (sourceElement != null)
            {
                // e.DeltaManipulation has the changes
                // Scale is a delta multiplier; 1.0 is last size,  (so 1.1 == scale 10%, 0.8 = shrink 20%)
                // Rotate = Rotation, in degrees
                // Pan = Translation, == Translate offset, in Device Independent Pixels
                var deltaManipulation = e.DeltaManipulation;

                var matrixTransform = sourceElement.RenderTransform as MatrixTransform;

                if (matrixTransform == null)
                {
                    matrixTransform = new MatrixTransform();
                    sourceElement.RenderTransform = matrixTransform;
                }

                var matrix = matrixTransform.Matrix;

                if (_previousMatrix == null)
                {
                    _previousMatrix = matrix;
                }

                // Center the translation around the user's fingers
                Point center = ((FrameworkElement)e.ManipulationContainer)
                    .TranslatePoint(e.ManipulationOrigin, sourceElement);

                // transform it to take into account transforms from previous manipulations
                center = matrix.Transform(center);

                // Perform a scaling (zoom) tranformation
                if ((deltaManipulation.Scale.X < 1 && matrix.Determinant >= this.MinimumScale)
                    || (deltaManipulation.Scale.X > 1 && matrix.Determinant <= this.MaximumScale))
                {
                    matrix.ScaleAt(deltaManipulation.Scale.X, deltaManipulation.Scale.Y, center.X, center.Y);
                }

                // Prevent scaling below the minimum scale amount.
                if (matrix.Determinant < this.MinimumScale)
                {
                    return;
                }

                // Rotation
                if (this.AllowRotate)
                {
                    matrix.RotateAt(e.DeltaManipulation.Rotation, center.X, center.Y);
                }

                // Limit the X/Y translation extent to prevent the element from 'jumping' when using slow touchscreens, or many touch points.
                double translationThreshold = 100.0;

                // Perform a translation (pan) tranformation
                if ((deltaManipulation.Translation.X < translationThreshold &&
                    deltaManipulation.Translation.X > -translationThreshold) &&
                    (deltaManipulation.Translation.Y < translationThreshold &&
                    deltaManipulation.Translation.Y > -translationThreshold))
                {
                    matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
                }

                // Perform the render transform on the element.
                sourceElement.RenderTransform = new MatrixTransform(matrix);

                e.Handled = true;
            }
        }

        private void ManipulationCompletedHandler(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            // Called once the manipulation is complete (the user has removed their fingers from the screen).
            var element = e.Source as FrameworkElement;

            if (element != null)
            {
                var matrixTransform = element.RenderTransform as MatrixTransform;

                if (matrixTransform == null)
                {
                    matrixTransform = new MatrixTransform();
                    element.RenderTransform = matrixTransform;
                }

                var matrix = matrixTransform.Matrix;

                if (matrix.Determinant < this.MinimumScale + 0.3)
                {
                    matrix = Reset(animate: true);
                }
                else if (!Utilities.IsElementVisibleWithinParent(_element))
                {
                    TransformToMatrix(_previousMatrix ?? new MatrixTransform().Matrix, animate: true);
                }

                if (TranslationCompleted != null)
                {
                    TranslationCompleted(this, e);
                }
            }

            _previousMatrix = null;
        }

        private void TouchDownHandler(object sender, System.Windows.Input.TouchEventArgs e)
        {
            // Handle double tap for zoom gesture.
            if ((DateTime.Now - _lastTouchDown) < TimeSpan.FromMilliseconds(250))
            {
                // Get parent element
                var parentElement = ((FrameworkElement)sender).Parent as FrameworkElement;

                // Get centre point of the touch event
                Point touchCentre = e.TouchDevice.GetTouchPoint(parentElement).Position;

                touchCentre = parentElement.TranslatePoint(touchCentre, _element);

                // Delay the start of the zoom animation, to allow chance for the user to lift their finger.
                touchCentre = DoubleTapZoom(touchCentre);
            }

            _lastTouchDown = DateTime.Now;
        }

        #endregion Event Handlers
    }
}
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Console.UI.Behaviors
{
    // A simple animation timeline for animating between LinearMatrices.
    // Based on various code examples obtained from this article on Stack Overflow:
    // http://stackoverflow.com/questions/1988421/smooth-animation-using-matrixtransform
    // Matrix animation timeline code inspired by: http://stackoverflow.com/users/98313/luken
    // Easing code by: http://stackoverflow.com/users/545561/bor
    public class LinearMatrixAnimation : AnimationTimeline
    {
        #region Dependency Properties

        public Matrix? From
        {
            set { SetValue(FromProperty, value); }
            get { return (Matrix)GetValue(FromProperty); }
        }

        public static DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Matrix?), typeof(LinearMatrixAnimation), new PropertyMetadata(null));

        public Matrix? To
        {
            set { SetValue(ToProperty, value); }
            get { return (Matrix)GetValue(ToProperty); }
        }

        public static DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Matrix?), typeof(LinearMatrixAnimation), new PropertyMetadata(null));
    
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(LinearMatrixAnimation), new UIPropertyMetadata(null));

        #endregion Dependency Properties

        public LinearMatrixAnimation()
        {
            if (this.EasingFunction == null)
            {
                this.EasingFunction = new CubicEase();
            }
        }

        public LinearMatrixAnimation(Matrix from, Matrix to, Duration duration)
            : this()
        {
            Duration = duration;
            From = from;
            To = to;
        }

        #region AnimationTimeline Method Overrides

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            if (animationClock.CurrentProgress == null)
            {
                return null;
            }
         
            double progress = animationClock.CurrentProgress.Value;
            Matrix from = From ?? (Matrix)defaultOriginValue;

            // Apply an easing function if one has been set.
            if (this.EasingFunction != null)
            {
                progress = this.EasingFunction.Ease(progress);
            }

            if (To.HasValue)
            {
                Matrix to = To.Value;
                                
                // Note: This new matrix has limited support for animation of rotation. It simply
                // animates all the matrix values to their new position ... which will result in a
                // percieved 'flip' of the element if it was rotated past 90-degrees.
                Matrix newMatrix = new Matrix(
                    ((to.M11 - from.M11) * progress) + from.M11,
                    ((to.M12 - from.M12) * progress) + from.M12,
                    ((to.M21 - from.M21) * progress) + from.M21,
                    ((to.M22 - from.M22) * progress) + from.M22,
                    ((to.OffsetX - from.OffsetX) * progress) + from.OffsetX,
                    ((to.OffsetY - from.OffsetY) * progress) + from.OffsetY);
                return newMatrix;
            }

            return Matrix.Identity;
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new LinearMatrixAnimation();
        }

        public override System.Type TargetPropertyType
        {
            get { return typeof(Matrix); }
        }

        #endregion AnimationTimeline Method Overrides

        
    }
}
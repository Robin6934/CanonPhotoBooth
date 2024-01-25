using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PhotoBooth
{
    internal class CountDown
    {
        public delegate void TimerCallback();

        public event TimerCallback CountDownExpired;

        public event TimerCallback CountDownEarly;

        private TextBlock _textBlockCountdown = new TextBlock();

        private DispatcherTimer _timer = new DispatcherTimer();

        private readonly double _circumference = 0;

        private int _countDownTime = 0;

        private readonly int _countDownTotal = 0;

        private readonly long _totalTicks = 0;

        private readonly Point _point;

        private ArcSegment _arc;

        private Path _path;

        private const double _startAngle = 359;

        private double _angle = 0;

        private long _timerTicks = 0;

        private readonly double _earlySeconds = 0;

        private readonly Canvas _canvas;

        /// <summary>
        /// This class is used to create a countdown which will be displayed on the screen
        /// </summary>
        /// <param name="Time">The time from which the countdown will start</param>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="point">This point will be the middle point of the countdown on the canvas</param>
        /// <param name="canvas">The canvas on which the Countdown will be drawn</param>
        public CountDown(int Time, double radius, Point point, Canvas canvas)
        {
            _angle = _startAngle;
            _countDownTime = Time;
            _countDownTotal = Time;
            _point = point;
            _circumference = radius;
            _canvas = canvas;
            _timer.Tick += Timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(Time / _angle);
            _totalTicks = TimeSpan.TicksPerSecond * Time;
        }

        /// <summary>
        /// This class is used to create a countdown which will be displayed on the screen
        /// </summary>
        /// <param name="Time">The time from which the countdown will start</param>
        /// <param name="early">When this time is remaining the early function is called</param>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="point">This point will be the middle point of the countdown on the canvas</param>
        /// <param name="canvas">The canvas on which the Countdown will be drawn</param>
        public CountDown(int Time, double early, double radius, Point point, Canvas canvas)
        {
            if (early >= Time)
            {
                throw new ArgumentException("Early seconds cant be lager than the total countdown time!!");
            }

            _angle = _startAngle;
            _countDownTime = Time;
            _countDownTotal = Time;
            _earlySeconds = early;
            _point = point;
            _circumference = radius;
            _canvas = canvas;
            _timer.Tick += Timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(Time / _angle);
            _totalTicks = TimeSpan.TicksPerSecond * Time;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_timerTicks % TimeSpan.TicksPerSecond <= _timer.Interval.Ticks - 1 && _timerTicks != 0)
            {
                _countDownTime--;
                _textBlockCountdown.Text = _countDownTime.ToString();
            }

            if (_timerTicks <= _totalTicks - (TimeSpan.TicksPerSecond * _earlySeconds - _timer.Interval.Ticks / 2) &&
               _timerTicks >= _totalTicks - (TimeSpan.TicksPerSecond * _earlySeconds + _timer.Interval.Ticks / 2) &&
               _earlySeconds != 0)
            {
                CountDownEarly();
            }

            SetAngleMinusOne();

            if (_timerTicks >= _totalTicks)
            {
                _timer.Stop();
                CountDownExpired();
                _canvas.Children.Remove(_textBlockCountdown);
                _canvas.Children.Remove(_path);
            }

            _timerTicks += _timer.Interval.Ticks;

        }

        /// <summary>
        /// Starts the countdown
        /// </summary>
        public void StartCountdown()
        {
            _timerTicks = 0;

            _angle = _startAngle;

            _countDownTime = _countDownTotal;

            DrawTextBoxCountdown();

            CreateArc();

            _timer.Start();
        }

        private void CreateArc()
        {
            var endPoint = new Point(
                _circumference * -Math.Sin(_angle * Math.PI / 180),
                _circumference * -Math.Cos(_angle * Math.PI / 180));

            _arc = new ArcSegment(
                endPoint, new Size(_circumference, _circumference), 0,
                _angle >= 180, SweepDirection.Counterclockwise, true);

            var figure = new PathFigure { StartPoint = new Point(0, -_circumference) };
            figure.Segments.Add(_arc);

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            _path = new Path();
            _path.Data = geometry;

            // Set the stroke (outline) color and thickness
            _path.Stroke = Brushes.Black;      // You can change this color as needed
            _path.StrokeThickness = 20;  // You can change this thickness as needed

            // Set the position of the path
            Canvas.SetLeft(_path, _point.X);
            Canvas.SetTop(_path, _point.Y);

            // Add the Path element to the canvas
            _canvas.Children.Add(_path);

        }

        private void SetAngleMinusOne()
        {
            _angle--;

            var endPoint = new Point(
                _circumference * -Math.Sin(_angle * Math.PI / 180),
                _circumference * -Math.Cos(_angle * Math.PI / 180));

            _arc.Point = endPoint;

            _arc.IsLargeArc = _angle >= 180;

            _canvas.UpdateLayout();
        }

        private void DrawTextBoxCountdown()
        {
            _textBlockCountdown.VerticalAlignment = VerticalAlignment.Center;

            _textBlockCountdown.HorizontalAlignment = HorizontalAlignment.Center;

            _textBlockCountdown.FontSize = 150;

            _textBlockCountdown.TextAlignment = TextAlignment.Center;

            _textBlockCountdown.Text = _countDownTime.ToString();

            _textBlockCountdown.Loaded += TextBlockCountdown_Loaded;

            _textBlockCountdown.RenderTransformOrigin = new Point(0.5, 0.5);

            _textBlockCountdown.RenderTransform = new ScaleTransform
            {
                ScaleX = -1
            };

            _canvas.Children.Add(_textBlockCountdown);
        }

        private void TextBlockCountdown_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(_textBlockCountdown, _point.X - _textBlockCountdown.ActualWidth / 2);
            Canvas.SetTop(_textBlockCountdown, _point.Y - _textBlockCountdown.ActualHeight / 2);
        }
    }
}

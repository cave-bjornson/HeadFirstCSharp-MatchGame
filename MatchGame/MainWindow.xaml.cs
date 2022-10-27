using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Linq;
using WinRT;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MatchGame
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        private readonly DispatcherTimer _timer = new();
        private int _tenthOfSecondsElapsed;
        private int _matchesFound;
        private TextBlock? _lastTextBlockClicked;

        private readonly string[] _animalEmoji = new string[]
        {
            "🦄",
            "🦄",
            "🐘",
            "🐘",
            "🦇",
            "🦇",
            "🦆",
            "🦆",
            "🦖",
            "🦖",
            "🐙",
            "🐙",
            "🐹",
            "🐹",
            "🐳",
            "🐳"
        };

        public MainWindow()
        {
            InitializeComponent();

            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += TimerOnTick;
            SetUpGame();
        }

        private void TimerOnTick(object? sender, object e)
        {
            _tenthOfSecondsElapsed++;
            TimeTextBlock.Text = (_tenthOfSecondsElapsed / 10f).ToString("0.0s");
            if (_matchesFound == 8)
            {
                _timer.Stop();
                TimeTextBlock.Text += " - Play again?";
            }
        }

        private void SetUpGame()
        {
            var shuffledEmojis = _animalEmoji.OrderBy(_ => Random.Shared.Next()).ToList();
            var textBlocks = MainGrid.Children
                .OfType<TextBlock>()
                .Where(tb => tb.Tag?.As<string>() == "emoji");
            foreach (
                var a in textBlocks.Zip(
                    shuffledEmojis,
                    (tb, se) => new { TextBlock = tb, Emoji = se }
                )
            )
            {
                a.TextBlock.Text = a.Emoji;
                a.TextBlock.Visibility = Visibility.Visible;
            }

            _tenthOfSecondsElapsed = 0;
            _matchesFound = 0;
            _timer.Start();
        }

        private void TextBlock_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            ArgumentNullException.ThrowIfNull(textBlock);

            if (_lastTextBlockClicked is not null)
            {
                if (textBlock.Text == _lastTextBlockClicked.Text)
                {
                    _matchesFound++;
                    textBlock.Visibility = Visibility.Collapsed;
                    _lastTextBlockClicked = null;
                }
                else
                {
                    _lastTextBlockClicked.Visibility = Visibility.Visible;
                    _lastTextBlockClicked = null;
                }
            }
            else
            {
                textBlock.Visibility = Visibility.Collapsed;
                _lastTextBlockClicked = textBlock;
            }
        }

        private void TimeTextBlock_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_matchesFound == 8)
            {
                SetUpGame();
            }
        }
    }
}

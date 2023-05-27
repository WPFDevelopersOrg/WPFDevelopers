using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFDevelopers.Samples.Controls
{
    public class EmojiHelper
    {
        private static readonly Lazy<EmojiHelper> lazy =
            new Lazy<EmojiHelper>(() => new EmojiHelper());

        /// <summary>
        ///     Emoji Size 默认30*30
        /// </summary>
        public double _emojiWidth = 30, _emojiHeight = 30;

        /// <summary>
        ///     Emoji 字典 Key ，Value
        /// </summary>
        public Dictionary<string, string> m_Emojis = null;

        private EmojiHelper()
        {
        }

        public static EmojiHelper Instance => lazy.Value;

        public void ParseText(FrameworkElement element)
        {
            TextBlock textBlock = null;
            var textBox = element as RichTextBox;
            if (textBox == null)
                textBlock = element as TextBlock;

            if (textBox == null && textBlock == null)
                return;

            if (textBox != null)
            {
                var doc = textBox.Document;
                for (var blockIndex = 0; blockIndex < doc.Blocks.Count; blockIndex++)
                {
                    var b = doc.Blocks.ElementAt(blockIndex);
                    var p = b as Paragraph;
                    if (p != null) ProcessInlines(textBox, p.Inlines);
                }
            }
            else
            {
                ProcessInlines(null, textBlock.Inlines);
            }
        }

        private void ProcessInlines(RichTextBox textBox, InlineCollection inlines)
        {
            for (var inlineIndex = 0; inlineIndex < inlines.Count; inlineIndex++)
            {
                var i = inlines.ElementAt(inlineIndex);
                if (i is Run)
                {
                    var r = i as Run;
                    var text = r.Text;
                    var emoticonFound = string.Empty;
                    var index = FindFirstEmoticon(text, 0, out emoticonFound);
                    if (index >= 0)
                    {
                        var tp = i.ContentStart;
                        var reposition = false;
                        while (!tp.GetTextInRun(LogicalDirection.Forward).StartsWith(emoticonFound))
                            tp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                        var end = tp;
                        for (var j = 0; j < emoticonFound.Length; j++)
                            end = end.GetNextInsertionPosition(LogicalDirection.Forward);
                        var tr = new TextRange(tp, end);
                        if (textBox != null)
                            reposition = textBox.CaretPosition.CompareTo(tr.End) == 0;
                        tr.Text = string.Empty;

                        var imageFile = m_Emojis[emoticonFound];
                        var image = new Image { Stretch = Stretch.Fill, Width = _emojiWidth, Height = _emojiHeight };
                        image.BeginInit();
                        image.Source = new BitmapImage(new Uri(imageFile));
                        image.EndInit();


                        var iui = new InlineUIContainer(image, tp);
                        iui.BaselineAlignment = BaselineAlignment.Center;

                        if (textBox != null && reposition)
                            textBox.CaretPosition = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                    }
                }
            }
        }

        private int FindFirstEmoticon(string text, int startIndex, out string emoticonFound)
        {
            emoticonFound = string.Empty;
            var minIndex = -1;
            foreach (var e in m_Emojis.Keys)
            {
                var index = text.IndexOf(e, startIndex);
                if (index >= 0)
                    if (minIndex < 0 || index < minIndex)
                    {
                        minIndex = index;
                        emoticonFound = e;
                    }
            }

            return minIndex;
        }
    }
}
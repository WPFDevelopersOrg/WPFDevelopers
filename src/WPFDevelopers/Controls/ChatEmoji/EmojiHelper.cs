using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFDevelopers.Controls
{
    public class EmojiHelper
    {
        private static readonly Lazy<EmojiHelper> lazy =
        new Lazy<EmojiHelper>(() => new EmojiHelper());

        public static EmojiHelper Instance { get { return lazy.Value; } }

        private EmojiHelper()
        {
        }
        /// <summary>
        /// Emoji 字典 Key ，Value
        /// </summary>
        public Dictionary<string, string> m_Emojis = null;
        /// <summary>
        /// Emoji Size 默认30*30
        /// </summary>
        public double _emojiWidth = 30, _emojiHeight = 30;

        public void ParseText(FrameworkElement element)
        {
            TextBlock textBlock = null;
            RichTextBox textBox = element as RichTextBox;
            if (textBox == null)
                textBlock = element as TextBlock;

            if (textBox == null && textBlock == null)
                return;

            if (textBox != null)
            {
                FlowDocument doc = textBox.Document;
                for (int blockIndex = 0; blockIndex < doc.Blocks.Count; blockIndex++)
                {
                    Block b = doc.Blocks.ElementAt(blockIndex);
                    Paragraph p = b as Paragraph;
                    if (p != null)
                    {
                        ProcessInlines(textBox, p.Inlines);
                    }
                }
            }
            else
            {
                ProcessInlines(null, textBlock.Inlines);
            }
        }

        private void ProcessInlines(RichTextBox textBox, InlineCollection inlines)
        {
            for (int inlineIndex = 0; inlineIndex < inlines.Count; inlineIndex++)
            {
                Inline i = inlines.ElementAt(inlineIndex);
                if (i is Run)
                {
                    Run r = i as Run;
                    string text = r.Text;
                    string emoticonFound = string.Empty;
                    int index = FindFirstEmoticon(text, 0, out emoticonFound);
                    if (index >= 0)
                    {
                        TextPointer tp = i.ContentStart;
                        bool reposition = false;
                        while (!tp.GetTextInRun(LogicalDirection.Forward).StartsWith(emoticonFound))
                            tp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                        TextPointer end = tp;
                        for (int j = 0; j < emoticonFound.Length; j++)
                            end = end.GetNextInsertionPosition(LogicalDirection.Forward);
                        TextRange tr = new TextRange(tp, end);
                        if (textBox != null)
                            reposition = textBox.CaretPosition.CompareTo(tr.End) == 0;
                        tr.Text = string.Empty;

                        string imageFile = m_Emojis[emoticonFound];
                        Image image = new Image() { Stretch = Stretch.Fill, Width = _emojiWidth, Height = _emojiHeight };
                        image.BeginInit();
                        image.Source = new BitmapImage(new Uri(imageFile));
                        image.EndInit();


                        InlineUIContainer iui = new InlineUIContainer(image, tp);
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
            int minIndex = -1;
            foreach (string e in m_Emojis.Keys)
            {
                int index = text.IndexOf(e, startIndex);
                if (index >= 0)
                {
                    if (minIndex < 0 || index < minIndex)
                    {
                        minIndex = index;
                        emoticonFound = e;
                    }
                }
            }
            return minIndex;
        }

    }
}

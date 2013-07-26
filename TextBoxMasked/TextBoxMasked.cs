/*************************************************************************************
 *
 *   WPF TextBoxMasked 1.0
 *
 *   Copyright (C) 2013 Rikki Mongoose.
 *   
 *   Licensed under the Apache License, Version 2.0 (the "License");
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 *   Unless required by applicable law or agreed to in writing, software
 *   distributed under the License is distributed on an "AS IS" BASIS,
 *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *   See the License for the specific language governing permissions and
 *   limitations under the License.
 * 
 *   Component is based on MaskedTextBox from AvalonControlsLibrary (http://avaloncontrolslib.codeplex.com/).
 *   
 *   Extended with 100% masked text formatting support (see http://msdn.microsoft.com/en-us/library/system.windows.forms.maskedtextbox.mask.aspx)
 *   and some bugfixes were implemented as well.
 *   
 *   http://github.com/rikkimongoose/WPF-TextBoxMasked
 * 
 *   ***********************************************************************************/


using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TextBoxMasked.Filter;

namespace TextBoxMasked
{
    public class TextBoxMasked : TextBox
    {
        #region Properties


        /// <summary>
        /// Gets or sets the cashed mask to apply to the textbox
        /// </summary>
        private MaskedTextProvider MaskProviderCashed
        {
            get { return (MaskedTextProvider)GetValue(MaskProviderCashedProperty); }
            set { SetValue(MaskProviderCashedProperty, value); }
        }

        /// <summary>
        /// Dependency property to store the cashed mask to apply to the textbox
        /// </summary>
        private static readonly DependencyProperty MaskProviderCashedProperty =
            DependencyProperty.Register("MaskProviderCashed", typeof(MaskedTextProvider), typeof(TextBoxMasked), new UIPropertyMetadata(null, MaskChanged));

        /// <summary>
        /// Gets or sets the cashed mask format string to apply to the textbox
        /// </summary>
        private String MaskProviderCashedMask
        {
            get { return (String)GetValue(MaskProviderCashedMaskProperty); }
            set { SetValue(MaskProviderCashedMaskProperty, value); }
        }

        /// <summary>
        /// Dependency property to store the mask format string to apply to the textbox
        /// </summary>
        private static readonly DependencyProperty MaskProviderCashedMaskProperty =
            DependencyProperty.Register("MaskProviderCashedMask", typeof(string), typeof(TextBoxMasked), new UIPropertyMetadata(String.Empty, MaskChanged));

        /// <summary>
        /// Gets the MaskTextProvider for the specified Mask
        /// </summary>
        public MaskedTextProvider MaskProvider
        {
            get
            {
                if (!IsMaskProviderUpdated())
                    return MaskProviderCashed;
                MaskProviderCashedMask = Mask;
                if (!String.IsNullOrEmpty(MaskProviderCashedMask))
                {
                    MaskProviderCashed = new MaskedTextProvider(MaskProviderCashedMask) { PromptChar = this.PromptChar };
                }
                else
                {
                    MaskProviderCashed = null;
                }
                if (MaskProviderCashed != null)
                {
                    MaskProviderCashed.Set(Text);
                }
                return MaskProviderCashed;
            }
        }

        /// <summary>
        /// Check, is configuration of MaskProvider changed
        /// </summary>
        /// <returns><c>true</c>, if it was changed and <c>false</c>, if it wasn't.</returns>
        private bool IsMaskProviderUpdated()
        {
            bool result = false;
            if (MaskProviderCashedMask != Mask)
            {
                MaskProviderCashedMask = Mask;
                result = true;
            }
            if (PromptCharCached != PromptChar)
            {
                PromptCharCached = PromptChar;
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Gets or sets the cashed promt char to apply to the textbox mask
        /// </summary>
        private char PromptCharCached
        {
            get { return (char)GetValue(PromptCharCachedProperty); }
            set { SetValue(PromptCharCachedProperty, value); }
        }

        /// <summary>
        /// Dependency property to store the cashed promt char to apply to the textbox mask
        /// </summary>
        private static readonly DependencyProperty PromptCharCachedProperty =
            DependencyProperty.Register("PromptCharCached", typeof(char), typeof(TextBoxMasked), new UIPropertyMetadata(' ', MaskChanged));

        /// <summary>
        /// Gets or sets the promt char to apply to the textbox mask
        /// </summary>
        public char PromptChar
        {
            get { return (char)GetValue(PromptCharProperty); }
            set { SetValue(PromptCharProperty, value); }
        }

        /// <summary>
        /// Dependency property to store the promt char to apply to the textbox mask
        /// </summary>
        public static readonly DependencyProperty PromptCharProperty =
            DependencyProperty.Register("PromptChar", typeof(char), typeof(TextBoxMasked), new UIPropertyMetadata(' ', MaskChanged));

        /// <summary>
        /// Gets or sets the mask to apply to the textbox
        /// </summary>
        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        /// <summary>
        /// Dependency property to store the mask to apply to the textbox
        /// </summary>
        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(string), typeof(TextBoxMasked), new UIPropertyMetadata(String.Empty, MaskChanged));

        //callback for when the Mask property is changed
        static void MaskChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //make sure to update the text if the mask changes
            var textBox = (TextBoxMasked)sender;
            textBox.RefreshText(textBox.MaskProvider, 0);
        }

        /// <summary>
        /// Gets the RegExFilter for the validation Mask.
        /// </summary>
        private BaseFilter FilterValidator
        {
            get
            {
                return TextBoxMaskedFilterProvider.Instance.FilterForMaskedType(Filter); ;
            }
        }


        /// <summary>
        /// Dependency property to store the filter to apply to the textbox
        /// </summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(TextBoxMaskedFilterType), typeof(TextBoxMasked), new UIPropertyMetadata(TextBoxMaskedFilterType.Any, MaskChanged));

        /// <summary>
        /// Gets a predefined filter for the specified RegExp
        /// </summary>
        public TextBoxMaskedFilterType Filter
        {

            get { return (TextBoxMaskedFilterType)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// Static Constructor
        /// </summary>
        static TextBoxMasked()
        {
            //override the meta data for the Text Proeprty of the textbox 
            var metaData = new FrameworkPropertyMetadata { CoerceValueCallback = ForceText };
            TextProperty.OverrideMetadata(typeof(TextBoxMasked), metaData);
        }

        //force the text of the control to use the mask
        private static object ForceText(DependencyObject sender, object value)
        {
            var textBox = (TextBoxMasked)sender;
            if (!String.IsNullOrEmpty(textBox.Mask))
            {
                var provider = textBox.MaskProvider;
                if (provider != null)
                {
                    provider.Set((string)value);
                    return provider.ToDisplayString();
                }
            }
            return value;
        }

        ///<summary>
        /// Default  constructor
        ///</summary>
        public TextBoxMasked()
        {
            //cancel the paste and cut command
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, null, CancelCommand));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, null, CancelCommand));
        }

        //cancel the command
        private static void CancelCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }

        #region Overrides

        /// <summary>
        /// override this method to replace the characters enetered with the mask
        /// </summary>
        /// <param name="e">Arguments for event</param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            //if the text is readonly do not add the text
            if (IsReadOnly)
            {
                e.Handled = true;
                return;
            }

            int position = SelectionStart;
            var provider = MaskProvider;
            bool ifIsPositionInMiddle = position < Text.Length;
            if (provider != null)
            {
                if (ifIsPositionInMiddle)
                {
                    position = GetNextCharacterPosition(position);

                    if (Keyboard.IsKeyToggled(Key.Insert))
                    {
                        if (provider.Replace(e.Text, position))
                            position++;
                    }
                    else
                    {
                        if (provider.InsertAt(e.Text, position))
                            position++;
                    }

                    position = GetNextCharacterPosition(position);
                }

                RefreshText(provider, position);
                e.Handled = true;
            }

            String textToText = (ifIsPositionInMiddle)
                                    ? Text.Insert(position, e.Text)
                                    : String.Format("{0}{1}", Text, e.Text);
            if (!FilterValidator.IsTextValid(textToText))
            {
                e.Handled = true;
            }
            base.OnPreviewTextInput(e);
        }

        /// <summary>
        /// override the key down to handle delete of a character
        /// </summary>
        /// <param name="e">Arguments for the event</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            var provider = MaskProvider;
            if (provider == null)
                return;            
            int position = SelectionStart;
            switch (e.Key)
            {
                case Key.Delete:
                    if (position < Text.Length)
                    {
                        if (provider.RemoveAt(position))
                            RefreshText(provider, position);

                        e.Handled = true;
                    }
                    break;
                case Key.Space:
                    if (provider.InsertAt(" ", position))
                        RefreshText(provider, position);
                    e.Handled = true;
                    break;
                case Key.Back:
                    if (position > 0)
                    {
                        position--;
                        if (provider.RemoveAt(position))
                            RefreshText(provider, position);
                    }
                    e.Handled = true;
                    break;
            }
        }
        #endregion

        #region Helper Methods

        //refreshes the text of the textbox
        private void RefreshText(MaskedTextProvider provider, int position)
        {
            if (provider != null)
            {
                Text = provider.ToDisplayString();
                SelectionStart = position;
            }
        }
        //gets the next position in the textbox to move
        private int GetNextCharacterPosition(int startPosition)
        {
            if (MaskProvider != null)
            {
                int position = MaskProvider.FindEditPositionFrom(startPosition, true);
                if (position != -1)
                    return position;
            }
            return startPosition;
        }
        #endregion
    }
}

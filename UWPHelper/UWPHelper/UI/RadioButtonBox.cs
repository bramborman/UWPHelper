/*
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.UI
{
    public class RadioButtonBox : StackPanel
    {
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(RadioButtonBox), new PropertyMetadata((int)-1, OnSelectedIndexPropertyChanged));
        public static readonly DependencyProperty SelectedItemProperty  = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(RadioButtonBox), new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public string Header { get; set; }/// /////////////////////////////////////////////////////////////////////////////////////////////////

        public event SelectionChangedEventHandler SelectionChanged;

        public RadioButtonBox() : base()
        {
            foreach (UIElement uIElement in Children)
            {
                RadioButton radioButton = uIElement as RadioButton;

                if (radioButton != null)
                {
                    radioButton.Checked += (sender, e) =>
                    {
                        SelectedItem = sender;
                    };

                    if (radioButton.IsChecked == true)
                    {
                        if (SelectedItem == null)
                        {
                            SelectedItem = radioButton;
                        }
                        else
                        {
                            radioButton.IsChecked = false;
                        }
                    }
                }
            }
        }

        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadioButtonBox radioButtonBox = (RadioButtonBox)d;
            int newValue = (int)e.NewValue;

            if (newValue >= 0)
            {
                OnPropertyChanged(radioButtonBox, radioButtonBox.Children[(int)e.OldValue] as RadioButton, radioButtonBox.Children[newValue] as RadioButton);
            }
            else
            {
                radioButtonBox.SelectedItem = null;
            }
        }

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadioButtonBox radioButtonBox = (RadioButtonBox)d;

            if (e.NewValue != null)
            {
                OnPropertyChanged(radioButtonBox, e.OldValue as RadioButton, e.NewValue as RadioButton);
            }
            else
            {
                radioButtonBox.SelectedIndex = -1;
            }
        }

        private static void OnPropertyChanged(RadioButtonBox radioButtonBox, RadioButton oldRadioButton, RadioButton newRadioButton)
        {
            if (oldRadioButton != null)
            {
                oldRadioButton.IsChecked = false;
            }

            if (newRadioButton != null)
            {
                newRadioButton.IsChecked     = true;
                radioButtonBox.SelectedIndex = radioButtonBox.Children.IndexOf(newRadioButton);
            }
            
            radioButtonBox.SelectedItem = newRadioButton;

            List<object> removedItems = new List<object>() { oldRadioButton };
            List<object> addedItems   = new List<object>() { newRadioButton };
            radioButtonBox.SelectionChanged?.Invoke(radioButtonBox, new SelectionChangedEventArgs(removedItems, addedItems));
        }
    }
}
*/
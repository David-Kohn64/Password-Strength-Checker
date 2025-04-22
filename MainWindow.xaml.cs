using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Zxcvbn;


namespace PasswordStrengthChecker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = PasswordBox.Password;
            PasswordTextBox.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Collapsed;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = PasswordTextBox.Text;
            PasswordBox.Visibility = Visibility.Visible;
            PasswordTextBox.Visibility = Visibility.Collapsed;
        }
        private void PasswordText_Changed(object sender, TextChangedEventArgs e)
        {
            if (ShowPassword.IsChecked == true)
            {
                PasswordBox.Password = PasswordTextBox.Text;
            }
        }

        Boolean _noVowels = false;
        private void Click_NoVowels(object sender, RoutedEventArgs e)
        {
            if (_noVowels)
            {
                Secret_NoVowels.Content = "Secret";
                Secret_NoVowels.Background = Brushes.SkyBlue;
                StrengthLabel.Content = "Strength: Very Weak";
               _noVowels = false;
            }
            else
            {
                Secret_NoVowels.Content = "Original";
                Secret_NoVowels.Background = Brushes.AliceBlue;
                StrengthLabel.Content = "Secret Strength: Very Weak";
                _noVowels = true;
            }
            PasswordBox.Password = "";
            PasswordTextBox.Text = "";
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            if (string.IsNullOrEmpty(password))
            {
                StrengthBar.Value = 0;
                StrengthLabel.Content = _noVowels ? "Secret Strength: Very Weak" : "Strength: Very Weak";
                StrengthLabel.Foreground = Brushes.Red;
                return;
            }

            if (_noVowels)
            {
                int customScore = EvaluateNoVowels(password);
                StrengthBar.Value = customScore;
                UpdatePasswordStrengthUI(customScore);  
            }
            else
            {
                var zxcvbnResult = Zxcvbn.Core.EvaluatePassword(password);
                StrengthBar.Value = zxcvbnResult.Score;
                UpdatePasswordStrengthUI(zxcvbnResult.Score);  
            }

        }

        private void UpdatePasswordStrengthUI(int score)
        {
            switch (score)
            {
                case 0:
                    StrengthLabel.Content = _noVowels ? "Secret Strength: Very Weak" : "Strength: Very Weak";
                    StrengthLabel.Foreground = Brushes.Red;
                    StrengthBar.Foreground = Brushes.Red;
                    break;
                case 1:
                    StrengthLabel.Content = _noVowels ? "Secret Strength: Weak" : "Strength: Weak";
                    StrengthLabel.Foreground = Brushes.OrangeRed;
                    StrengthBar.Foreground = Brushes.OrangeRed;
                    break;
                case 2:
                    StrengthLabel.Content = _noVowels ? "Secret Strength: Okay" : "Strength: Okay";
                    StrengthLabel.Foreground = Brushes.Orange;
                    StrengthBar.Foreground = Brushes.Orange;
                    break;
                case 3:
                    StrengthLabel.Content = _noVowels ? "Secret Strength: Strong" : "Strength: Strong";
                    StrengthLabel.Foreground = Brushes.YellowGreen;
                    StrengthBar.Foreground = Brushes.YellowGreen;
                    break;
                case 4:
                    StrengthLabel.Content = _noVowels ? "Secret Strength: Very Strong" : "Strength: Very Strong";
                    StrengthLabel.Foreground = Brushes.Green;
                    StrengthBar.Foreground = Brushes.Green;
                    break;
            }
        }

        private int EvaluateNoVowels(string password)
        {
            Dictionary<char, int> letterCounts = new Dictionary<char, int>();
            int score = 0; 
            int vowelCounter = 0;
            int symbolCounter = 0;
            int duplicates = 0;
            int length = password.Length;
            int penalty = 0;

            if (length >= 5) { score++; }
            if (length >= 10) { score++; }
            if (length >= 15) { score++; }
            if (length >= 20) { score++; }

            foreach (char c in password)
            {
                if ("aeiouAEIOU".Contains(c))
                {
                    vowelCounter++;
                }
                if (char.IsDigit(c) || char.IsSymbol(c) || char.IsPunctuation(c))
                {
                    symbolCounter++;
                }
                if (letterCounts.ContainsKey(c)) { letterCounts[c]++; }
                else { letterCounts[c] = 1;}
                
                if (duplicates == 0 && letterCounts[c] >= 3)
                {
                    duplicates++;
                }
            }
            if (vowelCounter > 0) { penalty++; }
            if (vowelCounter >= 3) { penalty++; }

            if (symbolCounter > 0) { penalty++; }
            if (symbolCounter >= 3) { penalty++; }

            if (duplicates > 0) { penalty += 2; }

            score -= penalty;
            if (score < 0) { score = 0; }
            return score;
        }

        private void StrengthBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }
    }
}
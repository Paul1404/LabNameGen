using System.Windows;
using System.Net;
using System.Windows.Controls;

namespace LabNameGen
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Generate Hostname and Validate Inputs
        private void GenerateHostname_Click(object sender, RoutedEventArgs e)
        {
            string ip = IpTextBox.Text.Replace(".", "");
            string os = (OsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string hypervisor = (HypervisorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(os) || string.IsNullOrEmpty(hypervisor))
            {
                ResultLabel.Text = "Please fill in all fields.";
                return;
            }

            if (!IsValidIp(IpTextBox.Text))
            {
                ResultLabel.Text = "Invalid IP Address format.";
                return;
            }

            // Ensure lowercase and best-practice formatting for hostnames
            string hostname = $"{hypervisor}-{os}-{ip}".ToLower();
            hostname = CleanHostname(hostname);

            ResultLabel.Text = hostname;
        }

        // Copy hostname silently to clipboard
        private void CopyHostname_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ResultLabel.Text))
            {
                Clipboard.SetText(ResultLabel.Text);
            }
        }

        // Clear all fields
        private void ClearFields_Click(object sender, RoutedEventArgs e)
        {
            IpTextBox.Clear();
            OsComboBox.SelectedIndex = -1;
            HypervisorComboBox.SelectedIndex = -1;
            ResultLabel.Text = "";
        }

        // Validate IP Address format
        private bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }

        // Clean hostname to remove any special characters and enforce hostname best practices
        private string CleanHostname(string hostname)
        {
            // Allow only alphanumeric characters and hyphens
            string cleanedHostname = System.Text.RegularExpressions.Regex.Replace(hostname, @"[^a-z0-9-]", "");

            // Ensure the length doesn't exceed 63 characters (hostname length limit)
            if (cleanedHostname.Length > 63)
            {
                cleanedHostname = cleanedHostname.Substring(0, 63);
            }

            return cleanedHostname;
        }
    }
}

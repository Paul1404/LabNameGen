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

        // Handle hypervisor selection (simplified without visibility toggling)
        private void HypervisorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // No need to hide or show elements anymore, so this is simplified
            string hypervisor = GetComboBoxValue(HypervisorComboBox);
        }

        // Event handler for Baremetal checkbox checked
        private void BaremetalCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Optionally disable other inputs when baremetal is selected
            OsComboBox.IsEnabled = true;
            HypervisorComboBox.IsEnabled = false;
            VmwareProductComboBox.IsEnabled = false;
        }

        // Event handler for Baremetal checkbox unchecked
        private void BaremetalCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Re-enable inputs when baremetal is unchecked
            OsComboBox.IsEnabled = true;
            HypervisorComboBox.IsEnabled = true;
            VmwareProductComboBox.IsEnabled = true;
        }

        // Generate Hostname and Validate Inputs
        private void GenerateHostname_Click(object sender, RoutedEventArgs e)
        {
            string ip = GetLastTwoOctets(IpTextBox.Text);
            string location = GetLocationAbbreviation();

            if (string.IsNullOrEmpty(ip))
            {
                ResultTextBox.Text = "IP Address is required.";
                return;
            }

            if (!IsValidIp(IpTextBox.Text))
            {
                ResultTextBox.Text = "Invalid IP Address format.";
                return;
            }

            string hostname = BuildHostname(ip, location);
            hostname = CleanHostname(hostname);
            ResultTextBox.Text = hostname;
        }

        // Build the hostname based on selected options
        private string BuildHostname(string ip, string location)
        {
            // Check if a network device is selected
            if (IsNetworkDeviceSelected(out string networkDevice))
            {
                return $"{location}-{networkDevice.ToLower()}-{ip}";
            }

            // Handle Baremetal option
            if (BaremetalCheckBox.IsChecked == true)
            {
                string osAbbreviationInBaremetal = GetOsAbbreviation(GetComboBoxValue(OsComboBox));
                return $"{location}-baremetal-{osAbbreviationInBaremetal}-{ip}".ToLower();
            }

            // Handle VMware product cases
            string vmwareProduct = GetComboBoxValue(VmwareProductComboBox);
            if (vmwareProduct == "vCenter Appliance" || vmwareProduct == "NSX" || vmwareProduct == "vSAN" || vmwareProduct == "ESXi")
            {
                return $"{location}-{GetVmwareProductAbbreviation(vmwareProduct)}-{ip}";
            }

            // General case for OS and Hypervisor
            string hypervisorAbbreviationInGeneral = GetHypervisorAbbreviation(GetComboBoxValue(HypervisorComboBox));
            string osAbbreviationInGeneral = GetOsAbbreviation(GetComboBoxValue(OsComboBox));

            return $"{location}-{hypervisorAbbreviationInGeneral}-{osAbbreviationInGeneral}-{ip}".ToLower();
        }

        // Helper method to handle network device selection
        private bool IsNetworkDeviceSelected(out string networkDevice)
        {
            networkDevice = GetComboBoxValue(NetworkDeviceComboBox);
            return !string.IsNullOrEmpty(networkDevice);
        }

        // Get Location Abbreviation
        private string GetLocationAbbreviation() => GetComboBoxValue(LocationComboBox).ToLower() switch
        {
            var loc when loc.Contains("hldca") => "HLDCA",
            var loc when loc.Contains("cdcb") => "CDCB",
            _ => "LOC"  // Default if no match found
        };

        // Extract the last two octets from the IP address
        private string GetLastTwoOctets(string ip)
        {
            string[] octets = ip.Split('.');
            return octets.Length == 4 ? $"{octets[2]}{octets[3]}" : string.Empty;
        }

        // Abbreviation for OS
        private string GetOsAbbreviation(string os) => os switch
        {
            "RHEL" => "rhel",
            "Ubuntu" => "ubnt",
            "Debian" => "deb",
            "Windows Server Core" => "wsc",
            "Windows Server GUI" => "wsg",
            _ => "os"
        };

        // Abbreviation for Hypervisor
        private string GetHypervisorAbbreviation(string hypervisor) => hypervisor switch
        {
            "VMware" => "vmw",
            "Hyper-V" => "hv",
            "Proxmox" => "pmx",
            _ => "hyp"
        };

        // Abbreviation for VMware Products
        private string GetVmwareProductAbbreviation(string product) => product switch
        {
            "vCenter Appliance" => "vca",
            "ESXi" => "esx",
            "NSX" => "nsx",
            "vSAN" => "vsan",
            "Horizon" => "hor",
            _ => "vmwprod"
        };

        // Validate IP Address format
        private bool IsValidIp(string ip) => IPAddress.TryParse(ip, out _);

        // Clean hostname to remove any special characters and enforce hostname best practices
        private string CleanHostname(string hostname)
        {
            string cleaned = System.Text.RegularExpressions.Regex.Replace(hostname.ToLower(), @"[^a-z0-9-]", "");
            return cleaned.Length > 63 ? cleaned.Substring(0, 63) : cleaned;
        }

        // Copy hostname silently to clipboard
        private void CopyHostname_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ResultTextBox.Text))
            {
                Clipboard.SetText(ResultTextBox.Text);
            }
        }

        // Clear all fields
        private void ClearFields_Click(object sender, RoutedEventArgs e)
        {
            IpTextBox.Clear();
            LocationComboBox.SelectedIndex = -1;
            OsComboBox.SelectedIndex = -1;
            HypervisorComboBox.SelectedIndex = -1;
            VmwareProductComboBox.SelectedIndex = -1;
            ResultTextBox.Text = "";
        }

        // Helper to get the selected value from ComboBox
        private string GetComboBoxValue(ComboBox comboBox) => (comboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
    }
}

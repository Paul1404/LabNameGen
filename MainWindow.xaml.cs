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

        // Handle hypervisor selection and show VMware products if VMware is selected
        private void HypervisorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string hypervisor = GetComboBoxValue(HypervisorComboBox);

            if (hypervisor == "VMware")
            {
                SetVmwareProductVisibility(true);
                VmwareProductComboBox_SelectionChanged(); // Handle initial state
            }
            else
            {
                SetVmwareProductVisibility(false);
                SetOsHypervisorVisibility(true);  // Replacing the undefined method
            }
        }

        // Handle VMware Product selection and visibility for OS and hypervisor fields
        private void VmwareProductComboBox_SelectionChanged()
        {
            string selectedProduct = GetComboBoxValue(VmwareProductComboBox);
            bool hideOsAndHypervisor = selectedProduct == "vCenter Appliance" || selectedProduct == "NSX" || selectedProduct == "vSAN";

            SetOsHypervisorVisibility(!hideOsAndHypervisor);
        }

        // Generate Hostname and Validate Inputs
        private void GenerateHostname_Click(object sender, RoutedEventArgs e)
        {
            string ip = GetLastTwoOctets(IpTextBox.Text);
            string location = GetLocationAbbreviation();

            if (string.IsNullOrEmpty(ip))
            {
                DisplayError("IP Address is required.");
                return;
            }

            if (!IsValidIp(IpTextBox.Text))
            {
                DisplayError("Invalid IP Address format.");
                return;
            }

            string hostname = BuildHostname(ip, location);
            hostname = CleanHostname(hostname);

            // Set the generated hostname in the output TextBox
            ResultTextBox.Text = hostname;
        }

        // Build the hostname based on selected options
        private string BuildHostname(string ip, string location)
        {
            if (IsNetworkDeviceSelected(out string networkDevice))
            {
                return $"{location}-{networkDevice.ToLower()}-{ip}";
            }

            if (BaremetalCheckBox.IsChecked == true)
            {
                string osAbbreviation = GetOsAbbreviation(GetComboBoxValue(OsComboBox));  // Renamed to osAbbreviation
                return $"{location}-baremetal-{osAbbreviation}-{ip}".ToLower();
            }

            string vmwareProduct = GetComboBoxValue(VmwareProductComboBox);
            if (vmwareProduct == "vCenter Appliance" || vmwareProduct == "NSX" || vmwareProduct == "vSAN")
            {
                string productAbbr = GetVmwareProductAbbreviation(vmwareProduct);
                return $"{location}-{productAbbr}-{ip}";
            }

            string hypervisor = GetComboBoxValue(HypervisorComboBox);
            string os = GetComboBoxValue(OsComboBox);
            string osAbbr = GetOsAbbreviation(os);
            string hypervisorAbbr = GetHypervisorAbbreviation(hypervisor);

            return $"{location}-{hypervisorAbbr}-{osAbbr}-{ip}".ToLower();
        }

        // Helper method to handle network device selection
        private bool IsNetworkDeviceSelected(out string networkDevice)
        {
            networkDevice = GetComboBoxValue(NetworkDeviceComboBox);
            return !string.IsNullOrEmpty(networkDevice);
        }

        // Helper method to display error messages
        private void DisplayError(string message)
        {
            ResultTextBox.Text = message;
        }

        // Get Location Abbreviation
        private string GetLocationAbbreviation()
        {
            string location = GetComboBoxValue(LocationComboBox).ToLower();
            if (location.Contains("hldca")) return "HLDCA";
            if (location.Contains("cdcb")) return "CDCB";
            return "LOC"; // Default if no match found
        }

        // Extract the last two octets from the IP address
        private string GetLastTwoOctets(string ip)
        {
            string[] octets = ip.Split('.');
            return octets.Length == 4 ? $"{octets[2]}{octets[3]}" : string.Empty;
        }

        // Abbreviation for OS
        private string GetOsAbbreviation(string os)
        {
            return os switch
            {
                "RHEL" => "rhel",
                "Ubuntu" => "ubnt",
                "Debian" => "deb",
                "Windows Server Core" => "wsc",
                "Windows Server GUI" => "wsg",
                _ => "os"
            };
        }

        // Abbreviation for Hypervisor
        private string GetHypervisorAbbreviation(string hypervisor)
        {
            return hypervisor switch
            {
                "VMware" => "vmw",
                "Hyper-V" => "hv",
                "Proxmox" => "pmx",
                _ => "hyp"
            };
        }

        // Abbreviation for VMware Products
        private string GetVmwareProductAbbreviation(string product)
        {
            return product switch
            {
                "vCenter Appliance" => "vca",
                "ESXi" => "esx",
                "NSX" => "nsx",
                "vSAN" => "vsan",
                "Horizon" => "hor",
                _ => "vmwprod"
            };
        }

        // Handle Baremetal Checkbox selection
        private void BaremetalCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetHypervisorVisibility(false);
        }

        private void BaremetalCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetHypervisorVisibility(true);
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
            SetVmwareProductVisibility(false);
            SetOsHypervisorVisibility(true);
            ResultTextBox.Text = "";
        }

        // Validate IP Address format
        private bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }

        // Clean hostname to remove any special characters and enforce hostname best practices
        private string CleanHostname(string hostname)
        {
            string cleanedHostname = System.Text.RegularExpressions.Regex.Replace(hostname.ToLower(), @"[^a-z0-9-]", "");
            return cleanedHostname.Length > 63 ? cleanedHostname.Substring(0, 63) : cleanedHostname;
        }

        // Helper to set VMware product visibility
        private void SetVmwareProductVisibility(bool visible)
        {
            VmwareProductComboBox.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            VmwareLabel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        // Helper to set OS and Hypervisor visibility
        private void SetOsHypervisorVisibility(bool visible)
        {
            OsComboBox.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            OsLabel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            HypervisorComboBox.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            HypervisorLabel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        // Helper to set Hypervisor visibility
        private void SetHypervisorVisibility(bool visible)
        {
            HypervisorComboBox.IsEnabled = visible;
            HypervisorLabel.IsEnabled = visible;
        }

        // Helper to get the selected value from ComboBox
        private string GetComboBoxValue(ComboBox comboBox)
        {
            return (comboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
        }
    }
}

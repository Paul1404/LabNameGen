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
        private void HypervisorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string hypervisor = (HypervisorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (hypervisor == "VMware")
            {
                VmwareProductComboBox.Visibility = Visibility.Visible;
                VmwareLabel.Visibility = Visibility.Visible;

                // If a specific product is selected, hide OS and Hypervisor selection
                VmwareProductComboBox.SelectionChanged += (s, ev) =>
                {
                    string selectedProduct = (VmwareProductComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    if (selectedProduct == "vCenter Appliance" || selectedProduct == "NSX" || selectedProduct == "vSAN")
                    {
                        OsComboBox.Visibility = Visibility.Collapsed;
                        OsLabel.Visibility = Visibility.Collapsed;
                        HypervisorComboBox.Visibility = Visibility.Collapsed;
                        HypervisorLabel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        OsComboBox.Visibility = Visibility.Visible;
                        OsLabel.Visibility = Visibility.Visible;
                        HypervisorComboBox.Visibility = Visibility.Visible;
                        HypervisorLabel.Visibility = Visibility.Visible;
                    }
                };
            }
            else
            {
                VmwareProductComboBox.Visibility = Visibility.Collapsed;
                VmwareLabel.Visibility = Visibility.Collapsed;
            }
        }

        // Generate Hostname and Validate Inputs
        private void GenerateHostname_Click(object sender, RoutedEventArgs e)
        {
            string ip = GetLastTwoOctets(IpTextBox.Text);

            // Get the location abbreviation
            string location = GetLocationAbbreviation();

            string os = (OsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string hypervisor = (HypervisorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string vmwareProduct = (VmwareProductComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(ip))
            {
                ResultLabel.Text = "IP Address is required.";
                return;
            }

            if (!IsValidIp(IpTextBox.Text))
            {
                ResultLabel.Text = "Invalid IP Address format.";
                return;
            }

            string hostname;

            // If certain VMware products are selected, skip OS and Hypervisor
            if (vmwareProduct == "vCenter Appliance" || vmwareProduct == "NSX" || vmwareProduct == "vSAN")
            {
                string productAbbr = GetVmwareProductAbbreviation(vmwareProduct);
                hostname = $"{location}-{productAbbr}-{ip}";
            }
            else
            {
                string osAbbr = GetOsAbbreviation(os);
                string hypervisorAbbr = GetHypervisorAbbreviation(hypervisor);
                hostname = $"{location}-{hypervisorAbbr}-{osAbbr}-{ip}".ToLower();
            }

            hostname = CleanHostname(hostname);
            ResultLabel.Text = hostname;
        }




        // Get Location Abbreviation
        private string GetLocationAbbreviation()
        {
            if (LocationComboBox.SelectedItem == null)
            {
                return "LOC"; // Default if no location is selected
            }

            // Convert location to lowercase for case-insensitive comparison
            string location = LocationComboBox.SelectedItem.ToString().ToLower();
            if (location.Contains("hldca"))
            {
                return "HLDCA";
            }
            else if (location.Contains("cdcb"))
            {
                return "CDCB";
            }

            return "LOC"; // Default if no match found
        }





        // Extract the last two octets from the IP address
        private string GetLastTwoOctets(string ip)
        {
            string[] octets = ip.Split('.');
            if (octets.Length == 4)
            {
                return $"{octets[2]}{octets[3]}";
            }
            return string.Empty;
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
            LocationComboBox.SelectedIndex = -1;
            OsComboBox.SelectedIndex = -1;
            HypervisorComboBox.SelectedIndex = -1;
            VmwareProductComboBox.SelectedIndex = -1;
            VmwareProductComboBox.Visibility = Visibility.Collapsed;
            VmwareLabel.Visibility = Visibility.Collapsed;
            OsComboBox.Visibility = Visibility.Visible;
            OsLabel.Visibility = Visibility.Visible;
            HypervisorComboBox.Visibility = Visibility.Visible;
            HypervisorLabel.Visibility = Visibility.Visible;
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
            // Convert the hostname to lowercase
            string cleanedHostname = hostname.ToLower();

            // Remove any non-alphanumeric characters (except hyphen)
            cleanedHostname = System.Text.RegularExpressions.Regex.Replace(cleanedHostname, @"[^a-z0-9-]", "");

            // Ensure the hostname doesn't exceed 63 characters (hostname length limit)
            if (cleanedHostname.Length > 63)
            {
                cleanedHostname = cleanedHostname.Substring(0, 63);
            }

            return cleanedHostname;
        }

    }
}

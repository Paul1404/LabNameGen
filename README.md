# LabNameGen: Dynamic Homelab Hostname Generator

![LabNameGen](https://img.shields.io/badge/Version-1.0.2-blue.svg)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)
![WPF](https://img.shields.io/badge/WPF-C%23-blueviolet.svg)

## 📚 Overview

**LabNameGen** is a flexible and dynamic hostname generator tailored for homelab environments, built with WPF in C#. It simplifies the process of generating consistent, professional hostnames based on customizable inputs such as location, operating system, hypervisor, and VMware products. The tool is designed for homelab enthusiasts, system administrators, and IT professionals who want to maintain a clean, organized naming convention across their infrastructure.

---

## 🚀 Features

- **IP-Based Hostname Generation**: Automatically extracts and uses the last two octets of the IP address.
- **Location Awareness**: Supports multiple predefined locations such as `HLDCA` (Homelab DC A) and `CDCB` (Contabo VPS).
- **Dynamic Input Handling**: Automatically adjusts available input fields based on selected VMware products.
- **Clean, Consistent Hostnames**: Generates hostnames following best practices (lowercase, alphanumeric with hyphen).
- **Custom VMware Product Abbreviations**: Includes support for products like vCenter, ESXi, NSX, vSAN, and more.
- **Easy-to-Use UI**: Modern, responsive WPF interface that’s intuitive and efficient.

---

## 🎨 Example Hostname Formats

| VMware Product       | Location | Example Output          |
|----------------------|----------|-------------------------|
| vCenter Appliance    | HLDCA    | `hldca-vca-17820`       |
| ESXi                 | CDCB     | `cdcb-esx-17820`        |
| Ubuntu + VMware      | HLDCA    | `hldca-vmw-ubnt-17820`  |
| Windows Server Core  | CDCB     | `cdcb-vmw-wsc-17820`    |

---

## 🛠 Installation & Usage

### Prerequisites
- **Windows OS**
- **.NET Core SDK** (or any recent .NET runtime for C#)
- **Visual Studio** or any C# IDE

### Installation

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/Paul1404/LabNameGen.git
   cd labnamegen
   ```

2. **Open the Project in Visual Studio:**
    
    * Navigate to the project folder and open the `.csproj` file with Visual Studio or your preferred C# IDE.
3. **Build and Run:**
    
    * Simply build and run the application within your IDE.

### Usage

1. **Enter IP Address**: Input the device's IP address (e.g., `192.168.178.20`).
2. **Select Location**: Choose the location from the dropdown (e.g., `HLDCA`, `CDCB`).
3. **Choose VMware Product or OS + Hypervisor**: Select a VMware product (e.g., vCenter, ESXi) or configure your OS and hypervisor manually.
4. **Generate**: Click `Generate Hostname` to see your dynamically generated hostname.
5. **Copy**: Use the `Copy` button to copy the hostname to the clipboard for quick usage.
6. **Clear**: Reset the form with the `Clear` button.

* * *

🧩 How It Works
---------------

The application dynamically adjusts based on user input. For example:

* If a VMware product like **vCenter Appliance** is selected, OS and Hypervisor fields are hidden to avoid redundancy.
    
* Hostnames are generated in the format:
    
    ```php-template
    <location>-<product/hypervisor>-<os>-<last-two-octets>
    ```
    

The `CleanHostname()` function ensures all hostnames follow best practices (lowercase, limited special characters, length ≤ 63 characters).

* * *

🤝 Contributing
---------------

We welcome contributions to **LabNameGen**! If you would like to contribute, please follow these steps:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Make your changes and commit them (`git commit -m 'Add new feature'`).
4. Push the changes (`git push origin feature-branch`).
5. Open a pull request.

* * *


📜 License
----------

This project is licensed under the MIT License - see the LICENSE file for details.

* * *

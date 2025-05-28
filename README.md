# Aether

**Aether** is a powerful Android tool built on Frida for mobile game modding, APK management, and automation.

---

## Features

- Install APKs directly to Android devices  
- Manage and select Frida gadget versions and architectures  
- Inject custom scripts using Frida for advanced modding  
- Check environment readiness before injection  
- Simple, clean, and efficient user interface  
- Designed for developers and modders working on unity mobile game reverse engineering  

---

## Getting Started

### Prerequisites

- Windows OS  
- Android device with USB debugging enabled  
- .NET 9 (or higher) runtime installed
- Java 8 or higher

### Installation

1. Download the latest release from the [Releases](https://github.com/sflashy/aether/releases) page  
2. Run the installer and follow the instructions  
3. Connect your Android device via USB  

---

## Usage

1. Download the game in `.apx` format from the APKPure website and extract its contents to a folder. Then select that folder as the workspace.
2. Connect an Android device with USB debugging enabled. 
3. Choose the Frida gadget version and architecture  
4. Choose one of the injection modes:
    - **Debug Mode (Listening Only)**: Starts the Frida gadget in passive listening mode using the `frida-gadget-config.so` file, without injecting any agent script.
   - **Persistent Injection**: Injects the selected JavaScript agent into the application immediately when Frida gadget starts, enabling active instrumentation.
5. Inject your script using the interface  
6. Monitor the output and logs within the application  

---

## Screenshots

![Aether](/Images/aether.png)

## Contributing

Contributions are welcome! Please open issues and pull requests on the [GitHub repository](https://github.com/sflashy/aether).

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Contact

For questions or support, contact aether@sflashy.com

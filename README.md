# 🥚 Tattletail Multiplayer Mod

![Status](https://img.shields.io/badge/Status-Work--In--Progress-orange?style=for-the-badge)
![License](https://img.shields.io/badge/License-GPLv3-blue?style=for-the-badge)

> [!CAUTION]
> ### ⚠️ Development Build
> This mod is currently in **Active Development**. It is not a finished product. Expect bugs, crashes, and sync issues. This repository is shared for testing and collaborative purposes.

---

## 👤 Authorship & Credits
* **Lead Developer:** `VZP`
* **Original Game:** `Waygetter Electronics`

> [!IMPORTANT]
> Any redistribution of this code must maintain original authorship credit and be released under the same **GPLv3** license.

---

## 📥 Installation Guide
Follow these steps carefully. The game requires a very specific version of the mod loader to function.

### 1. Requirements
* **BepInEx 5 (Win32 / x86 version SPECIFICALLY)**
  * *Note: The x64 version will NOT work with Tattletail.*
* **Tattletail** (Steam version recommended)

### 2. Installing BepInEx
1. Download the **BepInEx 5 Win32** zip file.
2. Open your Tattletail game directory (where `tattletailWindows.exe` is located).
3. Drag and drop **all files** from the BepInEx zip into that folder.
4. **Launch the game once** and close it. This generates the necessary folders.

### 3. Installing the Mod
1. Download the `TattleTailMultiplayer.dll` from the **Releases** tab.
2. Navigate to `Tattletail/BepInEx/plugins`.
3. Drop the `.dll` file into the **plugins** folder.
4. Launch the game!

---

## 🏗 Developing & Contributing
If you are a developer looking to help with networking or bug fixes:

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git](https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git)
   ```
2. **Setup Dependencies:** Create a folder named libs in the project root. Copy the following DLLs from your game's tattletailWindows_Data/Managed folder into your new libs folder:
```
UnityEngine.dll
UnityEngine.UI.dll
Assembly-CSharp.dll
Assembly-CSharp-firstpass.dll
```
3. **Open Project:** Open the ```.sln``` file in Visual Studio 2022.

## 📞 Support
If you find a bug, please open an Issue on this GitHub repository with a copy of your LogOutput.log from the BepInEx folder.

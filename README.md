<p align="center">
  <img src="https://i.imgur.com/w9A0rh2.png" alt="Library Management System Mockup Image" style="max-width: 100%; height: auto;" />
</p>


### [!] Notice
- You may not commit to this project. If you find something bothering you, please open an issue first! 👍
- You may run unit tests (made with xUnit) by using the built-in Visual Studio tests explorer.

# 📚 Library Management System (.NET)
<p align="center">
  <a href="https://github.com/lates-codrin/Library-Management/releases/download/v0.0.1-beta/Setup.pdf">
    <img src="https://img.shields.io/badge/📄%20Download-Documentation%20PDF-green?style=for-the-badge" alt="Download Documentation PDF"/>
  </a>
  <a href="https://github.com/lates-codrin/Library-Management/releases/download/v0.0.1-beta/x64.zip">
    <img src="https://img.shields.io/badge/📦%20Download-Pre--Release-orange?style=for-the-badge" alt="Download Release"/>
  </a>
</p>


A desktop-based Library Management System built in C# with a multi-layered architecture. This project is developed as part of the Internship 2025 and aims to provide a user-friendly way for library admins to manage book inventories, issue and return books, and receive book recommendations using AI.

---


# 🚀 How to Run the Project

## Prerequisites
1. Install Visual Studio 2022 or newer.
2. Install .NET SDK 8.0 or higher (or when installing Visual Studio, choose ".NET Desktop Development")
   If you don't have the SDK installed, Visual Studio will automatically prompt you to install ".NET Desktop Development". Click Install.


## 🔹 Method 1: Using Visual Studio (Recommended)
1. Clone the Repository
2. Open a terminal or Git Bash and run:

```bash
git clone https://github.com/lates-codrin/Library-Management.git
```

3. Open the Solution File (*Library_Management_System.sln*)

4. Restore Dependencies & Build
Visual Studio will automatically restore NuGet packages. Then build the project using:
Build > Build Solution or Ctrl + Shift + B

5. Run the Application
Click the green Start button or press F5.

---

Method 2: Download the source code
1. Download the project from GitHub:
<p align="center">
  <img src="https://i.imgur.com/SdtVaA3.png" alt="Download Image" style="max-width: 100%; height: auto;" />
</p>


2. Open the .zip > first folder > .sln solution
3. Build & run using the built-in Visual Studio tools

---

Method 3: Download the release

1. Download the release (x64.zip) [here](https://github.com/lates-codrin/Library-Management/releases/tag/v0.0.1-alpha).
2. Open the .zip > folder > .exe
3. The application should start as usual


## HOW TO RUN SPECIAL FEATURE:
To enjoy the AI-powered book recommendation feature, you'll need an OpenAI API key. You have two simple options:

## 🔹 **Option 1**: Use the Built-in Key (Default)
No action required — a preloaded OpenAI key with a $5 credit limit (already covered) is integrated directly into the application. Just launch and enjoy!

## 🔹 **Option 2**: Use Your Own Key
Prefer using your personal OpenAI account? Follow these steps:

1. Sign in to the OpenAI platform: https://platform.openai.com/api-keys
2. Generate an API key from your account dashboard.
3. Purchase $5 in credits to enable usage.
4. Copy your key and store it securely.
5. Open the app, navigate to the settings, paste your key, click Save, then restart the application.

---

# Documentation Website (DocFX)

## Prerequisites

Familiarity with the command line
Install .NET SDK 8.0 or higher
Install Node.js v20 or higher (Optional: It's required when using Create PDF Files)

Make sure you have .NET SDK installed, then open a terminal and enter the following command to install the latest docfx:
```bash
dotnet tool update -g docfx
```

1. Open a terminal
2. Run 
```bash
docfx --serve
```
3. Restart PC
4. Open the link provided in the console by docfx




## 🛠️ Technologies Used

- **.NET (C# WPF)**
- **MVVM Pattern** (via [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/overview))
- **Local JSON Storage**
- **OpenAI GPT API Integration** (for book recommendations)
- **WPF UI** (via [WPF-UI](https://wpfui.lepo.co/))
  
<p align="center">
  <img src="https://i.imgur.com/oLldy3v.png" alt="Schema" style="max-width: 100%; height: auto;" />
</p>

---

💡 **Custom Feature Description: AI Book Recommender**
This unique feature allows the administrator to input:

* A book title
* Its category
* A brief synopsis

The system then uses the OpenAI GPT model to suggest a relevant and potentially interesting book. This serves as a smart assistant to help users discover new reads aligned with their interests or library offerings.



# 📄 **Notes**
* All data is persisted across sessions using JSON files stored locally.
* The UI is built using WPF (and WPFUI) with a clean and modern design.



# 📬 **Contact**
For any questions or feedback, feel free to contact:
Lates Codrin-Gabriel – latescodrin@gmail.com


# 📝 **Credits**
* **WPF UI Library**: See here https://wpfui.lepo.co/

* Apple Design Resources: Mock images used in the design were sourced from Apple's official design resources. Mock designed with https://mockuphone.com/.

* Third-Party Libraries: All third-party libraries used in this project are licensed under the MIT License.

# 🏁 Final Thoughts
This project demonstrates basic skills in **.NET development**, **WPF UI design**, the use of external libraries like **CommunityToolkit.Mvvm**, and a solid understanding of **multi-layered architecture** for clean separation of concerns and scalability.

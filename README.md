<p align="center">
  <img src="https://i.imgur.com/cftZsqd.png" width="800" height="500" alt="Library Management System Mockup Image"/>
</p>

### [!] Notice
- You may not commit to this project. If you find something bothering you, please open an issue first! 👍

# 📚 Library Management System (.NET)
<p align="center">
  <a href="https://github.com/lates-codrin/Library-Management-System/archive/refs/tags/1.0.0-alpha.zip">
    <img src="https://img.shields.io/badge/⬇️%20Download-Source%20Code-blue?style=for-the-badge" alt="Download Source Code"/>
  </a>
  <a href="https://github.com/lates-codrin/Library-Management-System/releases/download/1.0.0-alpha/PreRelease.zip">
    <img src="https://img.shields.io/badge/📦%20Download-Pre--Release-orange?style=for-the-badge" alt="Download Pre-release"/>
  </a>
</p>

A desktop-based Library Management System built in C# with a multi-layered architecture. This project is developed as part of the Internship 2025 and aims to provide a user-friendly way for library admins to manage book inventories, issue and return books, and receive book recommendations using AI.

---

## ✨ Features

### ✅ Core Functionalities
- **Book Management (CRUD):**
  - Add, update, delete, and view books.
    
- **Search & Filter:**
  - Search books by title, author, or status.
    
- **Lending Process:**
  - Issue and return books with automated stock updates.
  - Status-based logic to prevent lending when out of stock and handle returns correctly.
    
- **Persistent Storage:**
  - Book and lending records stored locally using **JSON files**.

### 🧠 Extra Feature – AI Book Recommender (Powered by GPT)
- 📘 **Input:** Book title, genre/category, and synopsis.
- 🤖 **Output:** Personalized book recommendations generated using GPT.
- A creative extension designed to elevate the library experience with intelligent suggestions.



## 🛠️ Technologies Used

- **.NET (C# WPF)**
- **MVVM Pattern** (via [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/overview))
- **Local JSON Storage**
- **OpenAI GPT API Integration** (for book recommendations)
- **WPF UI** (via [WPF-UI](https://wpfui.lepo.co/))
  


## 🚀 How to Run the Project

### Prerequisites

- Visual Studio 2022+
- .NET 6.0 SDK or later


### Steps

1. **Clone the Repository**  
   ```bash
   git clone https://github.com/lates-codrin/library-management-system.git
   ```

1. **Open the Solution in Visual Studio**
Open Library_Management_System.sln.

2. **Build the Project**
Press Ctrl+Shift+B or use the Build menu.

3. **Run the Application**
Press F5 or click the green Start button.

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

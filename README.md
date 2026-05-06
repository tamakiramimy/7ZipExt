# 7ZipExt

[中文](#中文说明) | [English](#english)

## 中文说明

7ZipExt 是一个基于 Avalonia 的轻量桌面工具，界面风格参考 7-Zip，支持压缩、解压、排除项配置，以及 Windows 资源管理器右键菜单集成。

### 功能特性

- 支持文件和文件夹压缩
- 支持独立解压窗口
- 支持排除文件、文件夹和通配符模式
- 根据源文件或文件夹自动生成压缩包名称
- 支持分卷、加密、更新方式等高级选项
- 支持注册和卸载 Windows 右键二级菜单

### 环境要求

- Windows：用于右键菜单注册
- .NET 10 SDK：用于本地构建
- 7-Zip：默认路径为 `C:\Program Files\7-Zip\7z.exe`

### 本地构建

```powershell
dotnet build .\7ZipExt.csproj -c Debug
```

### 运行方式

启动主程序：

```powershell
dotnet run --project .\7ZipExt.csproj
```

直接打开压缩窗口：

```powershell
dotnet .\bin\Debug\net10.0-windows\7ZipExt.dll compress "D:\SomeFolder"
```

直接打开解压窗口：

```powershell
dotnet .\bin\Debug\net10.0-windows\7ZipExt.dll extract "D:\SomeArchive.zip"
```

### 右键菜单注册

注册：

```powershell
dotnet .\bin\Debug\net10.0-windows\7ZipExt.dll register
```

卸载：

```powershell
dotnet .\bin\Debug\net10.0-windows\7ZipExt.dll unregister
```

### 界面预览

右键菜单：

![右键菜单](Assets/1.%E5%8F%B3%E9%94%AE%E8%8F%9C%E5%8D%95.png)

注册界面：

![注册界面](Assets/2.%E6%B3%A8%E5%86%8C.png)

压缩界面：

![压缩界面](Assets/3.%E5%8E%8B%E7%BC%A9.png)

### GitHub Actions / Release

仓库包含 GitHub Actions 工作流，支持：

- Push 和 Pull Request 自动构建
- Windows Release 构建
- Tag 发布时自动创建 GitHub Release 并上传构建产物

推荐使用 tag 触发发布，例如：

```powershell
git tag v1.0.0
git push origin v1.0.0
```

---

## English

7ZipExt is a lightweight Avalonia desktop utility inspired by the 7-Zip UI. It supports compression, extraction, exclusion rules, and Windows Explorer context-menu integration.

### Features

- Compress files and folders
- Extract archives with a dedicated window
- Exclude files, folders, and wildcard patterns
- Auto-generate archive names from the selected source item
- Support volume splitting, encryption, and update modes
- Register or remove a Windows Explorer submenu

### Requirements

- Windows for context-menu registration
- .NET 10 SDK for local builds
- 7-Zip installed, default path: `C:\Program Files\7-Zip\7z.exe`

### Build

```powershell
dotnet build .\7ZipExt.csproj -c Debug
```

### Run

Start the main application:

```powershell
dotnet run --project .\7ZipExt.csproj
```

Open the compression window directly:

```powershell
dotnet .\bin\Debug\net10.0-windows\7ZipExt.dll compress "D:\SomeFolder"
```

Open the extraction window directly:

```powershell
dotnet .\bin\Debug\net10.0-windows\7ZipExt.dll extract "D:\SomeArchive.zip"
```

### Context Menu Registration

Register the submenu:

```powershell
dotnet .\bin\Debug\net10.0-windows\7ZipExt.dll register
```

Remove the submenu:

```powershell
dotnet .\bin\Debug\net10.0-windows\7ZipExt.dll unregister
```

### Screenshots

Context menu:

![Context menu](Assets/1.%E5%8F%B3%E9%94%AE%E8%8F%9C%E5%8D%95.png)

Registration window:

![Registration window](Assets/2.%E6%B3%A8%E5%86%8C.png)

Compression window:

![Compression window](Assets/3.%E5%8E%8B%E7%BC%A9.png)

### GitHub Actions / Release

The repository includes a GitHub Actions workflow that supports:

- Automatic builds on push and pull requests
- Windows release builds
- Automatic GitHub Release creation and asset upload when a tag is pushed

Recommended release flow:

```powershell
git tag v1.0.0
git push origin v1.0.0
```
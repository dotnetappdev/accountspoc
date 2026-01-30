# Build and Deployment Instructions

Complete guide for building and deploying the Accounts POC system across all platforms.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Backend API Setup](#backend-api-setup)
3. [Blazor Web App Setup](#blazor-web-app-setup)
4. [React Native Contractor App Setup](#react-native-contractor-app-setup)
5. [Platform-Specific Build Instructions](#platform-specific-build-instructions)
6. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### For Backend (.NET)

- **.NET 10 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/10.0)
- **IDE** (Choose one):
  - Visual Studio 2022 (Windows/Mac)
  - Visual Studio Code with C# extension
  - JetBrains Rider

### For React Native App

- **Node.js** (v18 or later) - [Download here](https://nodejs.org/)
- **npm** or **yarn** package manager (comes with Node.js)
- **Expo CLI** - Install globally: `npm install -g expo-cli`

### Platform-Specific Requirements

#### Windows
- Windows 10/11
- For Android: Android Studio with Android SDK
- For iOS: Not supported directly (use Mac or Expo Go app)

#### macOS
- macOS 12 or later
- Xcode 14+ (for iOS builds)
- CocoaPods: `sudo gem install cocoapods`
- For Android: Android Studio with Android SDK

#### Linux
- Ubuntu 20.04+ or equivalent
- For Android: Android Studio with Android SDK
- libsecret-1-dev: `sudo apt-get install libsecret-1-dev`

#### iOS (requires macOS)
- Xcode 14 or later
- Xcode Command Line Tools: `xcode-select --install`
- CocoaPods: `sudo gem install cocoapods`
- Apple Developer Account (for device testing/deployment)

#### Android
- Android Studio
- Android SDK (API 31+)
- Java Development Kit (JDK) 17+
- Android Emulator or physical device with USB debugging

---

## Backend API Setup

### 1. Navigate to API Directory
```bash
cd src/AccountsPOC.WebAPI
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Project
```bash
dotnet build
```

### 4. Run Database Migrations
```bash
dotnet ef database update --project ../AccountsPOC.Infrastructure
```

### 5. Run the API
```bash
dotnet run
```

The API will start at `http://localhost:5001` by default.

### Configuration

Edit `appsettings.json` to configure:
- Database connection string
- CORS origins
- Logging levels

---

## Blazor Web App Setup

### 1. Navigate to Blazor Directory
```bash
cd src/AccountsPOC.BlazorApp
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Project
```bash
dotnet build
```

### 4. Run the Application
```bash
dotnet run
```

The Blazor app will start at `http://localhost:5193` by default.

### Configuration

Edit `appsettings.json` to set:
- API base URL (should match your running API)
- Other application settings

---

## React Native Contractor App Setup

### 1. Navigate to App Directory
```bash
cd ContractorApp
```

### 2. Install Dependencies
```bash
npm install
```

### 3. Configure Environment

Copy the example environment file:
```bash
cp .env.example .env
```

Edit `.env` and configure:
```env
API_URL=http://localhost:5001/api
OFFLINE_FIRST=true
WIFI_ONLY_SYNC=true
```

**Note:** For physical devices, replace `localhost` with your computer's IP address:
```env
API_URL=http://192.168.1.100:5001/api
```

### 4. Start Expo Development Server
```bash
npm start
```

This will open Expo Dev Tools in your browser.

---

## Platform-Specific Build Instructions

### iOS (macOS only)

#### Development (Expo Go)
1. Install Expo Go app on your iOS device from App Store
2. Run `npm start` in ContractorApp directory
3. Scan QR code with Camera app
4. App opens in Expo Go

#### Development (iOS Simulator)
```bash
npm run ios
```

This will:
- Install iOS Simulator if needed
- Build and launch the app in simulator

#### Production Build
```bash
# Install EAS CLI
npm install -g eas-cli

# Login to Expo account
eas login

# Configure project
eas build:configure

# Build for iOS
eas build --platform ios
```

### Android

#### Development (Expo Go)
1. Install Expo Go app on your Android device from Play Store
2. Run `npm start` in ContractorApp directory
3. Scan QR code with Expo Go app
4. App opens in Expo Go

#### Development (Android Emulator)
```bash
npm run android
```

This will:
- Launch Android emulator if not running
- Build and install the app
- Start the app on emulator

#### Production Build
```bash
# Build APK for Android
eas build --platform android --profile preview

# Build for Play Store
eas build --platform android --profile production
```

### Web

#### Development
```bash
npm run web
```

The app will open in your default browser at `http://localhost:19006`.

#### Production Build
```bash
# Build for web
expo build:web

# Output will be in web-build directory
# Deploy to any static hosting service (Netlify, Vercel, GitHub Pages, etc.)
```

### Windows

#### Development
The web version works on Windows:
```bash
npm run web
```

For a native Windows app, you can use:
```bash
# Install React Native for Windows dependencies
npx react-native-windows-init --overwrite

# Run on Windows
npx react-native run-windows
```

### Linux

#### Development
The web version works on Linux:
```bash
npm run web
```

You can also run Android builds on Linux:
```bash
npm run android
```

---

## Running the Complete System

### Development Environment

1. **Start the API** (Terminal 1):
```bash
cd src/AccountsPOC.WebAPI
dotnet run
```

2. **Start the Blazor App** (Terminal 2):
```bash
cd src/AccountsPOC.BlazorApp
dotnet run
```

3. **Start the React Native App** (Terminal 3):
```bash
cd ContractorApp
npm start
```

Now you have:
- API running on `http://localhost:5001`
- Blazor web app on `http://localhost:5193`
- React Native app accessible via Expo Go or simulators/emulators

---

## Troubleshooting

### Backend Issues

#### Port Already in Use
```bash
# Change port in appsettings.json or use:
dotnet run --urls="http://localhost:5002"
```

#### Database Errors
```bash
# Delete database and recreate
rm AccountsPOC.db
dotnet ef database update --project ../AccountsPOC.Infrastructure
```

### React Native Issues

#### Metro Bundler Issues
```bash
# Clear cache and restart
npm start -- --reset-cache
```

#### iOS Simulator Not Found
```bash
# Install iOS Simulator via Xcode
xcode-select --install
```

#### Android Emulator Not Starting
```bash
# List available emulators
emulator -list-avds

# Start specific emulator
emulator -avd Pixel_5_API_31
```

#### Network Connectivity (Can't reach API)
- For iOS Simulator: Use `http://localhost:5001/api`
- For Android Emulator: Use `http://10.0.2.2:5001/api`
- For Physical Devices: Use your computer's IP address `http://192.168.1.X:5001/api`

#### Dependencies Not Installing
```bash
# Clear npm cache
npm cache clean --force

# Delete node_modules and reinstall
rm -rf node_modules package-lock.json
npm install
```

### CORS Issues

If the React Native app can't connect to the API, check CORS settings in `AccountsPOC.WebAPI/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5193", "http://localhost:19006")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

Add your device IP if needed.

---

## Additional Resources

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
- [Expo Documentation](https://docs.expo.dev/)
- [React Native Documentation](https://reactnative.dev/docs/getting-started)
- [React Navigation](https://reactnavigation.org/docs/getting-started)

---

## Quick Reference

### Common Commands

**Backend:**
```bash
dotnet build          # Build project
dotnet run            # Run project
dotnet test           # Run tests
dotnet clean          # Clean build artifacts
```

**React Native:**
```bash
npm start             # Start Expo dev server
npm run ios           # Run on iOS simulator
npm run android       # Run on Android emulator
npm run web           # Run in web browser
```

### Default Ports

- Web API: `http://localhost:5001`
- Blazor App: `http://localhost:5193`
- React Native Web: `http://localhost:19006`
- Expo Dev Tools: `http://localhost:19002`

---

## Support

For issues or questions:
1. Check the troubleshooting section above
2. Review the respective documentation in `/docs` directory
3. Check the GitHub repository issues

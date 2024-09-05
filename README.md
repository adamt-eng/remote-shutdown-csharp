# Remote Shutdown

## Overview
Remote Shutdown is a Windows Service made in C#, that sets up an HTTP server to listen for shutdown commands over the network. When a valid request is received with the correct shutdown token, the program initiates a system shutdown on the local machine.

## Features
- Remote shutdown functionality over the network.
- Token-based authorization for security.
- Simple HTTP server to listen for shutdown requests.

## Requirements
- Windows OS

## Installation

### 1. **Prerequisites**

- Ensure that you have [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed on your machine:

- You can verify the .NET SDK installation by running:

  ```bash
  dotnet --version
  ```
  ### 2. **Download the Source Code**

- Clone this repository using the following command:

  ```bash
  git clone https://github.com/adamt-eng/Remote-Shutdown
  ```

### 3. **Navigate to the Project Directory**

- After cloning the repository, navigate into the project directory:

  ```bash
  cd Remote-Shutdown
  ```

### 4. **Compile the Source Code**

- Restore dependencies with the following command:

  ```bash
  dotnet restore
  ```

- Once dependencies are restored, compile the project using:

  ```bash
  dotnet build --configuration Release
  ```

### 5. **Run the Application:**

- If the build is successful, you can run the application with:

  ```bash
  dotnet run
  ```
- The first run will create the `config.json` file and instruct you to fill the required data manually.
- After you fill the data in `config.json` and re-run the application, it will start an HTTP listener on the port specified in the file.

## Install as a Windows Service

- You may not like having to run this application every time you start your PC or like keeping a Window open for it, for that you can install this application as a service that runs in the background when your PC starts.

- You may install this application as a service with the following command:

  ```bash
  sc create "Remote Shutdown" binPath= "BIN_PATH" start= auto
  ```
  
- Replace `BIN_PATH` with the path to `Remote Shutdown.exe`

- After installation, you can restart your PC or manually start the service using
  ```bash
  sc start "Remote Shutdown"
  ```

## Configuration
The application uses a `config.json` file for configuration, which includes:
- `PortNumber`: The port on which the HTTP server listens.
- `Token`: The token required to authorize the shutdown request.

Make sure the correct IP address and port are accessible over the network, and that the token is kept secure.

## Usage
1. To initiate a remote shutdown, append `parameters?shutdown=true&token=<your-token>` to the URL that the application says it's listenting to requests at.
  - Replace `<your-token>` with the token you previously specified in the `config.json` file.
2. If the request is valid, the machine will shut down immediately. An invalid request will return an error message.

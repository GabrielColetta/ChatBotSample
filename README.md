## Chatbot sample

This is an sample project that utilizes the .NET ecosystem to create a simple pre trained chatbot using the Deepseek model. 

#### This project is a prototype. All the beautiful animations like preloading,toast and tests was not developed.

### Features
- Web user interface using Angular.
- App user interface using .NET MAUI with Blazor (still experimenting).
- Real-time communication with SignalR.
- Historical data storage with MongoDB.
- .NET Aspire to configure and manage all containers.

## Requirements
To run this project you will need the following prerequisites:

- .NET SDK Version 9.0 or higher.
- Docker for desktop (recommended).
- Node.js: Version 14.x or higher.
- Angular CLI: Version 12.x or higher.
- A good GPU to use the model. The rig that was used to develop this project has an NVIDIA GeForce RTX 4070 super. If you have lower specs I recommend to change the parameters of the model to a lower value.

## Environment Setup
1. The Docker for desktop is recommended because the Docker for desktop app configure all the require network you need to run your containers, but of course, you can try configure all manually and only use the WSL2.
2. The Node.Js on your machine is required so you run the "npm install" on the SampleAI.Ui folder. The .NET Aspire will not run this command for you. After that, theoretically, you can uninstall the Node.Js.
3. The .NET MAUI app need to be compile and deployed before you can use, or you can use the Visual Studio to run the app.
4. You can change the model running on the appsettings.json on the AppHost project. The default model is the Deepseek with 14 billions parameter, but you can change to the other models that are available on the Ollama repository.
```json
"LanguageModel": {
    "Model": "deepseek-r1:14b",
    "Port": 11434
}
```
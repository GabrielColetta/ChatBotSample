## Chatbot sample

This is a sample project that utilizes the .NET ecosystem to create a simple pre-trained chatbot using AI model. 

#### This project is my build to learn project. Do not use as a build to earn project or something like that.

### Features
- Web user interface using Angular.
- App user interface using .NET MAUI with Blazor (obsolete).
- Real-time communication with SignalR.
- Historical data storage with MongoDB Atlas locally and vector search using the embedding model to generate the information.
- .NET Aspire to configure and manage all containers.
- Gemini CLI configurations to use the assistant.

## Requirements
To run this project you will need the following prerequisites:

- .NET SDK Version 10.0 or higher.
- Docker for desktop (recommended).
- Node.js: Version 14.x or higher.
- Angular CLI: Version 12.x or higher.
- A good GPU to use the model. The rig that was used to develop this project has an NVIDIA GeForce RTX 4070 super. If you have lower specs I recommend to change the parameters of the model to a lower value.
- Run a ``dotnet user-secrets set Parameters:mongodb-password "YourSecurePassword"`` on the AppHost to create the MongoDB password.
- If you want to use the Gemini CLI, run: ``npm install -g @google/gemini-cli`` and then ``gemini`` on your terminal.

## Environment Setup
1. The Docker for desktop is recommended because the Docker for desktop app configure all the required network you need to run your containers, but of course, you can try set all manually and only use the WSL2.
2. The Node.Js on your machine is required so you run the "npm install" on the SampleAI.Ui folder. The .NET Aspire will not run this command for you. After that, theoretically, you can uninstall the Node.Js.
3. The .NET MAUI app need to be compile and deployed before you can use, or you can use the Visual Studio to run the app.
4. You can change the model running on the appsettings.json on the AppHost project and on the Test project. If you change the embeddingModel, it might be required to change the index configuration on the ``MongoDbIndexInitializer``
```json
"LanguageModel": {
    "Model": "gemma3:4b",
    "EmbeddingModel": "all-minilm:33m"
  }
```
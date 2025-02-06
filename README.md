## Aspire Project using Olamma with DeekSeep model (14B), Angular, SignalR, and MongoDB

This is an example project that utilizes the .NET Aspire framework in combination with Olamma, Angular, SignalR for real-time communication, and MongoDB for historical data storage.

### Features
- Dynamic and responsive user interface with Angular.
- Real-time communication with SignalR.
- Historical data storage in MongoDB.

## Requirements
To run this project locally, you will need the following prerequisites:

- .NET SDK Version 9.0 or higher.
- Docker for desktop.
- Node.js: Version 14.x or higher.
- Angular CLI: Version 12.x or higher.
- A good GPU to use the model. The rig that was used to develop this project has an NVIDIA GeForce RTX 4070 super. If you have lower specs I recommend to change the parameters of the model to a lower value.

### Environment Setup
Just run the .NET Aspire project and it will download all require containers and open the Aspire Dashboard. The default frontend port is 5000.
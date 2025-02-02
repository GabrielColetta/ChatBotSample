import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private connection!: signalR.HubConnection;
  private messageReceived = new Subject<string>();
  messageReceived$ = this.messageReceived.asObservable();
  private conversationId = localStorage.getItem("conversationId") || this.generateConversationId();

  constructor() {
    this.startConnection();
    this.addReceiveMessageListener();
  }

  private startConnection() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7252/chat', {
        withCredentials: false
      })
      .build();

    this.connection
      .start()
      .then(() => console.info('Start connection...'))
      .catch(err => console.error('Something went wrong: ' + err));
  }

  private addReceiveMessageListener() {
    this.connection.on('ReceiveMessageAsync', (message: string, conversationId: string) => {
      const __ = conversationId; // will be use in the future
      this.messageReceived.next(message);
    });
  }

  public sendMessage(message: string) {
    this.connection.invoke('SendMessageAsync', message, this.conversationId)
      .catch(err => console.error(err));
  }

  private generateConversationId() {
    return Math.random().toString(36).substring(2, 15);
  }
}

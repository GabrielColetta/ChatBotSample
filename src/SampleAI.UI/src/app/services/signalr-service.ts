import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { IChatResponse } from '../chatbot/chat.response';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private connection!: signalR.HubConnection;

  private messageReceived = new Subject<IChatResponse>();
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
    this.connection.on('ReceiveMessageAsync', (chatRole: string, content: string, conversationId: string) => {
      this.messageReceived.next({ chatRole, content, conversationId });
    });
  }

  public sendMessage(message: string) {
    this.connection.invoke('SendMessageAsync', message, this.conversationId)
      .catch(err => console.error(err));
  }

  private generateConversationId(): string {
    const conversationId = Math.random().toString(36).substring(2, 15);
    localStorage.setItem("conversationId", conversationId);

    return conversationId;
  }
}

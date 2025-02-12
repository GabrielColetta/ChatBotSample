import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { IChatResponse } from '../chat/chat.response';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private connection!: signalR.HubConnection;

  private messageReceived = new Subject<IChatResponse>();
  messageReceived$ = this.messageReceived.asObservable();


  constructor() {
    this.startConnection();
    this.addReceiveMessageListener();
  }

  private startConnection() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.webSocketUrl, {
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

  public sendMessage(message: string, conversationId: string) {
    this.connection.invoke('SendMessageAsync', message, conversationId)
      .catch(err => console.error(err));
  }
}

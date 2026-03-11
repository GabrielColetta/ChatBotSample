import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../environments/environment';
import { IConversationResponse } from '../conversation/conversation.response';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private connection!: signalR.HubConnection;

  private messageReceived = new Subject<IConversationResponse>();
  messageReceived$ = this.messageReceived.asObservable();


  constructor() {
    this.startConnection();
  }

  private startConnection() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.webSocketUrl, {
        withCredentials: false
      })
      .build();

    this.connection.on('ReceiveToken', (item: IConversationResponse) => {
      this.messageReceived.next(item);
    });

    this.connection
      .start()
      .then(() => console.info('Start connection...'))
      .catch(err => console.error('Something went wrong: ' + err));
  }

  public sendMessage(userPrompt: string, chatId: string | null) {
    this.connection
      .invoke('SendMessageAsync', userPrompt, chatId)
      .catch(err => console.error('Invoke failed: ' + err));
  }
}

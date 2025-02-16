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

  public sendMessage(message: string, conversationId: string | null) {
    this.connection.stream('SendMessageAsync', message, conversationId)
      .subscribe({
        next: (item: IChatResponse) => {
          this.messageReceived.next(item);
        },
        complete: () => {},
        error: (err) => console.error(err),
      });
  }
}

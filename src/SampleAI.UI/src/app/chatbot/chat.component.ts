import { Component, OnInit } from '@angular/core';
import { SignalRService } from '../services/signalr-service';
import { IChatResponse } from './chat.response';

@Component({
  standalone: false, 
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  messages: IChatResponse[] = [];
  messageToSend: string = '';

  constructor(private signalRService: SignalRService) { }

  ngOnInit(): void {
    const total = this.messages.length;
    this.signalRService.messageReceived$.subscribe((response: IChatResponse) => {
      if (total === this.messages.length) {
        this.messages.push(response);
      }
      else {
        this.messages[total+1] = response;
      }
    });
  }

  sendMessage() {
    if (this.messageToSend.trim() !== '') {
      this.signalRService.sendMessage(this.messageToSend);
      this.messageToSend = '';
    }
  }
}

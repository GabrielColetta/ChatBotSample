import { Component, OnInit } from '@angular/core';
import { SignalRService } from '../services/signalr-service';

@Component({
  standalone: false, 
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  messages: string[] = [];
  messageToSend: string = '';

  constructor(private signalRService: SignalRService) { }

  ngOnInit(): void {
    this.signalRService.messageReceived$.subscribe((message: string) => {
      this.messages.push(message);
    });
  }

  sendMessage() {
    if (this.messageToSend.trim() !== '') {
      this.signalRService.sendMessage(this.messageToSend);
      this.messageToSend = '';
    }
  }
}

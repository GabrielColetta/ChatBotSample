import { Component, OnInit } from '@angular/core';
import { SignalRService } from '../services/signalr-service';
import { IChatResponse } from './chat.response';
import { MenuItem } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { ChatRole } from '../shared/enums/chat-role.enum';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpService } from '../services/http-service';

@Component({
  standalone: false,
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  items: MenuItem[] = [
    { label: 'home' },
    { label: 'chat' }
  ];
  userForm: FormGroup<any>;

  private conversationId: string;
  messages: IChatResponse[] = [];

  constructor(private signalRService: SignalRService, private route: ActivatedRoute, private formBuilder: FormBuilder, private httpService: HttpService) {

    const conversationId = this.route.snapshot.paramMap.get('conversationId');
    if (conversationId === null) {
      this.conversationId = this.generateConversationId();
    }
    else {
      this.conversationId = conversationId;
      this.httpService
        .getById('history', conversationId)
        .subscribe((response: IChatResponse[]) => {
          this.messages = response;
        });
    }

    this.userForm = this.formBuilder.group({
      message: ['', [Validators.required, Validators.max(300)]]
    });

    this.signalRService.messageReceived$.subscribe((response: IChatResponse) => {
      this.messages[this.messages.length - 1] = response;
    });
  }

  ngOnInit(): void { }

  submitForm(): void {
    if (this.userForm.valid) {
      const message = this.userForm.get('message')!.value;
      if (message.trim() !== '') {
        this.messages.push({ chatRole: ChatRole.User, content: message, conversationId: this.conversationId });
        this.messages.push({ chatRole: ChatRole.Assistant, content: '...', conversationId: this.conversationId });

        this.signalRService.sendMessage(message, this.conversationId);
        this.userForm.reset();
      }
    }
  }

  private generateConversationId(): string {
    const conversationId = Math.random().toString(36).substring(2, 15);
    localStorage.setItem("conversationId", conversationId);

    return conversationId;
  }
}

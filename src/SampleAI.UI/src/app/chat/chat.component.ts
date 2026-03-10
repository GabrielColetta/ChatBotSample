import { Component, OnInit } from '@angular/core';
import { SignalRService } from '../services/signalr-service';
import { IChatResponse } from './chat.response';
import { MenuItem } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { ChatRole } from '../shared/enums/chat-role.enum';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpService } from '../services/http-service';
import { PaginatedResponseModel } from '../shared/models/paginated-response.model';

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

  private chatId: string | null = null;
  messages: IChatResponse[] = [];

  constructor(private signalRService: SignalRService, private route: ActivatedRoute, private formBuilder: FormBuilder, private httpService: HttpService) {
    this.userForm = this.formBuilder.group({
      message: ['', [Validators.required, Validators.max(300)]]
    });
  }

  ngOnInit(): void {
    const chatId = this.route.snapshot.paramMap.get('chatId');
    if (chatId !== null) {
      this.chatId = chatId;
      this.httpService
        .getById('conversation', chatId)
        .subscribe((response: PaginatedResponseModel<IChatResponse>) => {
          this.messages = response.data;
        });
    }

    this.signalRService.messageReceived$.subscribe((response: IChatResponse) => {
      this.messages[this.messages.length - 1].content += response.content;
    });

    this.route.queryParamMap.subscribe(params => {
      const initialMessage = params.get('message');
      if (initialMessage) {
        this.userForm.patchValue({ message: initialMessage });
        // Small delay to ensure SignalR connection is ready
        setTimeout(() => this.submitForm(), 500);
      }
    });
  }

  submitForm(): void {
    if (this.userForm.valid) {
      const message = this.userForm.get('message')!.value;
      if (message.trim() !== '') {
        this.messages.push({ chatRole: ChatRole.User, content: message, chatId: this.chatId });
        this.messages.push({ chatRole: ChatRole.Assistant, content: '', chatId: this.chatId });
        this.signalRService.sendMessage(message, this.chatId);
        this.userForm.reset();
      }
    }
  }
}

import { Component, OnInit } from '@angular/core';
import { SignalRService } from '../services/signalr-service';
import { IConversationResponse } from './conversation.response';
import { MenuItem } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { ChatRole } from '../shared/enums/chat-role.enum';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpService } from '../services/http-service';
import { PaginatedResponseModel } from '../shared/models/paginated-response.model';

@Component({
  standalone: false,
  templateUrl: './conversation.component.html',
  styleUrls: ['./conversation.component.css']
})
export class ConversationComponent implements OnInit {
  items: MenuItem[] = [
    { label: 'home' },
    { label: 'chat' }
  ];
  userForm: FormGroup<any>;

  private conversationId: string | null = null;
  messages: IConversationResponse[] = [];

  constructor(private signalRService: SignalRService, private route: ActivatedRoute, private formBuilder: FormBuilder, private httpService: HttpService) {
    this.userForm = this.formBuilder.group({
      message: ['', [Validators.required, Validators.maxLength(300)]]
    });
  }

  ngOnInit(): void {
    const conversationId = this.route.snapshot.paramMap.get('conversationId');
    if (conversationId !== null) {
      this.conversationId = conversationId;
      this.httpService
        .getById('conversation', conversationId)
        .subscribe((response: PaginatedResponseModel<IConversationResponse>) => {
          this.messages = response.data;
        });
    }

    this.signalRService.messageReceived$.subscribe((response: IConversationResponse) => {
      if (this.messages.length > 0) {
        this.messages[this.messages.length - 1].content += response.content;
      }
    });

    this.route.queryParamMap.subscribe(params => {
      const initialMessage = params.get('message');
      if (initialMessage) {
        this.userForm.patchValue({ message: initialMessage });
        setTimeout(() => this.submitForm(), 500);
      }
    });
  }

  submitForm(): void {
    if (this.userForm.valid) {
      const message = this.userForm.get('message')!.value;
      if (message.trim() !== '') {
        this.messages.push({ chatRole: ChatRole.User, content: message, chatId: this.conversationId });
        this.messages.push({ chatRole: ChatRole.Assistant, content: '', chatId: this.conversationId });
        this.signalRService.sendMessage(message, this.conversationId);
        this.userForm.reset();
      }
    }
  }
}

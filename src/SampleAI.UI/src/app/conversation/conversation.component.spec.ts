import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { of, Subject } from 'rxjs';
import { ConversationComponent } from './conversation.component';
import { SignalRService } from '../services/signalr-service';
import { HttpService } from '../services/http-service';
import { ChatRole } from '../shared/enums/chat-role.enum';
import { IConversationResponse } from './conversation.response';
import { PaginatedResponseModel } from '../shared/models/paginated-response.model';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('ConversationComponent', () => {
  let component: ConversationComponent;
  let fixture: ComponentFixture<ConversationComponent>;
  let signalRServiceMock: any;
  let httpServiceMock: any;
  let routeMock: any;
  let messageReceivedSubject: Subject<IConversationResponse>;

  let mockMessages: IConversationResponse[];
  let mockPaginatedResponse: PaginatedResponseModel<IConversationResponse>;

  beforeEach(async () => {
    mockMessages = [
      { chatRole: ChatRole.User, content: 'Hello', chatId: '123' },
      { chatRole: ChatRole.Assistant, content: 'Hi there!', chatId: '123' }
    ];

    mockPaginatedResponse = {
      data: mockMessages,
      total: 2
    };

    messageReceivedSubject = new Subject<IConversationResponse>();
    
    signalRServiceMock = {
      messageReceived$: messageReceivedSubject.asObservable(),
      sendMessage: jasmine.createSpy('sendMessage')
    };

    httpServiceMock = {
      getById: jasmine.createSpy('getById').and.returnValue(of(mockPaginatedResponse))
    };

    routeMock = {
      snapshot: {
        paramMap: convertToParamMap({ conversationId: '123' })
      },
      queryParamMap: of(convertToParamMap({}))
    };

    await TestBed.configureTestingModule({
      imports: [ ReactiveFormsModule ],
      declarations: [ ConversationComponent ],
      providers: [
        { provide: SignalRService, useValue: signalRServiceMock },
        { provide: HttpService, useValue: httpServiceMock },
        { provide: ActivatedRoute, useValue: routeMock },
        FormBuilder
      ],
      schemas: [ NO_ERRORS_SCHEMA ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConversationComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should load messages if conversationId is present in route', () => {
    fixture.detectChanges();
    expect(httpServiceMock.getById).toHaveBeenCalledWith('conversation', '123');
    expect(component.messages).toEqual(mockMessages);
  });

  it('should NOT load messages if conversationId is NOT present in route', () => {
    routeMock.snapshot.paramMap = convertToParamMap({});
    fixture = TestBed.createComponent(ConversationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    expect(httpServiceMock.getById).not.toHaveBeenCalled();
    expect(component.messages.length).toBe(0);
  });

  it('should append content to the last message when SignalR receives a message', () => {
    fixture.detectChanges();
    const initialLastMessageContent = component.messages[component.messages.length - 1].content;
    const additionalContent = ' Streaming text';
    
    messageReceivedSubject.next({ chatRole: ChatRole.Assistant, content: additionalContent, chatId: '123' });
    
    expect(component.messages[component.messages.length - 1].content).toBe(initialLastMessageContent + additionalContent);
  });

  it('should submit form and send message through SignalR', () => {
    fixture.detectChanges();
    const testMessage = 'New test message';
    component.userForm.get('message')?.setValue(testMessage);

    component.submitForm();

    expect(component.messages.length).toBe(4);
    expect(component.messages[2]).toEqual({ chatRole: ChatRole.User, content: testMessage, chatId: '123' });
    expect(component.messages[3]).toEqual({ chatRole: ChatRole.Assistant, content: '', chatId: '123' });
    expect(signalRServiceMock.sendMessage).toHaveBeenCalledWith(testMessage, '123');
    expect(component.userForm.get('message')?.value).toBeNull();
  });

  it('should not submit if message is empty or whitespace', () => {
    fixture.detectChanges();
    component.userForm.get('message')?.setValue('   ');
    component.submitForm();
    expect(signalRServiceMock.sendMessage).not.toHaveBeenCalled();
  });

  it('should handle initial message from query parameters', fakeAsync(() => {
    const initialMsg = 'Hello from URL';
    routeMock.queryParamMap = of(convertToParamMap({ message: initialMsg }));
    
    fixture = TestBed.createComponent(ConversationComponent);
    component = fixture.componentInstance;
    const submitSpy = spyOn(component, 'submitForm').and.callThrough();
    
    fixture.detectChanges();
    
    expect(component.userForm.get('message')?.value).toBe(initialMsg);
    tick(500);
    expect(submitSpy).toHaveBeenCalled();
    expect(signalRServiceMock.sendMessage).toHaveBeenCalledWith(initialMsg, '123');
  }));

  it('should initialize menu items', () => {
    fixture.detectChanges();
    expect(component.items.length).toBe(2);
    expect(component.items[0].label).toBe('home');
    expect(component.items[1].label).toBe('chat');
  });
});

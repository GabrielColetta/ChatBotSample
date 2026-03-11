import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { ChatComponent } from './chat.component';
import { HttpService } from '../services/http-service';
import { PaginatedResponseModel } from '../shared/models/paginated-response.model';
import { IChatResponse } from './chat.response';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('ChatComponent', () => {
  let component: ChatComponent;
  let fixture: ComponentFixture<ChatComponent>;
  let httpServiceMock: any;
  let routerMock: any;

  let mockChats: IChatResponse[];
  let mockPaginatedResponse: PaginatedResponseModel<IChatResponse>;

  beforeEach(async () => {
    mockChats = [
      { chatId: '1', title: 'Chat 1', date: '2023-01-01' },
      { chatId: '2', title: 'Chat 2', date: '2023-01-02' }
    ];

    mockPaginatedResponse = {
      data: mockChats,
      total: 2
    };

    httpServiceMock = {
      getPaginated: jasmine.createSpy('getPaginated').and.returnValue(of(mockPaginatedResponse)),
      search: jasmine.createSpy('search').and.returnValue(of({ data: [{ chatId: '3', content: 'Search 1', date: '2023-01-03' }], total: 1 }))
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      declarations: [ ChatComponent ],
      providers: [
        { provide: HttpService, useValue: httpServiceMock },
        { provide: Router, useValue: routerMock }
      ],
      schemas: [ NO_ERRORS_SCHEMA ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChatComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should load initial history on ngOnInit', () => {
    fixture.detectChanges();
    expect(httpServiceMock.getPaginated).toHaveBeenCalledWith('chat', { perPage: 20, currentPage: 0 });
    expect(component.chats).toEqual(mockChats);
  });

  it('should initialize menu items on ngOnInit', () => {
    fixture.detectChanges();
    expect(component.items).toBeDefined();
    expect(component.items?.[0].label).toBe('home');
  });

  it('should call performSearch after debounce time on onSearch', fakeAsync(() => {
    fixture.detectChanges();
    const event = { target: { value: 'test search' } } as any;
    component.onSearch(event);

    expect(httpServiceMock.search).not.toHaveBeenCalled();

    tick(2000);
    expect(httpServiceMock.search).toHaveBeenCalledWith('chat/search', 'test search');
  }));

  it('should map search results correctly in performSearch', () => {
    const searchTerm = 'query';
    component.performSearch(searchTerm);

    expect(httpServiceMock.search).toHaveBeenCalledWith('chat/search', searchTerm);
    expect(component.chats.length).toBe(1);
    expect(component.chats[0]).toEqual({
      chatId: '3',
      title: 'Search 1',
      date: '2023-01-03'
    });
  });

  it('should call loadInitialHistory when performSearch is called with empty string', () => {
    const spy = spyOn(component, 'loadInitialHistory');
    component.performSearch('   ');
    expect(spy).toHaveBeenCalled();
    expect(httpServiceMock.search).not.toHaveBeenCalled();
  });

  it('should navigate to conversation with id on continueChat', () => {
    const chatId = '123';
    component.continueChat(chatId);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/conversation', chatId]);
  });

  it('should navigate to new conversation on newChat', () => {
    component.newChat();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/conversation']);
  });

  it('should unsubscribe from searchSubscription on ngOnDestroy', () => {
    fixture.detectChanges();
    const unsubscribeSpy = spyOn((component as any).searchSubscription, 'unsubscribe');
    component.ngOnDestroy();
    expect(unsubscribeSpy).toHaveBeenCalled();
  });
});

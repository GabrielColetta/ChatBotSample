import { Component, OnDestroy, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { HttpService } from '../services/http-service';
import { PaginatedResponseModel } from '../shared/models/paginated-response.model';
import { IChatResponse } from './chat.response';
import { Router } from '@angular/router';
import { debounceTime, distinctUntilChanged, Subject, Subscription } from 'rxjs';

@Component({
  standalone: false,
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy {

  items: MenuItem[] | undefined;
  chats: IChatResponse[] = [];
  private searchSubject = new Subject<string>();
  private searchSubscription: Subscription | undefined;

  constructor(private httpService: HttpService, private router: Router) {

  }

  ngOnInit(): void {
    this.items = [
      { label: 'home' },
    ];

    this.loadInitialHistory();

    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(2000),
      distinctUntilChanged()
    ).subscribe(term => {
      this.performSearch(term);
    });
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }

  loadInitialHistory() {
    this.httpService
      .getPaginated('chat', {
        perPage: 20,
        currentPage: 0
      })
      .subscribe((response: PaginatedResponseModel<IChatResponse>) => {
        this.chats = response.data;
      });
  }

  onSearch(event: Event) {
    const term = (event.target as HTMLInputElement).value;
    this.searchSubject.next(term);
  }

  performSearch(search: string) {
    if (!search.trim()) {
      this.loadInitialHistory();
      return;
    }

    this.httpService
      .search('chat/search', search)
      .subscribe((response: PaginatedResponseModel<any>) => {
        this.chats = response.data.map((item: any) => ({
          chatId: item.chatId,
          title: item.content,
          date: item.date
        }));
      });
  }

  continueChat(chatId: string) {
    this.router.navigate(['/conversation', chatId]);
  }

  newChat() {
    this.router.navigate(['/conversation']);
  }
}

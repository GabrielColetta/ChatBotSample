import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { HttpService } from '../services/http-service';
import { PaginatedResponseModel } from '../shared/models/paginated-response.model';
import { HistoryResponse } from './history.response';
import { Router } from '@angular/router';

@Component({
  standalone: false,
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css']
})
export class HistoryComponent implements OnInit {
  items: MenuItem[] | undefined;
  historic: HistoryResponse[] = [];

  constructor(private httpService: HttpService, private router: Router) {
  }

  ngOnInit(): void {
    this.items = [
      { label: 'home' },
    ];

    this.httpService
      .getPaginated('history', {
        perPage: 20,
        currentPage: 0
      })
      .subscribe((response: PaginatedResponseModel<HistoryResponse>) => {
        this.historic = response.data;
    })
  }

  continueChat(conversationId: string) {
    this.router.navigate(['/chat', conversationId]);
  }

  newChat() {
    this.router.navigate(['/chat']);
  }
}

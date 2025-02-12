import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChatComponent } from './chat/chat.component';
import { HistoryComponent } from './history/history.component';

export const routes: Routes = [
  {
    path: '', component: HistoryComponent
  },
  {
    path: 'chat', component: ChatComponent
  },
  {
    path: 'chat/:conversationId', component: ChatComponent
  }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

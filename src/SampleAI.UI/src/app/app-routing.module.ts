import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChatComponent } from './chat/chat.component';
import { ConversationComponent } from './conversation/conversation.component';

export const routes: Routes = [
  {
    path: '', component: ChatComponent
  },
  {
    path: 'conversation', component: ConversationComponent
  },
  {
    path: 'conversation/:conversationId', component: ConversationComponent
  }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatPageComponent } from './chat-page/chat-page.component';
import { ChatsComponent } from './chats/chats.component';
import { ChatItemComponent } from './chat-item/chat-item.component';



@NgModule({
  declarations: [
    ChatPageComponent,
    ChatsComponent,
    ChatItemComponent
  ],
  imports: [
    CommonModule
  ]
})
export class ChatModule { }

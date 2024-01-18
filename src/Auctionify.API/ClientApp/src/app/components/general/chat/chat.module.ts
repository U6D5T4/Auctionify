import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatPageComponent } from './chat-page/chat-page.component';
import { ChatsComponent } from './chats/chats.component';
import { ChatItemComponent } from './chat-item/chat-item.component';
import { ConversationWindowComponent } from './conversation-window/conversation-window.component';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@NgModule({
    declarations: [
        ChatPageComponent,
        ChatsComponent,
        ChatItemComponent,
        ConversationWindowComponent,
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule,
    ],
})
export class ChatModule {}

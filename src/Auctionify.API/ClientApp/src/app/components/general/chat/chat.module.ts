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
import { MessageObserverDirective } from 'src/app/directives/message-observer.directive';
import { RouterModule } from '@angular/router';

@NgModule({
    declarations: [
        ChatPageComponent,
        ChatsComponent,
        ChatItemComponent,
        ConversationWindowComponent,
        MessageObserverDirective,
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule,
        RouterModule,
    ],
})
export class ChatModule {}
